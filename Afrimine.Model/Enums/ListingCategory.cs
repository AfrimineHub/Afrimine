using System.ComponentModel;

namespace Afrimine.Model.Enums
{
    public enum ListingCategory
    {
        [Description("Mining Site")]
        MiningSite = 1,

        [Description("Mineral Supply")]
        MineralSupply = 2,

        [Description("Equipment")]
        Equipment = 3,

        [Description("Investment")]
        Investment = 4
    }
}
