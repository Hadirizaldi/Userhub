using UserHub.Application.Users.Queries.GetUsers;
using UserHub.Application.Users.Commands.CreateUser;
using UserHub.Application.Users.Commands.UpdateUser;
using UserHub.Application.Users.Commands.ChangeUserRole;
using UserHub.Application.Users.Commands.ChangeUserStatus;
using UserHub.Application.Users.Commands.ChangeUserPassword;
using UserHub.Application.Auth.Commands.Login;

namespace UserHub.Application.Abstractions.Persistence;

public interface IUserRepository
{
    Task<(IReadOnlyList<UserListItemDto> Items, int TotalCount)> ListAsync(
        int Page,
        int PageSize,
        string? Search,
        CancellationToken cancellationToken );

    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken);

    Task<int> AddAsync(CreateUserData data, CancellationToken cancellationToken);

    Task<UserListItemDto?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<bool> UpdateAsync(int id, UpdateUserData data, CancellationToken cancellationToken);

    Task<int?> GetStatusIdAsync(int id, CancellationToken cancellationToken);

    Task<bool> ChangeRoleAsync(int userId, ChangeUserRoleData data, CancellationToken cancellationToken);

    Task<bool> ChangeStatusAsync(int userId, ChangeUserStatusData data, CancellationToken cancellationToken);

    Task<bool> ChangePasswordAsync(int userId, ChangeUserPasswordData data, CancellationToken cancellationToken);

    Task<bool> SoftDeleteAsync(int userId, DateTime utcNow, CancellationToken cancellationToken);

    Task<bool> RestoreAsync(int userId, DateTime utcNow, CancellationToken cancellationToken);

    Task<int> CountByRoleAsync(int roleId, CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<int, int>> GetStatusByIdsAsync(IReadOnlyList<int> userIds, CancellationToken cancellationToken);

    Task<int> BulkAssignRolesAsync(IReadOnlyList<(int UserId, int RoleId)> assignments, DateTime utcNow, CancellationToken cancellationToken);

    Task<UserCredentials?> GetCredentialsByEmailAsync(string email, CancellationToken cancellationToken);
}