namespace ChatApp.Application.Authorization.Queries.Conversation.RemoveAdminRole
{
    public sealed record ConversationSnapshot(
        Guid CreatedById,
        bool IsGroup,
        bool IsDeleted
    );
}
