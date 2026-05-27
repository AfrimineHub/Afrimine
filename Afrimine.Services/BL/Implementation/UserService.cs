using Afrimine.Model.Entities;
using Afrimine.Model.Enums;
using Afrimine.Model.ViewModels;
using Afrimine.Repository;
using Afrimine.Services.BL.Interfaces;
using Afrimine.Services.DTOs;
using Afrimine.Services.Responses;
using Afrimine.Services.Validators;
using Afrimine.Shared.Configs;
using Afrimine.Shared.ExternalServices;
using Afrimine.Shared.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Role = Afrimine.Model.Enums.Role;

namespace Afrimine.Services.BL.Implementation
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly AppConfig _settings;
        private readonly IRepositoryManager _repositoryManager;

        public UserService(UserManager<User> userManager,
                        SignInManager<User> signInManager, IOptions<AppConfig> options, IRepositoryManager repositoryManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _settings = options.Value;
            _repositoryManager = repositoryManager;
        }

        public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request)
        {
            var validationResult = await ValidateUser(request);
            if (!validationResult.Success)
            {
                return ApiResponse<LoginResponseDto>.Fail(validationResult.Message, validationResult.StatusCode);
            }

            var (user, roles) = validationResult.Data;
            var accessToken = CreateAccessToken(user.Id, user.Email!, roles);
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            user.LastLogin = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);

            return ApiResponse<LoginResponseDto>.Ok(new LoginResponseDto(accessToken), refreshToken);
        }

        public async Task<ApiResponse<CurrentUserDto>> GetCurrentUser(string? userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return ApiResponse<CurrentUserDto>.Fail(ResponseMessages.NotAuthenticated, StatusCodes.Status401Unauthorized);
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return ApiResponse<CurrentUserDto>.Fail(ResponseMessages.UserNotFound, StatusCodes.Status404NotFound);
            }

            return ApiResponse<CurrentUserDto>.Ok(new CurrentUserDto
            {
                Name = user.FullName,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber!,
                Status = user.Status
            });
        }

        public async Task<ApiResponse<string>> RegisterUserAsync(RegisterRequestDto request)
        {
            var validate = new RegistrationRequestValidator().Validate(request);
            if (!validate.IsValid)
            {
                return ApiResponse<string>.Fail(validate.Errors.FirstOrDefault()?.ErrorMessage ?? ResponseMessages.InvalidRequest, 400);
            }

            // Only these roles can self-register
            var allowedRoles = new[]
            {
                Role.Buyer,
                Role.Vendor,
                Role.Support,
                Role.Investor
            };

            if (!allowedRoles.Contains(request.Type))
                return ApiResponse<string>.Fail(string.Format(ResponseMessages.InvalidRegistrationRole, request.Type), 403);

            if (request.Type == Role.SuperAdmin || request.Type == Role.Support)
            {
                return ApiResponse<string>.Fail(string.Format(ResponseMessages.InvalidRegistrationRole, request.Type), 403);
            }

            var existing = await _userManager.Users.AnyAsync(u => u.Email == request.Email || u.PhoneNumber == request.Phone);

            if (existing)
            {
                return ApiResponse<string>.Fail(ResponseMessages.ExistingUser, 409);
            }

            var user = request.Initialize();
            var createResult = await _userManager.CreateAsync(user, request.Password);

            if (!createResult.Succeeded)
            {
                return ApiResponse<string>.Fail(createResult.Errors?.FirstOrDefault()?.Description ?? ResponseMessages.RegistrationFailed, 400);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, request.Type.ToString());
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);

                return ApiResponse<string>.Fail(roleResult.Errors?.FirstOrDefault()?.Description ?? ResponseMessages.RegistrationFailed, 400);
            }

            var otp = TokenHelpers.GenerateOtp();
            var hash = TokenHelpers.HashToken(otp, _settings.JwtKey);
            var tokenEntry = ObjectsInitializer.InitializeOtpEntry(user.Id, hash, EToken.ConfirmEmail);

            await _repositoryManager.Otp.CreateToken(tokenEntry);
            await _repositoryManager.SaveAsync();

            var html = GetEmailTemplate.GetConfirmEmailTemplate(otp);

            Notifications.SendEmail(user.Email!, "Confirm Email Address", html, html);

            return ApiResponse<string>.Ok(user.Email!, 200, "OTP sent successfully");
        }

        public async Task<ApiResponse<LoginResponseDto>> RefreshTokenAsync(string? refreshToken, RefreshTokenRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return ApiResponse<LoginResponseDto>.Fail("Refresh token missing.", StatusCodes.Status401Unauthorized);

            var principal = GetPrincipalFromExpiredToken(request.AccessToken);
            if (principal == null)
                return ApiResponse<LoginResponseDto>.Fail("Invalid access token.", StatusCodes.Status401Unauthorized);

            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId!);

            if (user == null
                || user.RefreshToken != refreshToken
                || user.RefreshTokenExpiry <= DateTime.UtcNow)
            {
                return ApiResponse<LoginResponseDto>.Fail("Invalid or expired refresh token.", StatusCodes.Status401Unauthorized);
            }

            var roles = (await _userManager.GetRolesAsync(user)).ToArray();
            var newAccessToken = CreateAccessToken(user.Id, user.Email!, roles);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return ApiResponse<LoginResponseDto>.Ok(new LoginResponseDto(newAccessToken), newRefreshToken);
        }

        public async Task<ApiResponse<string>> RevokeTokenAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return ApiResponse<string>.Fail(ResponseMessages.UserNotFound, StatusCodes.Status404NotFound);

            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;
            await _userManager.UpdateAsync(user);

            return ApiResponse<string>.Ok("Token revoked successfully.");
        }

        public async Task<ApiResponse<string>> ConfirmEmail(OtpForCreationDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return ApiResponse<string>.Fail(string.Format(ResponseMessages.UserRecordNotFound, model.Email), 404);
            }

            var hash = TokenHelpers.HashToken(model.Otp, _settings.JwtKey);

            var otp = await _repositoryManager.Otp.GetOtp(user.Id, model.Type, hash);

            var otpResult = ValidateOtp(otp);

            if (!otpResult.Success)
            {
                return otpResult;
            }

            user.EmailConfirmed = true;
            user.Status = AccountStatus.Active;

            if (otp != null)
            {
                _repositoryManager.Otp.DeleteToken(otp);
            }

            await _userManager.UpdateAsync(user);
            await _repositoryManager.SaveAsync();

            return ApiResponse<string>.Ok(user.Email!, 200, "Email confirmed successfully");
        }

        public async Task<ApiResponse<string>> ResetPassword(PasswordResetDto passwordResetDto)
        {
            var user = await _userManager.FindByEmailAsync(passwordResetDto.Email);

            if (user == null)
            {
                return ApiResponse<string>.Fail(ResponseMessages.UserNotFound, StatusCodes.Status404NotFound);
            }

            var otp = TokenHelpers.GenerateOtp();
            var hash = TokenHelpers.HashToken(otp, _settings.JwtKey);
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var tokenHash = TokenHelpers.Encrypt(token, _settings.JwtKey);
            var tokenEntry = ObjectsInitializer.InitializeOtpEntry(user.Id, hash, EToken.ResetPassword, tokenHash: tokenHash);

            await _repositoryManager.Otp.CreateToken(tokenEntry);
            await _repositoryManager.SaveAsync();

            var html = GetEmailTemplate.GetResetPasswordEmailTemplate(otp);

            Notifications.SendEmail(user.Email!, "Reset Password", html, html);

            return ApiResponse<string>.Ok(user.Email!, 200, "OTP sent successfully");
        }

        public async Task<ApiResponse<string>> ChangeForgottenPassword(ChangeForgotPasswordRequestModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return ApiResponse<string>.Fail(ResponseMessages.UserNotFound, StatusCodes.Status404NotFound);
            }

            var otpHash = TokenHelpers.HashToken(model.Otp, _settings.JwtKey);
            var otp = await _repositoryManager.Otp.GetOtp(user.Id, EToken.ResetPassword, otpHash);

            if (otp == null)
            {
                return ApiResponse<string>.Fail(ResponseMessages.InvalidOtp, StatusCodes.Status400BadRequest);
            }

            if (otp.ExpiresAt < DateTime.UtcNow)
            {
                return ApiResponse<string>.Fail("Reset token is expired", StatusCodes.Status400BadRequest);
            }

            if (string.IsNullOrWhiteSpace(otp.TokenHash))
            {
                return ApiResponse<string>.Fail("Reset token is invalid", StatusCodes.Status400BadRequest);
            }

            var token = TokenHelpers.Decrypt(otp.TokenHash, _settings.JwtKey);
            var changePassword = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

            if (!changePassword.Succeeded)
            {
                return ApiResponse<string>.Fail(changePassword.Errors.FirstOrDefault()?.Description ?? ResponseMessages.PasswordResetFailed, StatusCodes.Status400BadRequest);
            }

            _repositoryManager.Otp.DeleteToken(otp);
            await _repositoryManager.SaveAsync();

            return ApiResponse<string>.Ok(user.Email!, StatusCodes.Status200OK, "Password reset successful");
        }

        public async Task<ApiResponse<string>> ResendOtpAsync(ResendOtpRequestDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return ApiResponse<string>.Fail(ResponseMessages.UserNotFound, StatusCodes.Status404NotFound);

            if (user.EmailConfirmed)
                return ApiResponse<string>.Fail("Email is already confirmed.", StatusCodes.Status400BadRequest);

            var existingToken = await _repositoryManager.Otp.GetOtpByUser(user.Id, EToken.ConfirmEmail);
            if (existingToken != null)
            {
                _repositoryManager.Otp.DeleteToken(existingToken);
                await _repositoryManager.SaveAsync();
            }

            var otp = TokenHelpers.GenerateOtp();
            var hash = TokenHelpers.HashToken(otp, _settings.JwtKey);
            var tokenEntry = ObjectsInitializer.InitializeOtpEntry(user.Id, hash, EToken.ConfirmEmail);

            await _repositoryManager.Otp.CreateToken(tokenEntry);
            await _repositoryManager.SaveAsync();

            var html = GetEmailTemplate.GetConfirmEmailTemplate(otp);
            Notifications.SendEmail(user.Email!, "Confirm Email Address", html, html);

            return ApiResponse<string>.Ok(user.Email!, 200, "OTP resent successfully.");
        }

        public async Task<ApiResponse<string>> SetupBusinessProfileAsync(string userId, BusinessProfileDto request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return ApiResponse<string>.Fail(ResponseMessages.UserNotFound, StatusCodes.Status404NotFound);

            var roles = await _userManager.GetRolesAsync(user);
            var isVendor = VendorRoles.Any(r => roles.Contains(r.ToString()));
            if (!isVendor)
                return ApiResponse<string>.Fail("Only vendors can setup a business profile.", StatusCodes.Status403Forbidden);

            var profile = await _repositoryManager.VendorProfile.GetByUserId(userId);
            if (profile == null)
            {
                profile = new VendorProfile
                {
                    Id = Guid.NewGuid(),
                    UserId = userId
                };
                await _repositoryManager.VendorProfile.Create(profile);
            }

            profile.BusinessType = request.BusinessType;
            profile.Country = request.Country;
            profile.StateOrRegion = request.StateOrRegion;
            profile.OfficeAddress = request.OfficeAddress;
            profile.Website = request.Website;
            profile.OnboardingStep = 2;
            profile.UpdatedAt = DateTime.UtcNow;

            await _repositoryManager.SaveAsync();

            return ApiResponse<string>.Ok("Business profile saved successfully.", 200);
        }

        public async Task<ApiResponse<string>> UploadKycAsync(string userId, KycUploadDto request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return ApiResponse<string>.Fail(ResponseMessages.UserNotFound, StatusCodes.Status404NotFound);

            var profile = await _repositoryManager.VendorProfile.GetByUserId(userId);
            if (profile == null)
                return ApiResponse<string>.Fail("Please complete business profile setup first.", StatusCodes.Status400BadRequest);

            if (profile.OnboardingStep < 2)
                return ApiResponse<string>.Fail("Please complete business profile setup first.", StatusCodes.Status400BadRequest);

            var fileUrl = await UploadFileAsync(request.File);

            profile.DocumentType = request.DocumentType;
            profile.DocumentUrl = fileUrl;
            profile.DocumentFileName = request.File.FileName;
            profile.OnboardingStep = 3;
            profile.IsComplete = true;
            profile.UpdatedAt = DateTime.UtcNow;

            await _repositoryManager.SaveAsync();

            return ApiResponse<string>.Ok("KYC document uploaded successfully.", 200);
        }

        public async Task<ApiResponse<VendorProfileResponseDto>> GetVendorProfileAsync(string userId)
        {
            var profile = await _repositoryManager.VendorProfile.GetByUserId(userId);
            if (profile == null)
                return ApiResponse<VendorProfileResponseDto>.Fail("Profile not found.", StatusCodes.Status404NotFound);

            return ApiResponse<VendorProfileResponseDto>.Ok(new VendorProfileResponseDto
            {
                BusinessType = profile.BusinessType,
                Country = profile.Country,
                StateOrRegion = profile.StateOrRegion,
                OfficeAddress = profile.OfficeAddress,
                Website = profile.Website,
                DocumentType = profile.DocumentType,
                DocumentFileName = profile.DocumentFileName,
                DocumentUrl = profile.DocumentUrl,
                OnboardingStep = profile.OnboardingStep,
                IsComplete = profile.IsComplete
            });
        }

        #region Private Methods
        async Task<ApiResponse<(User user, string[] roles)>> ValidateUser(LoginRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return ApiResponse<(User user, string[] roles)>.Fail(ResponseMessages.InvalidEmailOrPassword, 400);
            }

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return ApiResponse<(User user, string[] roles)>.Fail(ResponseMessages.UserRecordNotFound, 404);
            }

            if (!user.EmailConfirmed || user.Status != AccountStatus.Active)
            {
                return ApiResponse<(User user, string[] roles)>.Fail(ResponseMessages.EmailNotConfirmedOrInactive, 403);
            }

            var check = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: true);
            if (!check.Succeeded)
            {
                return ApiResponse<(User user, string[] roles)>.Fail(ResponseMessages.WrongPassword, 403);
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (roles == null || roles.Count == 0)
            {
                return ApiResponse<(User user, string[] roles)>.Fail(ResponseMessages.NoAssignedRole, 403);
            }

            return ApiResponse<(User user, string[] roles)>.Ok((user, roles.ToArray()));
        }

        string CreateAccessToken(string userid, string email, string[] roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userid),
                new Claim(ClaimTypes.Name, email),
            };

            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var key = Encoding.UTF8.GetBytes(_settings.JwtKey);
            var secret = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);

            var now = DateTime.UtcNow;
            var jwtToken = new JwtSecurityToken(
                    issuer: _settings.JwtIssuer,
                    audience: _settings.JwtAudience,
                    claims: claims,
                    notBefore: now,
                    expires: now.AddMinutes(_settings.JwtExpirationMinutes),
                    signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }

        private async Task<string> UploadFileAsync(IFormFile file)
        {
            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            Directory.CreateDirectory(uploads);

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploads, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return filePath; // replace with your CDN/storage URL in production
        }

        private ApiResponse<string> ValidateOtp(OtpEntry? otp)
        {
            if (otp == null)
            {
                return ApiResponse<string>.Fail(ResponseMessages.OtpNotFound, StatusCodes.Status404NotFound);
            }

            if (otp.ExpiresAt < DateTime.Now)
            {
                return ApiResponse<string>.Fail(ResponseMessages.OtpExpired, StatusCodes.Status404NotFound);
            }

            return ApiResponse<string>.Ok("OTP validated successfully");
        }

        string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false, // allow expired tokens
                ValidateIssuerSigningKey = true,
                ValidIssuer = _settings.JwtIssuer,
                ValidAudience = _settings.JwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.JwtKey))
            };

            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return principal;
        }
        
        private static readonly Role[]
            VendorRoles =
            {
            Role.Buyer,
            Role.Vendor,
            Role.Support
        };
        #endregion
    }
}