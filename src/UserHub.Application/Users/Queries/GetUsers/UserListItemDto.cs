namespace UserHub.Application.Users.Queries.GetUsers;

public sealed record UserListItemDto(
    int Id,
    string Nip,
    string Fullname,
    string Email,
    int StatusId,
    string StatusName,
    int ConditionStatusId,
    string ConditionStatusName,
    DateTime? CreatedAt
);