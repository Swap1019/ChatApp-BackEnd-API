using ChatApp.Domain.Entities.Messaging;

namespace ChatApp.Domain.Entities.Media
{
    public class MessageAttachment
    {
        public Guid MessageId { get; set; }
        public Message Message { get; set; } = null!;

        public Guid MediaId { get; set; }
        public Media Media { get; set; } = null!;

        public int SortOrder { get; set; }
    }
}