using Afrimine.Model.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Afrimine.Services.DTOs
{
    public class VendorListingQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public ListingStatus? Status { get; set; }
        public string? Search { get; set; }
        public ListingCategory? Category { get; set; }
    }

    public class PagedResultDto<T>
    {
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }

    public class ListingImageDto
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
    }

    public class VendorListingListDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? PrimaryImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int ViewsCount { get; set; }
        public int InquiriesCount { get; set; }
    }

    public class VendorListingDetailDto : VendorListingListDto
    {
        public string Description { get; set; } = string.Empty;
        public string? PriceDescription { get; set; }
        public string? ContactInfo { get; set; }
        public string? AdminReviewNote { get; set; }
        public DateTime? PublishedAt { get; set; }
        public List<ListingImageDto> Images { get; set; } = new();
    }

    public class CreateListingDto
    {
        [Required] public string Title { get; set; } = string.Empty;
        [Required] public string Description { get; set; } = string.Empty;
        [Required] public string Location { get; set; } = string.Empty;
        [Required] public string Country { get; set; } = string.Empty;
        public string? PriceDescription { get; set; }
        public string? ContactInfo { get; set; }
        [Required] public ListingCategory CategoryType { get; set; }

        public List<IFormFile>? Images { get; set; }

        public string StateOrRegion { get; set; } = string.Empty;

        public decimal PriceAmount { get; set; }

        public string PriceCurrency { get; set; } = string.Empty;

        public string PriceUnit { get; set; } = string.Empty;

        public string? Quantity { get; set; }

        public string? MineralType { get; set; }

        public string? GradeOrPurity { get; set; }

        public string? EquipmentType { get; set; }

        public int? YearManufactured { get; set; }

        public string? Condition { get; set; }

        public decimal? AcreageHectares { get; set; }

        public string? LeaseType { get; set; }

        public string? ManpowerRole { get; set; }

        public string? Availability { get; set; }

        public bool? Publish { get; set; }
    }

    public class UpdateListingDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? Country { get; set; }
        public string? PriceDescription { get; set; }
        public string? ContactInfo { get; set; }
        public ListingCategory? Category { get; set; }
    }

    public class UploadListingImagesDto
    {
        [Required]
        public List<IFormFile> Images { get; set; } = new();
    }
}
