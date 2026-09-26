using Afrimine.Model.Entities;
using Afrimine.Model.Enums;

namespace Afrimine.Repository
{
    public interface IOtpRepository
    {
        Task CreateToken(OtpEntry otp);
        void DeleteToken(OtpEntry otp);
        Task<OtpEntry?> GetOtp(string UserId, EToken type, string hash);
        Task<OtpEntry?> GetOtpByUser(string userId, EToken type);
        void UpdateToken(OtpEntry otp);

    }
}
