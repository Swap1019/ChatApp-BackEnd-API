namespace ChatApp.Application.Authorization.Queries.Conversation.RemoveAdminRole
{
    public sealed record UserSnapshot(
        bool IsBanned,
        bool IsSuspended
    );
}
