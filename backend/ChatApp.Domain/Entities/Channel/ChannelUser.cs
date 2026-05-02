using ChatApp.Domain.Entities.Identity;

namespace ChatApp.Domain.Entities.Channel
{
    public class ChannelUser
    {
        public Guid ChannelId { get; set; }
        public Channel Channel { get; set; } = null!;

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        // One-to-one relationship with detailed admin permissions (if user is admin)
        public ChannelUserAdmin? AdminPermissions { get; set; }
    }
}