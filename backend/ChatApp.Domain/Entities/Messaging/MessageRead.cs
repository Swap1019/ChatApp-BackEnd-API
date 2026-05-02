using ChatApp.Domain.Entities.Identity;

namespace ChatApp.Domain.Entities.Messaging
{
    public class MessageRead
    {
        public Guid MessageId { get; set; }
        public Message Message { get; set; } = null!;

        public Guid UserId { get; set; }
        public User? User { get; set; }

        public DateTime ReadAt { get; set; }
    }
}