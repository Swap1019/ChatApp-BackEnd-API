namespace ChatApp.Application.Authorization.Queries.Conversations.AddUser
{
    public sealed record UserSnapshot(
        Guid UserId,
        bool IsBanned,
        bool IsSuspended
    );
}
