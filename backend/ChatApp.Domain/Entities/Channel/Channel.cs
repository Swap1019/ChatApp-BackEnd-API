using ChatApp.Domain.Entities.Identity;
using ConversationEntities = ChatApp.Domain.Entities.Conversation;
using ChatApp.Domain.Entities.Messaging;

namespace ChatApp.Domain.Entities.Channel
{
    public class Channel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? AvatarUrl { get; set; } // WILL BE UPDATED TO SUPPORT MEDIA ENTITIES LATER

        public Guid CreatorUserId { get; set; }
        public User CreatorUser { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
        public bool OnlyAdminsCanSendMessage { get; set; } = false;
        public bool IsPrivate { get; set; } = false;
        public string? AccessCode { get; set; } // For private channels, a code required to join
        public DateTime? AccessCodeExpiresAt { get; set; }
        public bool IsArchived { get; set; } = false;
        public DateTime? ArchivedAt { get; set; }
        public string? ArchivedBy { get; set; }
        public bool IsDiscoverable { get; set; } = true; // If false, channel won't appear in public listings/search
        

        // Navigation properties
        public ICollection<ConversationEntities.Conversation> Conversations { get; set; } = new List<ConversationEntities.Conversation>(); // One-to-many relationship with Conversation
        public ICollection<ChannelUrl> UrlHistory { get; set; } = new List<ChannelUrl>(); // One-to-many relationship with ChannelUrl
        public ICollection<ChannelUser> Subscribers { get; set; } = new List<ChannelUser>(); // Many-to-many relationship with User through ChannelUser
        public ICollection<ChannelUserBan> BannedUsers { get; set; } = new List<ChannelUserBan>(); // Bans tracked for this channel
        public ICollection<Message> Messages { get; set; } = new List<Message>(); // One-to-many relationship with Message
        public ICollection<ChannelPinnedMessage> PinnedMessages { get; set; } = new List<ChannelPinnedMessage>(); // One-to-many relationship with ChannelPinnedMessage
    }
}