namespace ChatApp.Application.Authorization.Queries.Conversations.AddUser
{
    public sealed record AdminSnapshot(
        Guid UserId,
        bool CanAddMembers
    );
}
