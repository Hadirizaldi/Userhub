namespace UserHub.Application.Auth.Commands.Login;

public sealed record CreateLoginData(
    int UserId,
    string? IpAddress,
    string? UserAgent,
    string RefreshTokenHash,
    DateTime RefreshTokenExpiresAt,
    DateTime UtcNow);
