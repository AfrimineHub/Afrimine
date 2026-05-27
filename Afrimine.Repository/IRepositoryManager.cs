using Microsoft.EntityFrameworkCore;

namespace Afrimine.Repository
{
    public interface IRepositoryManager
    {
        IOtpRepository Otp {  get; }
        ISendEmailRepository SendEmail { get; }

        Task SaveAsync();
        IVendorProfileRepository VendorProfile { get; }
    }
}
