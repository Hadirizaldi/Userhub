namespace UserHub.Domain.Common;

public static class ErrorCodes
{
    // Auth
    public const string InvalidCredentials = "INVALID_CREDENTIALS";
    public const string UserInactive = "USER_INACTIVE";
    public const string InvalidRefreshToken = "INVALID_REFRESH_TOKEN";
    public const string RefreshTokenExpired = "REFRESH_TOKEN_EXPIRED";
    public const string Forbidden = "FORBIDDEN";
    public const string LastAdmin = "LAST_ADMIN";

    // User
    public const string EmailAlreadyTaken = "EMAIL_ALREADY_TAKEN";
    public const string UserNotActive = "USER_NOT_ACTIVE";
    public const string UsersNotFound = "USERS_NOT_FOUND";
    public const string CannotDeleteSelf = "CANNOT_DELETE_SELF";

    // Role
    public const string RoleNameTaken = "ROLE_NAME_TAKEN";
    public const string RoleInUse = "ROLE_IN_USE";
    public const string RoleIsSystem = "ROLE_IS_SYSTEM";
    public const string RolesNotFound = "ROLES_NOT_FOUND";
}
