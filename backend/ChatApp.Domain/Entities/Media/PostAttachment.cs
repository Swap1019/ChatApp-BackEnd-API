using ChatApp.Domain.Entities.Social;

namespace ChatApp.Domain.Entities.Media
{
    public class PostAttachment
    {
        public Guid PostId { get; set; }
        public Post Post { get; set; } = null!;

        public Guid MediaId { get; set; }
        public Media Media { get; set; } = null!;

        public int SortOrder { get; set; }
    }
}