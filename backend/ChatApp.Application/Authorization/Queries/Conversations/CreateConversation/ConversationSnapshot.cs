namespace ChatApp.Application.Authorization.Queries.Conversations.CreateConversation
{
    public sealed record ConversationSnapshot(
        Guid Id,
        bool IsGroup
    );
}
