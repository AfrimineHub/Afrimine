using Afrimine.Model.Entities;
using Afrimine.Model.Enums;
using Afrimine.Repository;
using Afrimine.Services.BL.Interfaces;
using Afrimine.Services.DTOs;
using Afrimine.Services.Responses;
using Microsoft.AspNetCore.Http;

namespace Afrimine.Services.BL.Implementation
{
    public class VendorListingService : IVendorListingService
    {
        private readonly IRepositoryManager _repository;

        // Max 10 images per listing
        private const int MaxImages = 10;

        public VendorListingService(IRepositoryManager repository)
        {
            _repository = repository;
        }

        public async Task<ApiResponse<PagedResultDto<VendorListingListDto>>> GetListingsAsync(
           string vendorId, VendorListingQueryDto query)
        {
            var (items, total) = await _repository.Listing.GetVendorListingsAsync(
                vendorId, query.Page, query.PageSize,
                query.Status, query.Search, query.Category);

            var result = new PagedResultDto<VendorListingListDto>
            {
                Items = items.Select(MapToListDto),
                TotalCount = total,
                Page = query.Page,
                PageSize = query.PageSize
            };

            return ApiResponse<PagedResultDto<VendorListingListDto>>.Ok(result);
        }

        public async Task<ApiResponse<VendorListingDetailDto>> GetListingByIdAsync(string vendorId, Guid listingId)
        {
            var listing = await _repository.Listing.GetByIdWithImagesAsync(listingId);

            if (listing is null)
                return ApiResponse<VendorListingDetailDto>.Fail("Listing not found.", 404);

            if (listing.OwnerId != vendorId)
                return ApiResponse<VendorListingDetailDto>.Fail("Access denied.", 403);

            return ApiResponse<VendorListingDetailDto>.Ok(MapToDetailDto(listing));
        }

        public async Task<ApiResponse<VendorListingDetailDto>> CreateListingAsync(
            string vendorId, CreateListingDto request)
        {
            var listing = new Listing
            {
                Title = request.Title,
                Description = request.Description,
                Location = request.Location,
                Country = request.Country,
                PriceDescription = request.PriceDescription,
                ContactInfo = request.ContactInfo,
                CategoryType = request.CategoryType,
                OwnerId = vendorId,
                StateOrRegion = request.StateOrRegion,
                PriceAmount = request.PriceAmount,
                PriceCurrency = request.PriceCurrency,
                PriceUnit = request.PriceUnit,
                Quantity = request.Quantity,
                MineralType = request.MineralType,
                GradeOrPurity = request.GradeOrPurity,
                EquipmentType = request.EquipmentType,
                YearManufactured = request.YearManufactured,
                Condition = request.Condition,
                AcreageHectares = request.AcreageHectares,
                LeaseType = request.LeaseType,
                ManpowerRole = request.ManpowerRole,
                Availability = request.Availability,
                Status = request.Publish == true ? ListingStatus.Published : ListingStatus.Draft
            };

            await _repository.Listing.Create(listing);

            // Handle optional images on creation
            if (request.Images is { Count: > 0 })
            {
                var images = await SaveImagesAsync(listing.Id, request.Images);
                if (images.Any())
                    images.First().IsPrimary = true;

                foreach (var img in images)
                    await _repository.ListingImage.Create(img);
            }

            await _repository.SaveAsync();

            var created = await _repository.Listing.GetByIdWithImagesAsync(listing.Id);
            return ApiResponse<VendorListingDetailDto>.Ok(MapToDetailDto(created!), 201, "Listing created.");
        }

        public async Task<ApiResponse<VendorListingDetailDto>> UpdateListingAsync(
            string vendorId, Guid listingId, UpdateListingDto request)
        {
            var listing = await _repository.Listing.GetByIdWithImagesAsync(listingId);

            if (listing is null)
                return ApiResponse<VendorListingDetailDto>.Fail("Listing not found.", 404);

            if (listing.OwnerId != vendorId)
                return ApiResponse<VendorListingDetailDto>.Fail("Access denied.", 403);

            if (listing.Status == ListingStatus.PendingReview)
                return ApiResponse<VendorListingDetailDto>.Fail("Cannot edit a listing that is pending review.", 409);

            // Patch — only update provided fields
            if (request.Title is not null) listing.Title = request.Title;
            if (request.Description is not null) listing.Description = request.Description;
            if (request.Location is not null) listing.Location = request.Location;
            if (request.Country is not null) listing.Country = request.Country;
            if (request.PriceDescription is not null) listing.PriceDescription = request.PriceDescription;
            if (request.ContactInfo is not null) listing.ContactInfo = request.ContactInfo;
            if (request.Category.HasValue) listing.CategoryType = request.Category.Value;
            listing.UpdatedAt = DateTime.UtcNow;

            _repository.Listing.Update(listing);
            await _repository.SaveAsync();

            return ApiResponse<VendorListingDetailDto>.Ok(MapToDetailDto(listing));
        }

        public async Task<ApiResponse<string>> DeleteListingAsync(string vendorId, Guid listingId)
        {
            var listing = await _repository.Listing.GetByIdAsync(listingId);

            if (listing is null)
                return ApiResponse<string>.Fail("Listing not found.", 404);

            if (listing.OwnerId != vendorId)
                return ApiResponse<string>.Fail("Access denied.", 403);

            // Soft delete / archive
            listing.IsDeleted = true;
            listing.DeletedBy = vendorId;
            listing.Status = ListingStatus.Archived;
            listing.UpdatedAt = DateTime.UtcNow;

            _repository.Listing.Update(listing);
            await _repository.SaveAsync();

            return ApiResponse<string>.Ok("Listing archived successfully.");
        }

