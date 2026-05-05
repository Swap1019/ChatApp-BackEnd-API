namespace ChatApp.Application.Authorization.Queries.Conversation.JoinConversation
{
    public sealed record UserSnapshot(
        bool IsBanned,
        bool IsSuspended
    );
}
