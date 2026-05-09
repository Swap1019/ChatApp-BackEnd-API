using ChatApp.Domain.Enums;

namespace ChatApp.Application.Authorization.Queries.Messages.ReactMessage;

public sealed record MessageSnapshot(
    Guid Id,
    bool IsDeleted
);