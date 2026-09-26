namespace Afrimine.Model.Entities
{
    public class DailyCheck : BaseEntity
    {
        public Guid BookingId { get; set; }
        public Booking Booking { get; set; } = null!;
        public DateTime CheckDate { get; set; } = DateTime.UtcNow;
        public bool EngineOilChecked { get; set; }
        public bool HydraulicFluidChecked { get; set; }
        public bool CoolingSystemChecked { get; set; }
        public bool UndercarriageChecked { get; set; }
        public bool GreaseChecked { get; set; }
        public string? Notes { get; set; }
        public string CheckedByOperatorId { get; set; } = string.Empty;
    }
}
