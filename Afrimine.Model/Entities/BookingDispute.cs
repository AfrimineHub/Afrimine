using Afrimine.Model.Enums;

namespace Afrimine.Model.Entities
{
    public class BookingDispute : BaseEntity
    {
        public Guid BookingId { get; set; }
        public Booking Booking { get; set; } = null!;
        public string RaisedById { get; set; } = string.Empty;
        public User RaisedBy { get; set; } = null!;
        public string RaisedByRole { get; set; } = string.Empty; // "miner" | "supplier"
        public string Description { get; set; } = string.Empty;
        public DisputeStatus Status { get; set; } = DisputeStatus.Open;
        public string? Resolution { get; set; }
        public string? PayscrowDisputeRef { get; set; }
    }
}
