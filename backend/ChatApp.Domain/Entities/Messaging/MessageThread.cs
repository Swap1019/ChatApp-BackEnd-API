namespace ChatApp.Domain.Entities.Messaging
{
    public class MessageThread
    {
        public Guid Id { get; set; }

        // Root message that started the thread
        public Guid RootMessageId { get; set; }
        public Message RootMessage { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Performance fields
        public Guid? LastMessageId { get; set; }
        public Message? LastMessage { get; set; }
        public int ReplyCount { get; set; } = 0;

        // All replies in this thread
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}