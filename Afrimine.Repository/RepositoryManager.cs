using Afrimine.Migrations;

namespace Afrimine.Repository
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly AppDbContext _repositoryContext;
        private readonly Lazy<IOtpRepository> _otpRepository;
        private readonly Lazy<ISendEmailRepository> _sendEmailRepository;
        private readonly Lazy<IListingRepository> _listingRepository;
        private readonly Lazy<ISavedListingRepository> _savedListingRepository;
        private readonly Lazy<INotificationRepository> _notificationRepository;
        private readonly Lazy<IOrderRepository> _orderRepository;
        private readonly Lazy<IListingImageRepository> _listingImageRepository;

        public RepositoryManager(AppDbContext appDbContext)
        {
            _repositoryContext = appDbContext;
            _otpRepository = new Lazy<IOtpRepository>(() => new OtpRepository(appDbContext));
            _sendEmailRepository = new Lazy<ISendEmailRepository>(() => new SendEmailRepository(appDbContext));
            _listingRepository = new Lazy<IListingRepository>(() => new ListingRepository(appDbContext));
            _savedListingRepository = new Lazy<ISavedListingRepository>(() => new SavedListingRepository(appDbContext));
            _notificationRepository = new Lazy<INotificationRepository>(() => new NotificationRepository(appDbContext));
            _orderRepository = new Lazy<IOrderRepository>(() => new OrderRepository(appDbContext));
            _listingImageRepository = new Lazy<IListingImageRepository>(() => new ListingImageRepository(appDbContext));
        }

        public IOtpRepository Otp => _otpRepository.Value;
        public ISendEmailRepository SendEmail => _sendEmailRepository.Value;
        public async Task SaveAsync() => await _repositoryContext.SaveChangesAsync();
        public IVendorProfileRepository VendorProfile => 
            new VendorProfileRepository(_repositoryContext);
        public IListingRepository Listing => _listingRepository.Value;
        public ISavedListingRepository SavedListing => _savedListingRepository.Value;
        public INotificationRepository Notification => _notificationRepository.Value;
        public IOrderRepository Order => _orderRepository.Value;
        public IListingImageRepository ListingImage => _listingImageRepository.Value;
    }
}
