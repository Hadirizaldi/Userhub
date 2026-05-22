using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UserHub.Application.Abstractions.Persistence;
using UserHub.Application.Abstractions.Time;

namespace UserHub.Application.Auth.Jobs;

public sealed class TokenCleanupJob(
    ISessionRepository sessionRepository,
    IClock clock,
    IOptions<CleanupOptions> options,
    ILogger<TokenCleanupJob> logger)
{
    private readonly CleanupOptions _opt = options.Value;

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        var now = clock.UtcNow;
        var (tokens, sessions) = await sessionRepository.DeleteOldAsync(
            refreshTokenExpiredCutoff: now.AddDays(-_opt.RefreshTokenExpiredRetentionDays),
            refreshTokenRevokedCutoff: now.AddDays(-_opt.RefreshTokenRevokedRetentionDays),
            sessionRevokedCutoff: now.AddDays(-_opt.SessionRevokedRetentionDays),
            cancellationToken);

        logger.LogInformation(
            "TokenCleanupJob: deleted {Tokens} refresh tokens, {Sessions} sessions",
            tokens, sessions);
    }
}
