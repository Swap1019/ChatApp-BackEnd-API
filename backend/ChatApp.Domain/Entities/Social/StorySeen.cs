namespace ChatApp.Domain.Entities
{
    public class StorySeen
    {
        public Guid StoryId { get; set; }
        public Story Story { get; set; } = null!;

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public DateTime SeenAt { get; set; } = DateTime.UtcNow;
    }
}