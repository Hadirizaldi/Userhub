namespace UserHub.Application.Auth.Commands.Refresh;

public sealed record RefreshResult
(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);