using UserHub.Application.Auth.Commands.Login;
using UserHub.Application.Auth.Commands.Refresh;
using UserHub.Application.Auth.Queries.GetSessions;

namespace UserHub.Application.Abstractions.Persistence;

public interface ISessionRepository
{
    Task<CreateLoginResult> CreateLoginAsync(CreateLoginData data, CancellationToken cancellationToken);

    Task<RefreshTokenLookup?> GetByRefreshTokenHashAsync(string tokenHash, CancellationToken cancellationToken);

    Task<long> RotateRefreshTokenAsync(RotateRefreshTokenData data, CancellationToken cancellationToken);

    Task RevokeBySessionIdAsync(long sessionId, DateTime utcNow, CancellationToken cancellationToken);

    Task RevokeAllForUserAsync(int userId, DateTime utcNow, CancellationToken cancellationToken);

    Task<IReadOnlyList<SessionListItemDto>> GetActiveByUserAsync(int userId, CancellationToken cancellationToken);

    Task<(int RefreshTokensDeleted, int SessionsDeleted)> DeleteOldAsync(
        DateTime refreshTokenExpiredCutoff,
        DateTime refreshTokenRevokedCutoff,
        DateTime sessionRevokedCutoff,
        CancellationToken cancellationToken
    );
}
