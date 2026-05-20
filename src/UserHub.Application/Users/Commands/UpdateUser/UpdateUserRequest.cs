namespace UserHub.Application.Users.Commands.UpdateUser;

public sealed record UpdateUserRequest(
    string Fullname,
    string? Phone
);

