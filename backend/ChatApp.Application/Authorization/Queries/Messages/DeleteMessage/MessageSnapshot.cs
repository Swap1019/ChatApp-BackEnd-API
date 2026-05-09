using ChatApp.Domain.Enums;

namespace ChatApp.Application.Authorization.Queries.Messages.DeleteMessage;

public sealed record MessageSnapshot(
    Guid SenderId,
    MessageContextType? ContextType,
    Guid? ConversationId,
    bool IsDeleted
);