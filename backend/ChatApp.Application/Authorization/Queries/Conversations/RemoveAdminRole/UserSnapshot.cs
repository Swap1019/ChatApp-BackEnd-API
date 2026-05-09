namespace ChatApp.Application.Authorization.Queries.Conversations.RemoveAdminRole
{
    public sealed record UserSnapshot(
        bool IsBanned,
        bool IsSuspended
    );
}
