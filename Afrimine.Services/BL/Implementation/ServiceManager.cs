using Afrimine.Model.Entities;
using Afrimine.Repository;
using Afrimine.Services.BL.Interfaces;
using Afrimine.Shared.Configs;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Afrimine.Services.BL.Implementation
{
    public sealed class ServiceManager : IServiceManager
    {
        private readonly Lazy<IUserService> _userService;
        private readonly Lazy<IDashboardService> _dashboardService;
        private readonly Lazy<IVendorListingService> _vendorListingService;
        private readonly Lazy<IBuyerService> _buyerService;
        private readonly Lazy<IMarketService> _marketService;
        private readonly Lazy<IMessagingService> _messagingService;
        private readonly Lazy<ISubscriptionService> _subscriptionService;
        private readonly Lazy<IEscrowService> _escrowService;


        public ServiceManager(UserManager<User> userManager,
                            SignInManager<User> signInManager,
                            IOptions<AppConfig> options,
                            IRepositoryManager repositoryManager)
        {
            _userService = new Lazy<IUserService>(() => new UserService(userManager, signInManager, options, repositoryManager));
            _dashboardService = new Lazy<IDashboardService>(() => new DashboardService(repositoryManager));
            _vendorListingService = new Lazy<IVendorListingService>(() => new VendorListingService(repositoryManager));
            _vendorListingService = new Lazy<IVendorListingService>(() => new VendorListingService(repositoryManager));
            _buyerService = new Lazy<IBuyerService>(() => new BuyerService(repositoryManager));
            _marketService = new Lazy<IMarketService>(() => new MarketService(repositoryManager));
            _messagingService = new Lazy<IMessagingService>(() => new MessagingService(repositoryManager));
            _subscriptionService = new Lazy<ISubscriptionService>(() => new SubscriptionService(repositoryManager));
            _escrowService = new Lazy<IEscrowService>(() => new EscrowService(repositoryManager));
        }

        public IUserService User => _userService.Value;
        public IDashboardService Dashboard => _dashboardService.Value;
        public IVendorListingService VendorListing => _vendorListingService.Value;
        public IBuyerService Buyer => _buyerService.Value;
        public IMarketService Market => _marketService.Value;
        public IMessagingService Messaging => _messagingService.Value;
        public ISubscriptionService Subscription => _subscriptionService.Value;
        public IEscrowService Escrow => _escrowService.Value;
    }
}