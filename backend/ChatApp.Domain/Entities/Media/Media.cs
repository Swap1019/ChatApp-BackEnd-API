using ChatApp.Domain.ValueObjects;
using ChatApp.Domain.Enums;

namespace ChatApp.Domain.Entities
{
    public class Media
    {
        public Guid Id { get; set; }

        public Guid UploadedByUserId { get; set; }
        public User UploadedByUser { get; set; } = null!;

        public string Url { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public long SizeBytes { get; set; }

        public MediaType Type { get; set; }

        public MediaProcessingStatus ProcessingStatus { get; set; }

        public string? ContentHash { get; set; }

        public string? StorageProvider { get; set; }
        public string? StorageKey { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? DeletedAt { get; set; }
        public bool IsDeleted { get; set; } = false;

        // optional global metadata
        public MediaMetadata Metadata { get; set; } = new();

        public ICollection<MediaModeration> ModerationFlags { get; set; }
            = new List<MediaModeration>();

        public ICollection<SensitiveContentFlag> SensitiveContentFlags { get; set; }
            = new List<SensitiveContentFlag>();
        
        // Navigation properties for attachments
        public ICollection<MessageAttachment> MessageAttachments { get; set; } = new List<MessageAttachment>();
        public ICollection<PostAttachment> PostAttachments { get; set; } = new List<PostAttachment>();
    }
}