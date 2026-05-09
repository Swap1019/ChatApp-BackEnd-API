namespace ChatApp.Application.Authorization.Queries.Conversations.BanUser
{
    public sealed record ConversationSnapshot(
        Guid CreatedById,
        bool IsGroup,
        bool IsDeleted
    );
}
