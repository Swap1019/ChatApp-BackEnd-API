using ChatApp.Domain.Entities.Identity;
using ChatApp.Domain.Entities.Messaging;

namespace ChatApp.Domain.Entities.Channel
{
    public class ChannelPinnedMessage
    {
        public Guid MessageId { get; set; }
        public Message Message { get; set; } = null!;

        public Guid ChannelId { get; set; }
        public Channel Channel { get; set; } = null!;

        public Guid PinnedByUserId { get; set; }
        public User PinnedByUser { get; set; } = null!;

        public int Order { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}