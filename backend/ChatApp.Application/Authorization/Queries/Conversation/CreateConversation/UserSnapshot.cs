namespace ChatApp.Application.Authorization.Queries.Conversation.CreateConversation
{
    public sealed record UserSnapshot(
        bool IsBanned,
        bool IsSuspended
    );
}
