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
        ISubscriptionRepository Subscription { get; }
        IRevenueRepository Revenue { get; }
        IQuoteRepository Quote { get; }
        IPayoutRepository Payout { get; }
        IRfqRepository Rfq { get; }
        IMarketTrendRepository MarketTrend { get; }
        IInvestmentInsightRepository InvestmentInsight { get; }
        IInquiryRepository Inquiry { get; }
        IConversationRepository Conversation { get; }
        IMessageRepository MessageRepo { get; }
        IEscrowRepository Escrow { get; }
        IDisputeRepository Dispute { get; }
        ISubscriptionPlanRepository SubscriptionPlan { get; }
        ISubscriptionInvoiceRepository SubscriptionInvoice { get; }
        IRfqQuoteRepository RfqQuote { get; }
    }
}
