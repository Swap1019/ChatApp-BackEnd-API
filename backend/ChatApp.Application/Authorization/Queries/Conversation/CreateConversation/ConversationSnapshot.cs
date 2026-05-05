namespace ChatApp.Application.Authorization.Queries.Conversation.CreateConversation
{
    public sealed record ConversationSnapshot(
        Guid Id,
        bool IsGroup
    );
}
