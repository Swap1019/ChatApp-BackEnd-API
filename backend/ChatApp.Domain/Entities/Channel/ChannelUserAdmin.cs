using ChatApp.Domain.Entities.Identity;

namespace ChatApp.Domain.Entities.Channel
{
    public class ChannelUserAdmin
    {
        // Primary key - one admin role per ChannelUser
        public Guid ChannelUserId { get; set; }
        public ChannelUser ChannelUser { get; set; } = null!;

        // Admin permissions
        public bool CanKickMembers { get; set; } = false;
        public bool CanPinMessages { get; set; } = false;
        public bool CanDeleteMessages { get; set; } = false;
        public bool CanUpdateChannel { get; set; } = false;
        public bool CanManageRoles { get; set; } = false;
        public bool CanManageCalls { get; set; } = false;
        public bool CanAddAdmins { get; set; } = false;
        public bool SignName { get; set; } = false;

        public DateTime GrantedAt { get; set; } = DateTime.UtcNow;
        public Guid GrantedByUserId { get; set; }
        public User GrantedByUser { get; set; } = null!;
    }
}