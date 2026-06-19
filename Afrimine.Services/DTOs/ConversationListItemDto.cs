using System.ComponentModel.DataAnnotations;

namespace Afrimine.Services.DTOs
{
    public class ConversationListItemDto
    {
        public Guid Id { get; set; }
        public string ParticipantId { get; set; } = string.Empty;
        public string ParticipantName { get; set; } = string.Empty;
        public string? LastMessage { get; set; }
        public DateTime? LastMessageAt { get; set; }
        public int UnreadCount { get; set; }
        public Guid? ListingId { get; set; }
        public string? ListingTitle { get; set; }
    }

    public class MessageDto
    {
        public Guid Id { get; set; }
        public string SenderId { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ConversationContextDto
    {
        public Guid? ListingId { get; set; }
        public string? Title { get; set; }
        public string? Location { get; set; }
        public string? PriceRange { get; set; }
        public string? ImageUrl { get; set; }
        public string? Category { get; set; }
        public Guid? OrderId { get; set; }
        public string? OrderStatus { get; set; }
    }

    public class StartConversationDto
    {
        [Required] public string VendorId { get; set; } = string.Empty;
        public Guid? ListingId { get; set; }
        public Guid? OrderId { get; set; }
        [Required] public string InitialMessage { get; set; } = string.Empty;
    }

    public class SendMessageDto
    {
        [Required] public string Content { get; set; } = string.Empty;
    }
}
