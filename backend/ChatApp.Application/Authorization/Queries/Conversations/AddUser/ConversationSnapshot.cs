namespace ChatApp.Application.Authorization.Queries.Conversations.AddUser
{
    public sealed record ConversationSnapshot(
        Guid CreatedById,
        bool IsGroup,
        bool IsDeleted
    );
}
