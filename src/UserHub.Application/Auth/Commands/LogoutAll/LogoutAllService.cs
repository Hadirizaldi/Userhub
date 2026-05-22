using UserHub.Application.Abstractions.Auth;
using UserHub.Application.Abstractions.Persistence;
using UserHub.Application.Abstractions.Time;

namespace UserHub.Application.Auth.Commands.LogoutAll;

public sealed class LogoutAllService (
    ICurrentUserAccessor currentUser,
    ISessionRepository sessionRepository,
    IClock clock)
{
    public Task HandleAsync(CancellationToken cancellationToken) =>
        sessionRepository.RevokeAllForUserAsync(currentUser.UserId, clock.UtcNow, cancellationToken);
}