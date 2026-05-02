using MediaEntities = ChatApp.Domain.Entities.Media;

namespace ChatApp.Domain.Entities.Messaging
{
    public class Sticker
    {
        public Guid Id { get; set; }

        public Guid StickerPackId { get; set; }
        public StickerPack StickerPack { get; set; } = null!;

        public Guid MediaId { get; set; } // IMPORTANT (don’t store URL directly)
        public MediaEntities.Media Media { get; set; } = null!;

        public string? Emoji { get; set; } // 🔥 used for suggestions

        public int Order { get; set; } // position inside pack

        public bool IsAnimated { get; set; }

        public bool IsActive { get; set; } = true; // moderation

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}