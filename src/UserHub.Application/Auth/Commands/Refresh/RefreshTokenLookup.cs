namespace UserHub.Application.Auth.Commands.Refresh;

public sealed record RefreshTokenLookup(
    long RefreshTokenId,
    long SessionId,
    long LoginLogId,
    int UserId,
    string Email,
    string? RoleName,
    DateTime TokenExpiresAt,
    DateTime? TokenRevokedAt,
    DateTime? SessionRevokedAt);
