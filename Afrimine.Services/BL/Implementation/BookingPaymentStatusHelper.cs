using Afrimine.Model.Entities;
using Afrimine.Model.Enums;

namespace Afrimine.Services.BL.Implementation
{
    public static class BookingPaymentStatusHelper
    {
        public static bool IsPaid(BookingPaymentStatus status) => status is
            BookingPaymentStatus.InEscrow or
            BookingPaymentStatus.PartiallyReleased or
            BookingPaymentStatus.Released or
            BookingPaymentStatus.Disputed or
            BookingPaymentStatus.RefundPending or
            BookingPaymentStatus.Refunded;

        public static string Message(BookingPaymentStatus status) => status switch
        {
            BookingPaymentStatus.AwaitingPayment => "Awaiting payment. Complete payment with the payment link to secure this booking.",
            BookingPaymentStatus.InEscrow => "Payment received and held securely in escrow.",
            BookingPaymentStatus.PartiallyReleased => "Payment in escrow. Some milestones have been released to the supplier.",
            BookingPaymentStatus.Released => "All funds have been released to the supplier.",
            BookingPaymentStatus.Disputed => "A dispute is open on this booking. Funds are frozen until it is resolved.",
            BookingPaymentStatus.RefundPending => "The dispute was resolved in the buyer's favour. The refund is being processed.",
            BookingPaymentStatus.Refunded => "Payment has been refunded to the buyer.",
            BookingPaymentStatus.Failed => "Payment failed or the transaction was cancelled.",
            _ => string.Empty
        };

        public static int ReleasedMilestoneCount(Booking b) =>
            (b.Milestone1Status == MilestoneStatus.Released ? 1 : 0)
          + (b.Milestone2Status == MilestoneStatus.Released ? 1 : 0)
          + (b.Milestone3Status == MilestoneStatus.Released ? 1 : 0);

        /// <summary>Sets InEscrow / PartiallyReleased / Released from the milestone state.</summary>
        public static void SetFromMilestones(Booking b)
        {
            var released = ReleasedMilestoneCount(b);
            b.PaymentStatus = released switch
            {
                0 => BookingPaymentStatus.InEscrow,
                3 => BookingPaymentStatus.Released,
                _ => BookingPaymentStatus.PartiallyReleased
            };
            b.PaymentStatusUpdatedAt = DateTime.UtcNow;
        }

        /// <summary>Maps a booking's payment state onto the escrow vocabulary the admin escrow screen uses.</summary>
        public static EscrowStatus ToEscrowStatus(Booking b)
        {
            if (b.FundsFrozen) return EscrowStatus.Frozen;
            return b.PaymentStatus switch
            {
                BookingPaymentStatus.InEscrow or BookingPaymentStatus.PartiallyReleased => EscrowStatus.Funded,
                BookingPaymentStatus.Released => EscrowStatus.Released,
                BookingPaymentStatus.Disputed or BookingPaymentStatus.RefundPending => EscrowStatus.Frozen,
                BookingPaymentStatus.Refunded => EscrowStatus.Refunded,
                _ => EscrowStatus.Pending
            };
        }

        public static string Normalize(string? value) =>
            new string((value ?? string.Empty).Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();
    }
}
