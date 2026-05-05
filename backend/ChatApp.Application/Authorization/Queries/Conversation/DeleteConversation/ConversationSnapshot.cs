namespace ChatApp.Application.Authorization.Queries.Conversation.DeleteConversation
{
    public sealed record ConversationSnapshot(
        Guid CreatedById,
        bool IsGroup,
        bool IsDeleted
    );
}
