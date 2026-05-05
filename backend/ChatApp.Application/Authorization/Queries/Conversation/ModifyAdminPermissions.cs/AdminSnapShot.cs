namespace ChatApp.Application.Authorization.Queries.Conversation.ModifyAdminPermissions
{
    public sealed record AdminSnapshot(
    Guid UserId,
    Guid GrantedByUserId,
    bool CanManageRoles
    );
}

