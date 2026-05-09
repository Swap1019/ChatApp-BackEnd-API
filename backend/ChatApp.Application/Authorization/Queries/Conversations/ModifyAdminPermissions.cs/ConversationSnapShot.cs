namespace ChatApp.Application.Authorization.Queries.Conversations.ModifyAdminPermissions
{
    public sealed record ConversationSnapshot(
        Guid CreatedById,
        bool IsGroup,
        bool IsDeleted
    );
}

