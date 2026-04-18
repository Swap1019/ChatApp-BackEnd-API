using System.ComponentModel.DataAnnotations; 

namespace ChatApp.Domain.Entities
{
    public class UserSession
    {
        public Guid Id { get; set; }

        // Foreign key
        public Guid UserId { get; set; }

        // Navigation property
        public User User { get; set; } = null!;

        [MaxLength(500)]
        public string DeviceInfo { get; set; } = null!;

        [MaxLength(50)]
        public string IPAddress { get; set; } = null!;

        public string RefreshToken { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiresAt { get; set; }

        public bool IsRevoked { get; set; } = false;
        public string? RevokeReason { get; set; }
        public DateTime LastActivityAt { get; set; } = DateTime.UtcNow;

        public bool IsActive => !IsRevoked && (ExpiresAt == null || ExpiresAt > DateTime.UtcNow);
    }
}