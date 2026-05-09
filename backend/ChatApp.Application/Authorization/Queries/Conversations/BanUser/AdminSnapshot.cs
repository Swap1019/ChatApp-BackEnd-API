namespace ChatApp.Application.Authorization.Queries.Conversations.BanUser
{
    public sealed record AdminSnapshot(
        Guid UserId,
        bool CanKickMembers
    );
}
