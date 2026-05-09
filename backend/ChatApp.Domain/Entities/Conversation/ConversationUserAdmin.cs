using ChatApp.Domain.Entities.Identity;

namespace ChatApp.Domain.Entities.Conversation
{
    public class ConversationUserAdmin
    {
        // Primary key - one admin role per ConversationUser
        public Guid ConversationUserId { get; set; }
        public ConversationUser ConversationUser { get; set; } = null!;

        // Admin permissions
        public bool CanAddMembers { get; set; } = false;
        public bool CanKickMembers { get; set; } = false;
        public bool CanPinMessages { get; set; } = false;
        public bool CanDeleteMessages { get; set; } = false;
        public bool CanUpdateConversation { get; set; } = false;
        public bool CanManageRoles { get; set; } = false;
        public bool CanManageCalls { get; set; } = false;
        public bool CanAddAdmins { get; set; } = false;
        public bool IsAnonymousAdmin { get; set; } = false; // If true, this admin's actions are not attributed to a specific user (for system-generated admins)

        public DateTime GrantedAt { get; set; } = DateTime.UtcNow;
        public Guid GrantedByUserId { get; set; }
        public User GrantedByUser { get; set; } = null!;
    }
}