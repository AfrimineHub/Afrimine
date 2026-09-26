using System.ComponentModel;

namespace Afrimine.Model.Enums
{
    public enum BusinessType
    {
        [Description("Individual")]
        Individual = 1,

        [Description("Company")]
        Company = 2,

        [Description("Cooperative")]
        Cooperative = 3,

        [Description("Government Entity")]
        GovernmentEntity = 4
    }
}
