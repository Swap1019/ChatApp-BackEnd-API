namespace ChatApp.Application.Authorization.Queries.Conversation.JoinConversation
{
    public sealed record ConversationSnapshot(
        bool IsGroup,
        bool IsDeleted
    );
}
