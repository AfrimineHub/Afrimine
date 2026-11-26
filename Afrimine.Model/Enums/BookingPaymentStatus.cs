namespace Afrimine.Model.Enums
{
    /// <summary>
    /// The buyer-facing payment state of a booking, driven by PayScrow (webhook or status sync)
    /// and by milestone releases / dispute outcomes on our side.
    /// </summary>
    public enum BookingPaymentStatus
    {
        AwaitingPayment = 0,   // payment link generated, buyer has not paid yet
        InEscrow = 1,          // buyer paid, funds held by PayScrow, no milestone released
        PartiallyReleased = 2, // one or two milestones released to the supplier
        Released = 3,          // all milestones released
        Disputed = 4,          // a dispute is open, funds frozen
        RefundPending = 5,     // dispute resolved for the buyer, refund being processed
        Refunded = 6,          // buyer refunded
        Failed = 7             // PayScrow transaction failed / terminated before payment
    }
}
