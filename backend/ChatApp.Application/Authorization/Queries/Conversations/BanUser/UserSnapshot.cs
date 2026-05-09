namespace ChatApp.Application.Authorization.Queries.Conversations.BanUser
{
    public sealed record UserSnapshot(
        bool IsBanned,
        bool IsSuspended
    );
}
