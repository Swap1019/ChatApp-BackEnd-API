namespace ChatApp.Application.Authorization.Queries.Conversations.JoinConversation
{
    public sealed record ConversationSnapshot(
        bool IsGroup,
        bool IsDeleted
    );
}
