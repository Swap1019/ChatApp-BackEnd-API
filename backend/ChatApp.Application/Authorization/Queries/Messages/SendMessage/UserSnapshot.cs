namespace ChatApp.Application.Authorization.Queries.Messages.SendMessage
{
    public sealed record UserSnapshot(
        bool IsBanned,
        bool IsSuspended
    );
}
