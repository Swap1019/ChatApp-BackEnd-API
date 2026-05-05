namespace ChatApp.Application.Authorization.Queries.Conversation.BanUser
{
    public sealed record UserSnapshot(
        bool IsBanned,
        bool IsSuspended
    );
}
