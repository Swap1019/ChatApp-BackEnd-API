using ChatApp.Domain.Enums;

namespace ChatApp.Domain.Entities.Media
{
    public class SensitiveContentFlag
    {
        public Guid Id { get; private set; }

        public Guid MediaId { get; private set; }
        public Media Media { get; private set; } = null!;

        public SensitiveContentType Type { get; private set; }
        public SensitiveContentSeverity Severity { get; private set; }

        public float Confidence { get; private set; } // 0.0 - 1.0

        public string? Reason { get; private set; }

        public DateTime DetectedAt { get; private set; } = DateTime.UtcNow;

        public string? ModelVersion { get; private set; }

        public bool IsReviewed { get; private set; } = false;
        public bool IsApproved { get; private set; } = false;

        public DateTime? ReviewedAt { get; private set; }

        public string? ReviewedBy { get; private set; }

        // EF Core requires empty constructor
        private SensitiveContentFlag() { }

        public SensitiveContentFlag(
            Guid mediaId,
            SensitiveContentType type,
            SensitiveContentSeverity severity,
            float confidence,
            string? reason,
            string? modelVersion)
        {
            Id = Guid.NewGuid();
            MediaId = mediaId;
            Type = type;
            Severity = severity;
            Confidence = confidence;
            Reason = reason;
            ModelVersion = modelVersion;

            Validate();
        }

        private void Validate()
        {
            if (Confidence < 0 || Confidence > 1)
                throw new ArgumentException("Confidence must be between 0 and 1");
        }

        public void MarkReviewed(string reviewedBy, bool approved)
        {
            IsReviewed = true;
            IsApproved = approved;
            ReviewedBy = reviewedBy;
            ReviewedAt = DateTime.UtcNow;
        }
    }
}