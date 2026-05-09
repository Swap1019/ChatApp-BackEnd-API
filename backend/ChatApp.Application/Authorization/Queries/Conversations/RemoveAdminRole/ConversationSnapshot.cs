namespace ChatApp.Application.Authorization.Queries.Conversations.RemoveAdminRole
{
    public sealed record ConversationSnapshot(
        Guid CreatedById,
        bool IsGroup,
        bool IsDeleted
    );
}
