namespace UserHub.Application.Roles.Queries.GetRoles;

public sealed record RoleListItemDto(
    int Id,
    string Name,
    bool IsSystem,
    int UserCount,
    DateTime CreatedAt,
    DateTime UpdatedAt);
