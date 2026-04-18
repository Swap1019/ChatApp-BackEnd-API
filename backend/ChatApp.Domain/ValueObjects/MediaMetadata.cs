namespace ChatApp.Domain.ValueObjects
{
    public class MediaMetadata
    {
        public int? Width { get; set; }
        public int? Height { get; set; }

        // For videos and voice messages
        public TimeSpan? Duration { get; set; }

        public string? ThumbnailUrl { get; set; }

        public string? OriginalFileName { get; set; }
    }
}