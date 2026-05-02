namespace ChatApp.Domain.Entities.Media
{
    public class GifMetadata
    {
        public Guid Id { get; set; }

        // One-to-one relationship with Media
        public Guid MediaId { get; set; }
        public Media Media { get; set; } = null!;

        // Searchable metadata
        public string? Title { get; set; }

        public List<string> Tags { get; set; } = new();

        // Optional emoji hint for quick categorization
        public string? EmojiHint { get; set; }

        // Trending indicator and usage tracking
        public bool IsTrending { get; set; } = false;
        public int UsageCount { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Track deletion
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
    }
}
