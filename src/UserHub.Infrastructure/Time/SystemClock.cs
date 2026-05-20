using UserHub.Application.Abstractions.Time;

namespace UserHub.Infrastructure.Time;

public sealed class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}