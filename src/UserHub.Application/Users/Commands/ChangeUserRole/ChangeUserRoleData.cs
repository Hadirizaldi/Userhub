namespace UserHub.Application.Users.Commands.ChangeUserRole;

public sealed record ChangeUserRoleData
(
    int RoleId,
    DateTime UpdatedAt
);