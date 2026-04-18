namespace ChatApp.Domain.Entities
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