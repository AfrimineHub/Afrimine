namespace Afrimine.Model.Entities
{
    public class Message : BaseEntity
    {
        public Guid ConversationId { get; set; }
        public Conversation Conversation { get; set; } = null!;
        public string SenderId { get; set; } = string.Empty;
        public User Sender { get; set; } = null!;
        public string Content { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;
    }
}
