using ChatApp.Domain.Enums;
using ChatApp.Domain.Entities.Identity;
using ConversationEntities = ChatApp.Domain.Entities.Conversation;
using ChannelEntities = ChatApp.Domain.Entities.Channel;
using MediaEntities = ChatApp.Domain.Entities.Media;

namespace ChatApp.Domain.Entities.Messaging
{
    public class Message
    {
        public Guid Id { get; set; }

        // Foreign key to Conversation
        public Guid? ConversationId { get; set; }
        public ConversationEntities.Conversation? Conversation { get; set; } = null!;

        // Foreign key to Channel (if this message is posted in a channel)
        public Guid? ChannelId { get; set; }
        public ChannelEntities.Channel? Channel { get; set; }

        // Context type for the message
        public MessageContextType? ContextType { get; set; }

        // Optional: if this message is a forward, reference the original message
        public Guid? ForwardedFromMessageId { get; set; }
        public Message? ForwardedFromMessage { get; set; }

        // Foreign key to User (sender)
        public Guid SenderId { get; set; }
        public User Sender { get; set; } = null!;

        //sequence number within the conversation for ordering
        public long Sequence { get; set; }

        // Message content
        public string Content { get; set; } = null!;

        // Optional sticker
        public Guid? StickerId { get; set; }
        public Sticker? Sticker { get; set; }

        // Optional GIF
        public Guid? GifMediaId { get; set; }
        public MediaEntities.Media? GifMedia { get; set; }

        // Optional media attachments
        public ICollection<MediaEntities.MessageAttachment> MessageAttachments { get; set; } = new List<MediaEntities.MessageAttachment>();

        // Message state
        public bool IsEdited { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? EditedAt { get; set; }
        public MessageStatus MessageStatus { get; set; } = MessageStatus.Sent;

        // Optional: convenience for replies or threads
        public MessageReferenceType ReferenceType { get; set; }
        public Guid? ReferenceId { get; set; }

        // For direct reply references (only to messages)
        public Guid? RepliedMessageId { get; set; }

        // THREAD LINK
        public Guid? ThreadId { get; set; }
        public MessageThread? Thread { get; set; }

        // optional: marks root message
        public bool IsThreadRoot { get; set; }

        public ICollection<MessageDeletion> Deletions { get; set; } = new List<MessageDeletion>();

        public ICollection<MessageReaction> Reactions { get; set; } = new List<MessageReaction>();

        public ICollection<MessageRead> Reads { get; set; } = new List<MessageRead>();
        public void SetReference(MessageReferenceType type, Guid? id)
        {
            if (type != MessageReferenceType.None && id == null)
                throw new ArgumentException("ReferenceId is required");

            ReferenceType = type;
            ReferenceId = id;
        }
    }
}