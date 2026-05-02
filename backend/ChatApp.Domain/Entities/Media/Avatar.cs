using ChatApp.Domain.Enums;
using ChatApp.Domain.Entities.Identity;

namespace ChatApp.Domain.Entities.Media
{
    public class Avatar
    {
        public Guid Id { get; set; }

        // Foreign key to Media - avatar files are stored in Media
        public Guid MediaId { get; set; }
        public Media Media { get; set; } = null!;

        // Owner information (polymorphic: User, Channel, or Conversation)
        public AvatarOwnerType OwnerType { get; set; }
        public Guid OwnerId { get; set; }

        // Track who uploaded this avatar
        public Guid UploadedByUserId { get; set; }
        public User UploadedByUser { get; set; } = null!;

        // Version tracking for avatar updates
        public int Version { get; set; } = 1;

        // Status
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}