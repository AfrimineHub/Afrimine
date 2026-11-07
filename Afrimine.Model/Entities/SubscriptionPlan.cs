using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Afrimine.Model.Entities
{
    public class SubscriptionPlan : BaseEntity
    {
        public string PlanKey { get; set; } = string.Empty; // free, bronze, silver, gold, diamond, platinum
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal PriceMonthly { get; set; }
        public string Currency { get; set; } = "NGN";
        public int ListingsLimit { get; set; }
        public bool IsPopular { get; set; } = false;
        public string FeaturesJson { get; set; } = "[]";
    }
}
