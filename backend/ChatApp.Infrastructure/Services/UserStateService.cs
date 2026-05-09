using ChatApp.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using ChatApp.Application.Services;

namespace ChatApp.Infrastructure.Services
{
    public sealed class UserStateService : IUserStateService
    {
        private readonly IAppDbContext _context;

        public UserStateService(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<UserStateDto?> GetUserStateAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new UserStateDto
                {
                    IsBanned = u.IsBanned,
                    IsSuspended = u.IsSuspended
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
