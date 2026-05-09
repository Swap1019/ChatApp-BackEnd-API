namespace ChatApp.Application.Authorization.Queries.Conversations.CreateConversation
{
    public sealed record UserSnapshot(
        bool IsBanned,
        bool IsSuspended
    );
}
