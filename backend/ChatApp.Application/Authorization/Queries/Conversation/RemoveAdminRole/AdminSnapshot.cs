namespace ChatApp.Application.Authorization.Queries.Conversation.RemoveAdminRole
{
    public sealed record AdminSnapshot(
        Guid UserId,
        Guid GrantedByUserId
    );
}
