namespace UserHub.Application.Auth.Commands.Login;

public sealed record LoginResult
(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);