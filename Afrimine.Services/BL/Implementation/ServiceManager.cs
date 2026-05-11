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

        public ServiceManager(UserManager<User> userManager,
                            SignInManager<User> signInManager,
                            IOptions<AppConfig> options,
                            IRepositoryManager repositoryManager)
        {
            _userService = new Lazy<IUserService>(() => new UserService(userManager, signInManager, options, repositoryManager));
        }

        public IUserService User => _userService.Value;
    }
}