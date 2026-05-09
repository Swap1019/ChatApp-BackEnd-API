namespace ChatApp.Application.Authorization.Queries.Messages.DeleteMessage;

public sealed record ConversationSnapshot(
    Guid Id,
    Guid CreatedById,
    bool IsGroup,
    bool IsDeleted
);
