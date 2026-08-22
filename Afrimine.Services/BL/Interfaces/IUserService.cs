using Afrimine.Model.ViewModels;
using Afrimine.Services.DTOs;
using Afrimine.Services.Responses;
using static Afrimine.Services.DTOs.AuthDto;

namespace Afrimine.Services.BL.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse<string>> ChangeForgottenPassword(ChangeForgotPasswordRequestModel model);
        Task<ApiResponse<string>> ConfirmEmail(OtpForCreationDto model);
        Task<ApiResponse<CurrentUserDto>> GetCurrentUser(string? userId);
        Task<ApiResponse<VendorProfileResponseDto>> GetVendorProfileAsync(string userId);
        Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request);
        Task<ApiResponse<LoginResponseDto>> RefreshTokenAsync(string? refreshToken, RefreshTokenRequestDto request);
        Task<ApiResponse<string>> RegisterUserAsync(RegisterRequestDto request);
        Task<ApiResponse<string>> ResendOtpAsync(ResendOtpRequestDto request);
        Task<ApiResponse<string>> ResetPassword(PasswordResetDto passwordResetDto);
        Task<ApiResponse<string>> RevokeTokenAsync(string userId);
        Task<ApiResponse<string>> SetupBusinessProfileAsync(string userId, BusinessProfileDto model);
        Task<ApiResponse<string>> UploadKycAsync(string userId, KycUploadDto request);
        Task<ApiResponse<string>> ChangePasswordAsync(string userId, ChangePasswordDto request);
        Task<ApiResponse<string>> UploadProfilePhotoAsync(string userId, ProfilePhotoUploadDto request);
    }
}
