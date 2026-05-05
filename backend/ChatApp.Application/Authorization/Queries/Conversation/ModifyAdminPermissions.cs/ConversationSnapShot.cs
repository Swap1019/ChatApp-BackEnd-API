namespace ChatApp.Application.Authorization.Queries.Conversation.ModifyAdminPermissions
{
    public sealed record ConversationSnapshot(
        Guid CreatedById,
        bool IsGroup,
        bool IsDeleted
    );
}

