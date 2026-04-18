using ChatApp.Domain.Enums;

namespace ChatApp.Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public NotificationType Type { get; set; }
        public Guid? ReferenceId { get; set; }

        // JSON payload for additional data
        public string? Data { get; set; }

        // Priority: 1=Low, 2=Normal, 3=High, 4=Urgent
        public int Priority { get; set; } = 2;

        // For grouping similar notifications (e.g., multiple likes on same post)
        public Guid? GroupingId { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Soft delete
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
    }
}