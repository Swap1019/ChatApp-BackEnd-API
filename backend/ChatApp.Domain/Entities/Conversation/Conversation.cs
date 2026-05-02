using ChatApp.Domain.Entities.Identity;
using ChatApp.Domain.Entities.Messaging;

namespace ChatApp.Domain.Entities.Conversation
{
    public class Conversation
    {
        public Guid Id { get; private set; }
        public required string Name { get; set; }
        public bool IsGroup { get; set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public User CreatedBy { get; set; } = null!;
        public Guid CreatedById { get; set; }
        public DateTime? LastMessageAt { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Description { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid UpdatedById { get; set; }
        public User UpdatedBy { get; set; } = null!;
        public bool OnlyAdminsCanSendMeesage { get; set; } = false;

        // URL sharing properties
        public string? ActiveUrlSlug { get; set; }
        public DateTime? UrlSlugUpdatedAt { get; set; }

        //One to many relationship with Message
        public ICollection<Message> Messages { get; set; } = new List<Message>();

        //One to many relationship with PinnedMessage
        public ICollection<PinnedMessage> PinnedMessages { get; set; } = new List<PinnedMessage>();

        //Many to many relationship with User through ConversationUser
        public ICollection<ConversationUser> Members { get; set; } = new List<ConversationUser>();

        // Bans tracked for this conversation
        public ICollection<ConversationUserBan> BannedUsers { get; set; } = new List<ConversationUserBan>();

        // URL history for this conversation
        public ICollection<ConversationUrl> UrlHistory { get; set; } = new List<ConversationUrl>();

        // Convenience property to get admins of the conversation
        public IEnumerable<ConversationUser> Admins => Members.Where(m => m.IsAdmin);
        // Constructors for managing conversation creation and updates
        public Conversation(string name, bool isGroup)
        {
            Id = Guid.NewGuid();

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Conversation name is required.", nameof(name));

            Name = name;
            IsGroup = isGroup;
        }

        public void UpdateConversation(string? name, string? description, string? avatarUrl, Guid updatedById)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Conversation name is required.", nameof(name));

            Name = name;
            Description = description;
            AvatarUrl = avatarUrl;
            UpdatedAt = DateTime.UtcNow;
            UpdatedById = updatedById;
        }

        public void MarkAsDeleted(Guid deletedById)
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            UpdatedById = deletedById;
        }

        public void MarkAsActive(Guid updatedById)
        {
            IsDeleted = false;
            DeletedAt = null;
            UpdatedAt = DateTime.UtcNow;
            UpdatedById = updatedById;
        }

        public void UpdateLastMessageTime()
        {
            LastMessageAt = DateTime.UtcNow;
        }

        public void RemoveMember(Guid userId)
        {
            var member = Members.FirstOrDefault(m => m.UserId == userId);
            if (member == null)
            {
                throw new InvalidOperationException("Cannot remove member , member does not exist in the conversation.");
            }

            Members.Remove(member);
        }

        public void GenerateUrlSlug(string newSlug)
        {
            if (string.IsNullOrWhiteSpace(newSlug))
                throw new ArgumentException("URL slug cannot be empty.", nameof(newSlug));

            // Validate slug format: alphanumeric, hyphens, underscores only
            if (!System.Text.RegularExpressions.Regex.IsMatch(newSlug, @"^[a-zA-Z0-9_-]+$"))
                throw new ArgumentException("URL slug can only contain alphanumeric characters, hyphens, and underscores.", nameof(newSlug));

            ActiveUrlSlug = newSlug;
            UrlSlugUpdatedAt = DateTime.UtcNow;
        }

        public void RotateUrlSlug(string newSlug, string? reason = null)
        {
            if (string.IsNullOrWhiteSpace(newSlug))
                throw new ArgumentException("URL slug cannot be empty.", nameof(newSlug));

            if (!System.Text.RegularExpressions.Regex.IsMatch(newSlug, @"^[a-zA-Z0-9_-]+$"))
                throw new ArgumentException("URL slug can only contain alphanumeric characters, hyphens, and underscores.", nameof(newSlug));

            GenerateUrlSlug(newSlug);
        }
    }
}