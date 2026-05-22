namespace UserHub.Application.Auth.Commands.Login;

public sealed record LoginRequest(
    string Email, 
    string Password
);