        public async Task<ApiResponse<List<ListingImageDto>>> UploadImagesAsync(
            string vendorId, Guid listingId, UploadListingImagesDto request)
        {
            var listing = await _repository.Listing.GetByIdWithImagesAsync(listingId);

            if (listing is null)
                return ApiResponse<List<ListingImageDto>>.Fail("Listing not found.", 404);

            if (listing.OwnerId != vendorId)
                return ApiResponse<List<ListingImageDto>>.Fail("Access denied.", 403);

            var currentCount = await _repository.ListingImage.CountByListingAsync(listingId);
            if (currentCount + request.Images.Count > MaxImages)
                return ApiResponse<List<ListingImageDto>>.Fail(
                    $"Cannot exceed {MaxImages} images per listing. Current: {currentCount}.", 400);

            var hasPrimary = listing.Images.Any(x => x.IsPrimary && !x.IsDeleted);
            var newImages = await SaveImagesAsync(listingId, request.Images);

            // Make first image primary if none exists
            if (!hasPrimary && newImages.Any())
                newImages.First().IsPrimary = true;

            foreach (var img in newImages)
                await _repository.ListingImage.Create(img);

            await _repository.SaveAsync();

            return ApiResponse<List<ListingImageDto>>.Ok(newImages.Select(MapToImageDto).ToList());
        }

        public async Task<ApiResponse<string>> DeleteImageAsync(string vendorId, Guid listingId, Guid imageId)
        {
            var listing = await _repository.Listing.GetByIdAsync(listingId);

            if (listing is null)
                return ApiResponse<string>.Fail("Listing not found.", 404);

            if (listing.OwnerId != vendorId)
                return ApiResponse<string>.Fail("Access denied.", 403);

            var image = await _repository.ListingImage.GetByIdAndListingAsync(imageId, listingId);
            if (image is null)
                return ApiResponse<string>.Fail("Image not found.", 404);

            image.IsDeleted = true;
            image.DeletedBy = vendorId;
            _repository.ListingImage.Update(image);
            await _repository.SaveAsync();

            return ApiResponse<string>.Ok("Image removed.");
        }

        public async Task<ApiResponse<string>> PublishListingAsync(string vendorId, Guid listingId)
        {
            var listing = await _repository.Listing.GetByIdAsync(listingId);

            if (listing is null)
                return ApiResponse<string>.Fail("Listing not found.", 404);

            if (listing.OwnerId != vendorId)
                return ApiResponse<string>.Fail("Access denied.", 403);

            if (listing.Status == ListingStatus.PendingReview)
                return ApiResponse<string>.Fail("Listing is already submitted for review.", 409);

            if (listing.Status == ListingStatus.Active)
                return ApiResponse<string>.Fail("Listing is already active.", 409);

            listing.Status = ListingStatus.PendingReview;
            listing.UpdatedAt = DateTime.UtcNow;

            _repository.Listing.Update(listing);
            await _repository.SaveAsync();

            return ApiResponse<string>.Ok("Listing submitted for admin review.");
        }

        // ── Private helpers ───────────────────────────────────────────────────

        private static async Task<List<ListingImage>> SaveImagesAsync(Guid listingId, List<IFormFile> files)
        {
            // TODO: Replace this with your actual cloud storage upload (e.g. Azure Blob / S3 / Cloudinary)
            // For now it stores the file name as the URL placeholder
            var images = new List<ListingImage>();

            foreach (var file in files)
            {
                if (file.Length == 0) continue;

                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";

                // ↓ Swap this line with your actual upload call and use the returned URL
                var imageUrl = $"/uploads/listings/{listingId}/{fileName}";

                images.Add(new ListingImage
                {
                    ListingId = listingId,
                    ImageUrl = imageUrl,
                    FileName = fileName
                });
            }

            return await Task.FromResult(images);
        }

        private static VendorListingListDto MapToListDto(Listing l) => new()
        {
            Id = l.Id,
            Title = l.Title,
            Location = l.Location,
            Country = l.Country,
            Category = l.CategoryType.ToString(),
            Status = l.Status.ToString(),
            PrimaryImageUrl = l.Images.FirstOrDefault(x => x.IsPrimary && !x.IsDeleted)?.ImageUrl
                              ?? l.Images.FirstOrDefault(x => !x.IsDeleted)?.ImageUrl,
            CreatedAt = l.CreatedAt,
            UpdatedAt = l.UpdatedAt
        };

        private static VendorListingDetailDto MapToDetailDto(Listing l) => new()
        {
            Id = l.Id,
            Title = l.Title,
            Description = l.Description,
            Location = l.Location,
            Country = l.Country,
            Category = l.CategoryType.ToString(),
            Status = l.Status.ToString(),
            PriceDescription = l.PriceDescription,
            ContactInfo = l.ContactInfo,
            AdminReviewNote = l.AdminReviewNote,
            PublishedAt = l.PublishedAt,
            PrimaryImageUrl = l.Images.FirstOrDefault(x => x.IsPrimary && !x.IsDeleted)?.ImageUrl
                              ?? l.Images.FirstOrDefault(x => !x.IsDeleted)?.ImageUrl,
            CreatedAt = l.CreatedAt,
            UpdatedAt = l.UpdatedAt,
            Images = l.Images.Where(x => !x.IsDeleted).Select(MapToImageDto).ToList()
        };

        private static ListingImageDto MapToImageDto(ListingImage img) => new()
        {
            Id = img.Id,
            ImageUrl = img.ImageUrl,
            FileName = img.FileName,
            IsPrimary = img.IsPrimary
        };
    }
}
