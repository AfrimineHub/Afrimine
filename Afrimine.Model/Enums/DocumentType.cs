using System.ComponentModel;

namespace Afrimine.Model.Enums
{
    public enum DocumentType
    {
        [Description("Government ID or Passport")]
        GovernmentIdOrPassport = 1,

        [Description("Company Registration Certificate")]
        CompanyRegistrationCertificate = 2,

        [Description("Mining License")]
        MiningLicense = 3,

        [Description("Export License")]
        ExportLicense = 4,

        [Description("Mineral License")]
        MineralLicense = 5
    }
}
