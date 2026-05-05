namespace ChatApp.Application.Authorization.Queries.Messages.PinMessage
{
    public sealed record ConversationSnapshot(
        Guid CreatedById,
        bool IsGroup,
        bool IsDeleted
    );
}
