using FluentValidation;
using UserHub.Application.Abstractions.Auth;
using UserHub.Application.Abstractions.Persistence;
using UserHub.Application.Abstractions.Time;

namespace UserHub.Application.Auth.Commands.Logout;

public class LogoutService(
    IValidator<LogoutRequest> validator,
    ISessionRepository sessionRepository,
    IRefreshTokenGenerator refreshTokenGenerator,
    IClock clock)
{
    public async Task HandleAsync(LogoutRequest request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var hash = refreshTokenGenerator.Hash(request.RefreshToken);
        var lookup = await sessionRepository.GetByRefreshTokenHashAsync(hash, cancellationToken);

        if( lookup is null || lookup.SessionRevokedAt is not null) return;

        await sessionRepository.RevokeBySessionIdAsync(lookup.SessionId, clock.UtcNow, cancellationToken);
    }
}