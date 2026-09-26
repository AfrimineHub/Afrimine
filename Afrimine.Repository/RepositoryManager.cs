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
        private readonly Lazy<ISubscriptionRepository> _subscriptionRepository;
        private readonly Lazy<IRevenueRepository> _revenueRepository;
        private readonly Lazy<IQuoteRepository> _quoteRepository;
        private readonly Lazy<IPayoutRepository> _payoutRepository;
        private readonly Lazy<IRfqRepository> _rfqRepository;
        private readonly Lazy<IMarketTrendRepository> _marketTrendRepository;
        private readonly Lazy<IInvestmentInsightRepository> _investmentInsightRepository;
        private readonly Lazy<IInquiryRepository> _inquiryRepository;
        private readonly Lazy<IConversationRepository> _conversationRepository;
        private readonly Lazy<IMessageRepository> _messageRepository;
        private readonly Lazy<IEscrowRepository> _escrowRepository;
        private readonly Lazy<IDisputeRepository> _disputeRepository;
        private readonly Lazy<ISubscriptionPlanRepository> _subscriptionPlanRepository;
        private readonly Lazy<ISubscriptionInvoiceRepository> _subscriptionInvoiceRepository;
        private readonly Lazy<IRfqQuoteRepository> _rfqQuoteRepository;

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
            _subscriptionRepository = new Lazy<ISubscriptionRepository>(() => new SubscriptionRepository(appDbContext));
            _revenueRepository = new Lazy<IRevenueRepository>(() => new RevenueRepository(appDbContext));
            _quoteRepository = new Lazy<IQuoteRepository>(() => new QuoteRepository(appDbContext));
            _payoutRepository = new Lazy<IPayoutRepository>(() => new PayoutRepository(appDbContext));
            _rfqRepository = new Lazy<IRfqRepository>(() => new RfqRepository(appDbContext));
            _marketTrendRepository = new Lazy<IMarketTrendRepository>(() => new MarketTrendRepository(appDbContext));
            _investmentInsightRepository = new Lazy<IInvestmentInsightRepository>(() => new InvestmentInsightRepository(appDbContext));
            _inquiryRepository = new Lazy<IInquiryRepository>(() => new InquiryRepository(appDbContext));
            _conversationRepository = new Lazy<IConversationRepository>(() => new ConversationRepository(appDbContext));
            _messageRepository = new Lazy<IMessageRepository>(() => new MessageRepository(appDbContext));
            _escrowRepository = new Lazy<IEscrowRepository>(() => new EscrowRepository(appDbContext));
            _disputeRepository = new Lazy<IDisputeRepository>(() => new DisputeRepository(appDbContext));
            _subscriptionPlanRepository = new Lazy<ISubscriptionPlanRepository>(() => new SubscriptionPlanRepository(appDbContext));
            _subscriptionInvoiceRepository = new Lazy<ISubscriptionInvoiceRepository>(() => new SubscriptionInvoiceRepository(appDbContext));
            _rfqQuoteRepository = new Lazy<IRfqQuoteRepository>(() => new RfqQuoteRepository(appDbContext));
        }

        public IOtpRepository Otp => _otpRepository.Value;
        public ISendEmailRepository SendEmail => _sendEmailRepository.Value;
        public async Task SaveAsync() => await _repositoryContext.SaveChangesAsync();
        public ISupplierProfileRepository Profile =>
            new SupplierProfileRepository(_repositoryContext);
        public IListingRepository Listing => _listingRepository.Value;
        public ISavedListingRepository SavedListing => _savedListingRepository.Value;
        public INotificationRepository Notification => _notificationRepository.Value;
        public IOrderRepository Order => _orderRepository.Value;
        public IListingImageRepository ListingImage => _listingImageRepository.Value;
        public IRevenueRepository Revenue => _revenueRepository.Value;
        public ISubscriptionRepository Subscription => _subscriptionRepository.Value;
        public IQuoteRepository Quote => _quoteRepository.Value;
        public IPayoutRepository Payout => _payoutRepository.Value;
        public IRfqRepository Rfq => _rfqRepository.Value;
        public IMarketTrendRepository MarketTrend => _marketTrendRepository.Value;
        public IInvestmentInsightRepository InvestmentInsight => _investmentInsightRepository.Value;
        public IInquiryRepository Inquiry => _inquiryRepository.Value;
        public IConversationRepository Conversation => _conversationRepository.Value;
        public IMessageRepository MessageRepo => _messageRepository.Value;
        public IEscrowRepository Escrow => _escrowRepository.Value;
        public IDisputeRepository Dispute => _disputeRepository.Value;
        public ISubscriptionPlanRepository SubscriptionPlan => _subscriptionPlanRepository.Value;
        public ISubscriptionInvoiceRepository SubscriptionInvoice => _subscriptionInvoiceRepository.Value;
        public IRfqQuoteRepository RfqQuote => _rfqQuoteRepository.Value;
    }
}
