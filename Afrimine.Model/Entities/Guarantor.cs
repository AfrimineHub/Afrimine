namespace Afrimine.Model.Entities
{
    public class Guarantor : BaseEntity
    {
        public Guid OperatorId { get; set; }
        public Operator Operator { get; set; } = null!;
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Occupation { get; set; } = string.Empty;
        public string IdType { get; set; } = string.Empty;
        public string IdNumber { get; set; } = string.Empty;
    }
}
