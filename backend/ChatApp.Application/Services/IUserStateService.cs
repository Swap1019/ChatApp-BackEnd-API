namespace ChatApp.Application.Services
{
    public interface IUserStateService
    {
        Task<UserStateDto?> GetUserStateAsync(Guid userId, CancellationToken cancellationToken = default);
    }

    public sealed class UserStateDto
    {
        public bool IsBanned { get; init; }
        public bool IsSuspended { get; init; }
    }   
}