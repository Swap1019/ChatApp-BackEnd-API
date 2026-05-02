using ChatApp.Domain.Entities.Identity;
using ChatApp.Domain.Entities.Media;

namespace ChatApp.Domain.Entities.Social
{
    public class Post
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public long Sequence { get; set; }

        public string? Caption { get; set; }

        public int ViewsCount { get; set; } = 0;

        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; } = null;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<PostAttachment> PostAttachments { get; set; } = new List<PostAttachment>();
        public ICollection<PostLike> Likes { get; set; } = new List<PostLike>();
    }
}