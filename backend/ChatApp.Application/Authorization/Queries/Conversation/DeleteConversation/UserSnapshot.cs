namespace ChatApp.Application.Authorization.Queries.Conversation.DeleteConversation
{
    public sealed record UserSnapshot(
        bool IsBanned,
        bool IsSuspended
    );
}
