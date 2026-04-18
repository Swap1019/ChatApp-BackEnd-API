namespace ChatApp.Domain.Entities
{
    public class ConversationUrl
    {
        public Guid Id { get; private set; }
        public Guid ConversationId { get; set; }
        public Conversation Conversation { get; set; } = null!;

        public string UrlSlug { get; set; } = null!;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? DeactivatedAt { get; set; }
        public string? DeactivatedReason { get; set; }

        public ConversationUrl(Guid conversationId, string urlSlug)
        {
            Id = Guid.NewGuid();
            ConversationId = conversationId;
            UrlSlug = urlSlug;
        }

        public void Deactivate(string? reason = null)
        {
            IsActive = false;
            DeactivatedAt = DateTime.UtcNow;
            DeactivatedReason = reason;
        }
    }
}
