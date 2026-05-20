namespace UserHub.Application.Users.Commands.UpdateUser;

public sealed record UpdateUserData(
    string Fullname,
    string? Phone,
    DateTime UpdatedAt
);