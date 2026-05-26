namespace UserHub.Application.Users.Commands.DeleteUser;

public sealed record UserDeletionInfo(
    bool IsDeleted,
    string? RoleName
);
