namespace ChatApp.Application.Authorization.Queries.Conversations.ModifyAdminPermissions
{
    public sealed record AdminSnapshot(
    Guid UserId,
    Guid GrantedByUserId,
    bool CanManageRoles
    );
}

