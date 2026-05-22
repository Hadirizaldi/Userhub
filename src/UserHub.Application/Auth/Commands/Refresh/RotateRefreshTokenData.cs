namespace UserHub.Application.Auth.Commands.Refresh;

public sealed record RotateRefreshTokenData(
    long OldRefreshTokenId,
    long SessionId,
    string NewRefreshTokenHash,
    DateTime NewRefreshTokenExpiresAt,
    DateTime UtcNow);
