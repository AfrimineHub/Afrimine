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


        public ServiceManager(UserManager<User> userManager,
                            SignInManager<User> signInManager,
                            IOptions<AppConfig> options,
                            IRepositoryManager repositoryManager)
        {
            _userService = new Lazy<IUserService>(() => new UserService(userManager, signInManager, options, repositoryManager));
            _dashboardService = new Lazy<IDashboardService>(() => new DashboardService(repositoryManager));
            _vendorListingService = new Lazy<IVendorListingService>(() => new VendorListingService(repositoryManager));
            _vendorListingService = new Lazy<IVendorListingService>(() => new VendorListingService(repositoryManager));
        }

        public IUserService User => _userService.Value;
        public IDashboardService Dashboard => _dashboardService.Value;
        public IVendorListingService VendorListing => _vendorListingService.Value;
    }
}