namespace Afrimine.Model.Entities
{
    public class AssetOperator : BaseEntity
    {
        public Guid AssetId { get; set; }
        public Asset Asset { get; set; } = null!;
        public Guid OperatorId { get; set; }
        public Operator Operator { get; set; } = null!;
        public bool IsPrimary { get; set; } = false;
    }
}
