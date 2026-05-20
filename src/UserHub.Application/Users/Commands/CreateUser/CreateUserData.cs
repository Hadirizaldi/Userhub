namespace UserHub.Application.Users.Commands.CreateUser;

public sealed record CreateUserData(
    string Nip,
    string Fullname,
    string Email,
    string PasswordHash,
    string? Phone,
    int StatusId,
    int ConditionStatusId,
    int RoleId,
    DateTime CreatedAt,
    DateTime UpdatedAt
);