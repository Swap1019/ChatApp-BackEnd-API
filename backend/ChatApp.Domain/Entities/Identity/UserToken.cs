using System.ComponentModel.DataAnnotations;

namespace ChatApp.Domain.Entities.Identity
{
    public class UserToken
    {
        public Guid Id { get; set; }

        // Foreign key
        public Guid UserId { get; set; }

        // Navigation property
        public User User { get; set; } = null!;

        [MaxLength(200)]
        public string Token { get; set; } = null!;

        [MaxLength(50)]
        public string Type { get; set; } = null!; // PasswordReset, EmailVerification, etc.

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }

        public bool IsUsed { get; set; } = false;
        public DateTime? ConsumedAt { get; set; }
    }
}

