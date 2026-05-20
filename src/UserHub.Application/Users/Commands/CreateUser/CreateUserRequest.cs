namespace UserHub.Application.Users.Commands.CreateUser;

public sealed record CreateUserRequest(
    string Fullname,
    string Email,
    string Password,
    string? Phone);
