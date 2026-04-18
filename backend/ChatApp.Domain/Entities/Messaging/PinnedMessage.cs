namespace ChatApp.Domain.Entities
{
    public class PinnedMessage
    {
        public Guid Id { get; set; }

        public Guid MessageId { get; set; }
        public Message Message { get; set; } = null!;

        public Guid ConversationId { get; set; }
        public Conversation Conversation { get; set; } = null!;

        public Guid PinnedByUserId { get; set; }
        public User PinnedByUser { get; set; } = null!;

        public int Order { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}