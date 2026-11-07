using Afrimine.Model.Enums;
using Microsoft.AspNetCore.Http;

namespace Afrimine.Services.DTOs
{
    public class KycUploadDto
    {
        public DocumentType DocumentType { get; set; }
        public IFormFile File { get; set; } = null!;
    }
}
