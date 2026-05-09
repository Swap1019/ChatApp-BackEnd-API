namespace ChatApp.Application.Authorization.Queries.Conversations.RemoveAdminRole
{
    public sealed record AdminSnapshot(
        Guid UserId,
        Guid GrantedByUserId
    );
}
