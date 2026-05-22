namespace UserHub.Application.Auth.Commands.Logout;

public sealed record LogoutRequest
(
    string RefreshToken
);