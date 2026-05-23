using Afrimine.Model.ViewModels;
using Afrimine.Services.DTOs;
using Afrimine.Services.Responses;

namespace Afrimine.Services.BL.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse<string>> ChangeForgottenPassword(ChangeForgotPasswordRequestModel model);
        Task<ApiResponse<string>> ConfirmEmail(OtpForCreationDto model);
        Task<ApiResponse<CurrentUserDto>> GetCurrentUser(string? userId);
        Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request);
        Task<ApiResponse<LoginResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request);
        Task<ApiResponse<string>> RegisterUserAsync(RegisterRequestDto request);
        Task<ApiResponse<string>> ResetPassword(PasswordResetDto passwordResetDto);
        Task<ApiResponse<string>> RevokeTokenAsync(string userId);
    }
}
