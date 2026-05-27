using Afrimine.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Afrimine.Services.DTOs
{
    public class BusinessProfileDto
    {
        public BusinessType BusinessType { get; set; }
        public string Country { get; set; } = string.Empty;
        public string StateOrRegion { get; set; } = string.Empty;
        public string OfficeAddress { get; set; } = string.Empty;
        public string? Website { get; set; }
    }
}
