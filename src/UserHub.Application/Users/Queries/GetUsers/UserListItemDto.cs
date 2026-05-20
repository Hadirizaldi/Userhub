namespace UserHub.Application.Users.Queries.GetUsers;

public sealed record UserListItemDto(
    int Id,
    string Nip,
    string Fullname,
    string Email,
    string? Phone,
    int StatusId,
    string StatusName,
    int ConditionStatusId,
    string ConditionStatusName,
    int? RoleId,
    string? RoleName,
    DateTime? CreatedAt
);