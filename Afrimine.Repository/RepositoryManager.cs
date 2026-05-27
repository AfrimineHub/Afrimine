using Afrimine.Migrations;

namespace Afrimine.Repository
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly AppDbContext _repositoryContext;
        private readonly Lazy<IOtpRepository> _otpRepository;
        private readonly Lazy<ISendEmailRepository> _sendEmailRepository;
        public RepositoryManager(AppDbContext appDbContext)
        {
            _repositoryContext = appDbContext;
            _otpRepository = new Lazy<IOtpRepository>(() => new OtpRepository(appDbContext));
            _sendEmailRepository = new Lazy<ISendEmailRepository>(() => new SendEmailRepository(appDbContext));
        }

        public IOtpRepository Otp => _otpRepository.Value;
        public ISendEmailRepository SendEmail => _sendEmailRepository.Value;
        public async Task SaveAsync() => await _repositoryContext.SaveChangesAsync();
        public IVendorProfileRepository VendorProfile => 
            new VendorProfileRepository(_repositoryContext);
    }
}
