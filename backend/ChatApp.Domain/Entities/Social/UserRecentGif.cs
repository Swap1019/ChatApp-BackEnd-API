using ChatApp.Domain.Entities.Identity;
using ChatApp.Domain.Entities.Media;

namespace ChatApp.Domain.Entities.Social
{
    public class UserRecentGif
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Guid MediaId { get; set; }
        public Media Media { get; set; } = null!;

        // Composite primary key: (UserId, MediaId)

        public DateTime LastUsedAt { get; set; } = DateTime.UtcNow;
        public int UsageCount { get; set; } = 1;

        // Track when this GIF was added to recent list
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
