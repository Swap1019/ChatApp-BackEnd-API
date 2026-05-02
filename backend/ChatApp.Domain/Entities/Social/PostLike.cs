using ChatApp.Domain.Entities.Identity;

namespace ChatApp.Domain.Entities.Social
{
    public class PostLike
    {
        public Guid PostId { get; set; }
        public Post Post { get; set; } = null!;

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}