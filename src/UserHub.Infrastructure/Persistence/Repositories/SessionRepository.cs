using Microsoft.EntityFrameworkCore;
using UserHub.Application.Abstractions.Persistence;
using UserHub.Application.Auth.Commands.Login;
using UserHub.Application.Auth.Commands.Refresh;
using UserHub.Application.Auth.Queries.GetSessions;
using UserHub.Infrastructure.Persistence.Entities;

namespace UserHub.Infrastructure.Persistence.Repositories;

public sealed class SessionRepository(AppDbContext db) : ISessionRepository
{
    public async Task<CreateLoginResult> CreateLoginAsync(CreateLoginData data, CancellationToken cancellationToken)
    {
        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);

        var loginLog = new LoginLogs
        {
            UserId = data.UserId,
            LoginAt = data.UtcNow,
            LogoutAt = null,
            IsLoggedIn = true
        };
        db.LoginLogs.Add(loginLog);
        await db.SaveChangesAsync(cancellationToken);

        var session = new Sessions
        {
            LoginLogId = loginLog.Id,
            UserId = data.UserId,
            IpAddress = data.IpAddress,
            UserAgent = data.UserAgent,
            LastUsedAt = data.UtcNow,
            RevokedAt = null,
            CreatedAt = data.UtcNow
        };
        db.Sessions.Add(session);
        await db.SaveChangesAsync(cancellationToken);

        var token = new RefreshTokens
        {
            SessionId = session.Id,
            TokenHash = data.RefreshTokenHash,
            ExpiresAt = data.RefreshTokenExpiresAt,
            RevokedAt = null,
            CreatedAt = data.UtcNow
        };
        db.RefreshTokens.Add(token);
        await db.SaveChangesAsync(cancellationToken);

        await tx.CommitAsync(cancellationToken);
        return new CreateLoginResult(session.Id, loginLog.Id, token.Id);
    }

    public Task<RefreshTokenLookup?> GetByRefreshTokenHashAsync(string tokenHash, CancellationToken cancellationToken) =>
        db.RefreshTokens
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Where(t => t.TokenHash == tokenHash)
            .Select(t => new RefreshTokenLookup(
                t.Id,
                t.SessionId,
                t.Session.LoginLogId,
                t.Session.UserId,
                t.Session.User.Email,
                t.Session.User.Role.Select(r => r.Name).FirstOrDefault(),
                t.ExpiresAt,
                t.RevokedAt,
                t.Session.RevokedAt
            ))
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<long> RotateRefreshTokenAsync(
        RotateRefreshTokenData data, CancellationToken cancellationToken)
    {
        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);

        var oldToken = await db.RefreshTokens
            .FirstOrDefaultAsync(t => t.Id == data.OldRefreshTokenId, cancellationToken)
            ?? throw new InvalidOperationException("Refresh token not found.");
        oldToken.RevokedAt = data.UtcNow;

        var newToken = new RefreshTokens
        {
            SessionId = data.SessionId,
            TokenHash = data.NewRefreshTokenHash,
            ExpiresAt = data.NewRefreshTokenExpiresAt,
            RevokedAt = null,
            CreatedAt = data.UtcNow
        };
        db.RefreshTokens.Add(newToken);
        await db.SaveChangesAsync(cancellationToken);

        var session = await db.Sessions
            .FirstOrDefaultAsync(s => s.Id == data.SessionId, cancellationToken)
            ?? throw new InvalidOperationException("Session not found."); 
        session.LastUsedAt = data.UtcNow;
        await db.SaveChangesAsync(cancellationToken);

        await tx.CommitAsync(cancellationToken);
        return newToken.Id;
    }

    public async Task RevokeBySessionIdAsync(
        long sessionId, DateTime utcNow, CancellationToken cancellationToken)
    {
        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);

        var session = await db.Sessions
            .FirstOrDefaultAsync(s => s.Id == sessionId, cancellationToken);
        if (session is null) return;

        if (session.RevokedAt is null)
            session.RevokedAt = utcNow;

        var loginLog = await db.LoginLogs
            .FirstOrDefaultAsync(l => l.Id == session.LoginLogId, cancellationToken);
        if (loginLog is not null && loginLog.IsLoggedIn == true)
        {
            loginLog.LogoutAt = utcNow;
            loginLog.IsLoggedIn = false;
        }

        await db.SaveChangesAsync(cancellationToken);
        await tx.CommitAsync(cancellationToken);
    }

    public async Task RevokeAllForUserAsync(
        int userId, DateTime utcNow, CancellationToken cancellationToken)
    {
        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);

        var activeSessions = await db.Sessions
            .Where(s => s.UserId == userId && s.RevokedAt == null)
            .ToListAsync(cancellationToken);

        if (activeSessions.Count == 0)
        {
            await tx.CommitAsync(cancellationToken);
            return;
        }

        var loginLogIds = activeSessions.Select(s => s.LoginLogId).ToList();

        foreach (var s in activeSessions)
            s.RevokedAt = utcNow;

        var loginLogs = await db.LoginLogs
            .Where(l => loginLogIds.Contains(l.Id) && l.IsLoggedIn == true)
            .ToListAsync(cancellationToken);

        foreach (var l in loginLogs)
        {
            l.LogoutAt = utcNow;
            l.IsLoggedIn = false;
        }

        await db.SaveChangesAsync(cancellationToken);
        await tx.CommitAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SessionListItemDto>> GetActiveByUserAsync(
        int userId, CancellationToken cancellationToken) =>
        await db.Sessions
            .AsNoTracking()
            .Where(s => s.UserId == userId && s.RevokedAt == null)
            .OrderByDescending(s => s.LastUsedAt)
            .Select(s => new SessionListItemDto(
                s.Id,
                s.IpAddress,
                s.UserAgent,
                s.LastUsedAt,
                s.CreatedAt))
            .ToListAsync(cancellationToken);
}