namespace ChatApp.Application.Authorization.Queries.Conversations.DeleteConversation
{
    public sealed record ConversationSnapshot(
        Guid CreatedById,
        bool IsGroup,
        bool IsDeleted
    );
}
