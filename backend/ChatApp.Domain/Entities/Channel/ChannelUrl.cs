namespace ChatApp.Domain.Entities.Channel
{
    public class ChannelUrl
    {
        public Guid Id { get; private set; }
        public Guid ChannelId { get; set; }
        public Channel Channel { get; set; } = null!;

        public string UrlSlug { get; set; } = null!;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? DeactivatedAt { get; set; }
        public string? DeactivatedReason { get; set; }

        public ChannelUrl(Guid channelId, string urlSlug)
        {
            Id = Guid.NewGuid();
            ChannelId = channelId;
            UrlSlug = urlSlug;
        }

        public void Deactivate(string? reason = null)
        {
            IsActive = false;
            DeactivatedAt = DateTime.UtcNow;
            DeactivatedReason = reason;
        }
    }
}
