namespace ChatApp.Domain.Entities
{
    public class ConversationUser
    {
        public Guid ConversationId { get; set; }
        public Guid UserId { get; set; }
        public bool IsAdmin { get; set; } = false;
        public Conversation Conversation { get; set; } = null!;        
        public User User { get; set; } = null!;
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        // Track the last seen message sequence for this user in the conversation
        public long LastSeenMessageSequence { get; set; } = 0;
        // Optionally track when the user last saw the conversation for presence/status purposes
        public DateTime? LastSeenAt { get; set; }
        
        // Collection for detailed admin permissions
        public ICollection<ConversationUserAdmin> AdminPermissions { get; set; } = new List<ConversationUserAdmin>();
    }
}