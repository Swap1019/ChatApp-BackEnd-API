namespace ChatApp.Domain.Entities
{
    public class BlockedUser
    {
        public Guid Id { get; set; }

        // The user who is doing the blocking
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        // The user being blocked
        public Guid BlockedUserId { get; set; }
        public User Blocked { get; set; } = null!;

        public string? Reason { get; set; } // optional
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}