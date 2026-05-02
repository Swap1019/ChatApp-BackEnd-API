using ChatApp.Domain.Entities.Identity;

namespace ChatApp.Domain.Entities.Conversation
{
    public class ConversationUser
    {
        public Guid ConversationId { get; set; }
        public Conversation Conversation { get; set; } = null!;    
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public bool IsAdmin { get; set; } = false;
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        // Track the last seen message sequence for this user in the conversation
        public long LastSeenMessageSequence { get; set; } = 0;
        // Optionally track when the user last saw the conversation for presence/status purposes
        public DateTime? LastSeenAt { get; set; }
        
        // One-to-one relationship with detailed admin permissions (if user is admin)
        public ConversationUserAdmin? AdminPermissions { get; set; }
    }
}