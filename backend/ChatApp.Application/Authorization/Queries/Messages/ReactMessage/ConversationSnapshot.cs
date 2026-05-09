namespace ChatApp.Application.Authorization.Queries.Messages.ReactMessage;

public sealed record ConversationSnapshot(
    Guid Id,
    bool IsGroup,
    bool IsDeleted
);
