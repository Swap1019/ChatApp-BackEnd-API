namespace ChatApp.Application.Authorization.Queries.Conversations.UrlGroupCreation
{
    public sealed record ConversationSnapshot(
        Guid CreatedById,
        bool IsGroup,
        bool IsDeleted
    );
}

