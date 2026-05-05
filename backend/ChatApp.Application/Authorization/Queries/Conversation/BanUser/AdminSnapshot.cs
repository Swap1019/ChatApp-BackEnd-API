namespace ChatApp.Application.Authorization.Queries.Conversation.BanUser
{
    public sealed record AdminSnapshot(
        Guid UserId,
        bool CanKickMembers
    );
}
