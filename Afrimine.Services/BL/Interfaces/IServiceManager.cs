namespace Afrimine.Services.BL.Interfaces
{
    public interface IServiceManager
    {
        IUserService User {  get; }
        IDashboardService Dashboard { get; }
        IVendorListingService VendorListing { get; }
        IBuyerService Buyer { get; }
        IMarketService Market { get; }
        IMessagingService Messaging { get; }
        ISubscriptionService Subscription { get; }
        IEscrowService Escrow { get; }
        IAdminService Admin { get; }
        IEquipmentService Equipment { get; }
    }
}
