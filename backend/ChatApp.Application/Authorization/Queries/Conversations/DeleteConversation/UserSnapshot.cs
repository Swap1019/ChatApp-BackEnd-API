namespace ChatApp.Application.Authorization.Queries.Conversations.DeleteConversation
{
    public sealed record UserSnapshot(
        bool IsBanned,
        bool IsSuspended
    );
}
