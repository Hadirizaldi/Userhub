namespace UserHub.Application.Auth.Commands.Login;

public sealed record UserCredentials(
    int Id,
    string Email,
    string PasswordHash,
    int StatusId,
    string? RoleName
);