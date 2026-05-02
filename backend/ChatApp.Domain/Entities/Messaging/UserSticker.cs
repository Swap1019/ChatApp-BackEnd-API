using ChatApp.Domain.Entities.Identity;

namespace ChatApp.Domain.Entities.Messaging
{
    public class UserStickerPack
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Guid StickerPackId { get; set; }
        public StickerPack StickerPack { get; set; } = null!;

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}