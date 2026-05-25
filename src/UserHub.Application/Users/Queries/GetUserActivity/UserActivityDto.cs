namespace UserHub.Application.Users.Queries.GetUserActivity;

public sealed record UserActivityDto(
    int Id,
    string Nip,
    string Fullname,
    string? Rolename,
    DateTime? LastLogin,
    DateTime? LastLogout,
    bool IsLoggedIn,
    int StatusId,
    string Statusname,
    int ConditionStatusId,
    string ConditionStatusName
);