using ChatApp.Application.Abstractions.Time;

namespace ChatApp.Infrastructure.Time;

public sealed class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
