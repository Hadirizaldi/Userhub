namespace UserHub.Application.Users.Commands.BulkAssignRoles;

public sealed record BulkAssignRolesRequest(IReadOnlyList<UserRoleAssignment> Assignments);
