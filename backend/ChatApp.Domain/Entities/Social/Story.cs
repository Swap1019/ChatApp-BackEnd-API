using ChatApp.Domain.Entities.Identity;
using MediaEntities = ChatApp.Domain.Entities.Media;

namespace ChatApp.Domain.Entities.Social
{
    public class Story
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public long Sequence { get; set; }

        public string? Caption { get; set; }

        public int ViewsCount { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }

        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; } = null;

        // A story can have only one media attachment
        public Guid MediaId { get; set; }
        public MediaEntities.Media Media { get; set; } = null!;
        public ICollection<StorySeen> SeenBy { get; set; } = new List<StorySeen>();
    }
}