namespace UserHub.Application.Users.Commands.HardDeleteUser;

public sealed record HardDeleteUserInfo(
    bool IsDeleted,
    string? RoleName
);