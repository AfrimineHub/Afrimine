using Microsoft.EntityFrameworkCore;

namespace Afrimine.Repository
{
    public interface IRepositoryManager
    {
        IOtpRepository Otp {  get; }
        ISendEmailRepository SendEmail { get; }

        Task SaveAsync();
        IVendorProfileRepository VendorProfile { get; }

        IListingRepository Listing { get; }
        ISavedListingRepository SavedListing { get; }
        INotificationRepository Notification { get; }
        IOrderRepository Order { get; }
        IListingImageRepository ListingImage { get; }
    }
}
