namespace ChatApp.Application.Authorization.Queries.Messages.PinMessage
{
    public sealed record UserSnapshot(
        Guid Id,
        bool IsBanned,
        bool IsSuspended
    );
}
