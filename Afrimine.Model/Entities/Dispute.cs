using Afrimine.Model.Enums;

namespace Afrimine.Model.Entities
{
    public class Dispute : BaseEntity
    {
        public Guid OrderId { get; set; }
        public Order Order { get; set; } = null!;
        public string RaisedById { get; set; } = string.Empty;
        public User RaisedBy { get; set; } = null!;
        public string Reason { get; set; } = string.Empty;
        public DisputeStatus Status { get; set; } = DisputeStatus.Open;
        public string? AdminNote { get; set; }
        public string? ResolvedById { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }
}
