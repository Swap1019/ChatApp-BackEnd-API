namespace ChatApp.Domain.Entities
{
    public class ConversationUserAdmin
    {
        public Guid Id { get; set; }

        // Foreign key to ConversationUser
        public Guid ConversationUserId { get; set; }
        public ConversationUser ConversationUser { get; set; } = null!;

        // Admin permissions
        public bool CanKickMembers { get; set; } = false;
        public bool CanPinMessages { get; set; } = false;
        public bool CanDeleteMessages { get; set; } = false;
        public bool CanUpdateConversation { get; set; } = false;
        public bool CanManageRoles { get; set; } = false;

        public DateTime GrantedAt { get; set; } = DateTime.UtcNow;
    }
}