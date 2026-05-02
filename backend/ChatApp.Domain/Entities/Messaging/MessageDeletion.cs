using ChatApp.Domain.Enums;

namespace ChatApp.Domain.Entities.Messaging
{
    public class MessageDeletion
    {
        public Guid MessageId { get; set; }
        public Guid UserId { get; set; }

        public Guid DeletedByUserId { get; set; }
        public MessageDeletionScope Scope { get; set; }
        public DateTime DeletedAt { get; set; } = DateTime.UtcNow;
    }
}