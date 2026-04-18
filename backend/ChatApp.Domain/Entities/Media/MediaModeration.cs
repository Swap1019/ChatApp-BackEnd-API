using ChatApp.Domain.Enums;

namespace ChatApp.Domain.Entities
{
    public class MediaModeration
    {
        public Guid Id { get; set; }

        public Guid MediaId { get; set; }
        public Media Media { get; set; } = null!;

        public bool IsSensitive { get; set; }

        public string? Labels { get; set; }

        // AI confidence score (0.0 - 1.0)
        public float Confidence { get; set; }

        public MediaReviewStatus ReviewStatus { get; set; } = MediaReviewStatus.Pending;

        public Guid? ReviewedById { get; set; }
        public User? ReviewedBy { get; set; }

        public DateTime? ReviewedAt { get; set; }
    }
}