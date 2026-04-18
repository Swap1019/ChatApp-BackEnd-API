namespace ChatApp.Domain.Entities
{
    public class ConversationUserBan
    {
        // One record per user ban in a conversation. Allows tracking who banned the user and when.
        public Guid ConversationId { get; set; }
        public Conversation Conversation { get; set; } = null!;

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Guid BannedByUserId { get; set; }
        public User BannedByUser { get; set; } = null!;

        public DateTime BannedAt { get; set; } = DateTime.UtcNow;
        public string? Reason { get; set; }
        public DateTime? ExpiresAt { get; set; } // optional expiration for temporary bans
    }
}