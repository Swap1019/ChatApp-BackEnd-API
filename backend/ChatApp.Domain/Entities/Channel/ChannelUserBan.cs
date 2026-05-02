using ChatApp.Domain.Entities.Identity;

namespace ChatApp.Domain.Entities.Channel
{
    public class ChannelUserBan
    {

        public Guid ChannelId { get; set; }
        public Channel Channel { get; set; } = null!;

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Guid BannedByUserId { get; set; }
        public User BannedByUser { get; set; } = null!;

        public DateTime BannedAt { get; set; } = DateTime.UtcNow;
    } 
}