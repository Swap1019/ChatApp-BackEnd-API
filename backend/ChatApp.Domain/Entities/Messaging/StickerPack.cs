using MediaEntities = ChatApp.Domain.Entities.Media;

namespace ChatApp.Domain.Entities.Messaging
{
    public class StickerPack
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public Guid? CoverMediaId { get; set; }
        public MediaEntities.Media? CoverMedia { get; set; }

        public bool IsOfficial { get; set; } = false;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Sticker> Stickers { get; set; } = new List<Sticker>();
    }
}