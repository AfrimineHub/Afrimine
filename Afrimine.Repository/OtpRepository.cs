using Afrimine.Migrations;
using Afrimine.Model.Entities;
using Afrimine.Model.Enums;
using Microsoft.EntityFrameworkCore;

namespace Afrimine.Repository
{
    public class OtpRepository : RepositoryBase<OtpEntry>, IOtpRepository
    {
        public OtpRepository(AppDbContext appDbContext)
        : base(appDbContext){}

        public async Task CreateToken(OtpEntry otp) => await Create(otp);

        public async Task<OtpEntry?> GetOtp(string UserId, EToken type,string hash) =>
            await FindByCondition(a => a.UserId.Equals(UserId) && a.Type == type && a.OtpHash == hash, true)
            .OrderByDescending(a => a.CreatedAt)
            .FirstOrDefaultAsync();

        public void UpdateToken (OtpEntry otp) => Update(otp);
        public void DeleteToken(OtpEntry otp) => Delete(otp);

        public async Task<OtpEntry?> GetOtpByUser(string userId, EToken type) =>
            await FindByCondition(a => a.UserId.Equals(userId) && a.Type == type, true)
            .OrderByDescending(a => a.CreatedAt)
            .FirstOrDefaultAsync();
    }
}
