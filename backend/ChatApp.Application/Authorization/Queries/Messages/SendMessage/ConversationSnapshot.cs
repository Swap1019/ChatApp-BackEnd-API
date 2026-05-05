namespace ChatApp.Application.Authorization.Queries.Messages.SendMessage
{
    public sealed record ConversationSnapshot(
        bool IsGroup,
        bool IsDeleted
    );
}
