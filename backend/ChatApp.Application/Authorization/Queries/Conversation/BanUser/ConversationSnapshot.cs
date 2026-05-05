namespace ChatApp.Application.Authorization.Queries.Conversation.BanUser
{
    public sealed record ConversationSnapshot(
        Guid CreatedById,
        bool IsGroup,
        bool IsDeleted
    );
}
