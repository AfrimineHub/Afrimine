using Microsoft.AspNetCore.Http;

namespace Afrimine.Services.BL.Interfaces
{
    public interface ICloudinaryService
    {
        Task<CloudinaryUploadResult> UploadImageAsync(IFormFile file, string folder);
        Task<CloudinaryUploadResult> UploadDocumentAsync(IFormFile file, string folder);
        Task<bool> DeleteAsync(string publicId);
    }

    public class CloudinaryUploadResult
    {
        public string Url { get; set; } = string.Empty;
        public string PublicId { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty;
        public long Bytes { get; set; }
        public bool Success { get; set; }
        public string? Error { get; set; }
    }
}
