using System;

using ChatApp.Domain.Entities.Identity;

namespace ChatApp.Domain.Entities.Social
{
    public class Contact
    {
        public Guid Id { get; set; }

        // Foreign key to the owner of this contact
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        // Foreign key to the actual contact (another user)
        public Guid ContactUserId { get; set; }
        public User ContactUser { get; set; } = null!;

        public string? Nickname { get; set; } // Optional display name
        public string? Notes { get; set; } // Optional note about this contact
        public bool IsFavorite { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}