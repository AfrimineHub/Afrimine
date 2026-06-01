using Afrimine.Model.Enums;

namespace Afrimine.Model.Entities
{
    public class Order : BaseEntity
    {
        public string BuyerId { get; set; } = string.Empty;
        public User Buyer { get; set; } = null!;
        public Guid ListingId { get; set; }
        public Listing Listing { get; set; } = null!;
        public OrderStatus Status { get; set; } = OrderStatus.Ongoing;
    }
}
