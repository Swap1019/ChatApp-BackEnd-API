using ChatApp.Domain.Entities.Identity;

namespace ChatApp.Domain.Entities.Messaging
{
    public class UserRecentSticker
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Guid StickerId { get; set; }
        public Sticker Sticker { get; set; } = null!;

        public DateTime LastUsedAt { get; set; } = DateTime.UtcNow;
    }
}