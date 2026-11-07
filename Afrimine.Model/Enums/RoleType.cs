using System.ComponentModel;

namespace Afrimine.Model.Enums
{
    public enum RoleType
    {
        Vendor = 1,
        Buyer,
        Investor,
        Support,
        SuperAdmin
    }

    public enum AccountStatus
    {
        [Description("Pending")]
        Pending,
        [Description("Active")]
        Active,
        [Description("Suspended")]
        Suspended,
        [Description("Banned")]
        Banned,
        [Description("Deactivated")]
        Deactivated
    }
}
