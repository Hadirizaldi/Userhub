namespace UserHub.Application.Roles.Queries.GetRoleById;

public sealed record RoleDto(
    int Id,
    string Name,
    bool IsSystem,
    DateTime CreatedAt,
    DateTime UpdatedAt);
