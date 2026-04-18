using ChatApp.Domain.Enums;

namespace ChatApp.Domain.Entities
{
    public class UserSuspension
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Guid? SuspendedByUserId { get; set; }
        public User? SuspendedByUser { get; set; }

        public string Reason { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // null = permanent (ban)
        public DateTime? EndsAt { get; set; }

        // Soft delete
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
    }
}