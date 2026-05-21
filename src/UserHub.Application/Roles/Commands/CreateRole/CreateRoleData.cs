namespace UserHub.Application.Roles.Commands.CreateRole;

public sealed record CreateRoleData(
    string Name,
    DateTime CreatedAt,
    DateTime UpdatedAt);
