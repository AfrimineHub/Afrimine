namespace Afrimine.Model.Enums
{
    public enum SupplierStatus
    {
        Pending,
        Active,
        Rejected,
        Suspended
    }

    public enum AssetStatus
    {
        Available,
        Rented,
        UnderMaintenance,
        Inactive
    }

    public enum MachineType
    {
        Excavator,
        Bulldozer,
        Payloader,
        Tipper,
        Grader,
        Crane,
        Compactor
    }

    public enum BookingStatus
    {
        Pending,
        Approved,
        Declined,
        Active,
        Completed,
        Disputed,
        Cancelled
    }

    public enum LogisticsStatus
    {
        NotStarted,
        Dispatched,
        EnRoute,
        Arrived,
        Returned
    }

    public enum MilestoneStatus
    {
        Locked,
        Pending,
        Released
    }

    public enum VettingStatus
    {
        NotStarted,
        Submitted,
        Passed,
        Failed
    }

    public enum PayscrowTransactionStatus
    {
        Pending,
        InProgress,
        Completed,
        Finalized,
        Terminated
    }
}
