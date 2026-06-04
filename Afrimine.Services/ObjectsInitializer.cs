using Afrimine.Model.Entities;
using Afrimine.Model.Enums;
using Afrimine.Services.DTOs;
using Afrimine.Shared.Extensions;

namespace Afrimine.Services
{
    internal static class ObjectsInitializer
    {
        public static User Initialize(this RegisterRequestDto requestDto)
        {
            return new User
            {
                Email = requestDto.Email,
                PhoneNumber = requestDto.Phone,
                FullName = requestDto.FullName.CapitalizeWords(),
                UserName = requestDto.Email,
                Status = AccountStatus.Pending,
                Type = requestDto.Type
            };
        }

        public static OtpEntry InitializeOtpEntry(string userId, string otpHash, EToken type, int expires = 5, string? tokenHash = null)
        {
            return new OtpEntry
            {
                UserId = userId,
                TokenHash = tokenHash,
                OtpHash = otpHash,
                Type = type,
                ExpiresAt = DateTime.UtcNow.AddMinutes(expires)
            };
        }
    }
}
