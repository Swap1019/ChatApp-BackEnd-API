namespace ChatApp.Application.Authorization.Queries.Conversations.JoinConversation
{
    public sealed record UserSnapshot(
        bool IsBanned,
        bool IsSuspended
    );
}
