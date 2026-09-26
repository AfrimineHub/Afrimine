using Afrimine.Services.BL.Interfaces;
using Afrimine.Shared.Configs;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Afrimine.Services.BL.Implementation
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        private static readonly HashSet<string> AllowedImageTypes = new()
        {
            "image/jpeg", "image/jpg", "image/png", "image/webp", "image/gif"
        };

        private static readonly HashSet<string> AllowedDocTypes = new()
        {
            "application/pdf",
            "image/jpeg", "image/jpg", "image/png",
            "application/msword",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
        };

        private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10MB

        public CloudinaryService(IOptions<CloudinaryConfig> config)
        {
            var account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret);

            _cloudinary = new Cloudinary(account);
            _cloudinary.Api.Secure = true;
        }

        public async Task<CloudinaryUploadResult> UploadImageAsync(IFormFile file, string folder)
        {
            var validation = ValidateFile(file, AllowedImageTypes);
            if (!validation.Success) return validation;

            await using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folder,
                UseFilename = false,
                UniqueFilename = true,
                Overwrite = false,
                Transformation = new Transformation()
                    .Quality("auto")
                    .FetchFormat("auto")
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error is not null)
                return new CloudinaryUploadResult { Success = false, Error = result.Error.Message };

            return new CloudinaryUploadResult
            {
                Success = true,
                Url = result.SecureUrl.ToString(),
                PublicId = result.PublicId,
                Format = result.Format,
                Bytes = result.Bytes
            };
        }

        public async Task<CloudinaryUploadResult> UploadDocumentAsync(IFormFile file, string folder)
        {
            var validation = ValidateFile(file, AllowedDocTypes);
            if (!validation.Success) return validation;

            await using var stream = file.OpenReadStream();

            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folder,
                UseFilename = true,
                UniqueFilename = true,
                Overwrite = false
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error is not null)
                return new CloudinaryUploadResult { Success = false, Error = result.Error.Message };

            return new CloudinaryUploadResult
            {
                Success = true,
                Url = result.SecureUrl.ToString(),
                PublicId = result.PublicId,
                Format = result.Format ?? Path.GetExtension(file.FileName).TrimStart('.'),
                Bytes = result.Bytes
            };
        }

        public async Task<bool> DeleteAsync(string publicId)
        {
            var result = await _cloudinary.DestroyAsync(new DeletionParams(publicId));
            return result.Result == "ok";
        }

        private static CloudinaryUploadResult ValidateFile(IFormFile file, HashSet<string> allowedTypes)
        {
            if (file.Length == 0)
                return new CloudinaryUploadResult { Success = false, Error = "File is empty." };

            if (file.Length > MaxFileSizeBytes)
                return new CloudinaryUploadResult { Success = false, Error = "File exceeds 10MB limit." };

            if (!allowedTypes.Contains(file.ContentType.ToLower()))
                return new CloudinaryUploadResult
                {
                    Success = false,
                    Error = $"File type '{file.ContentType}' is not allowed."
                };

            return new CloudinaryUploadResult { Success = true };
        }
    }
}
