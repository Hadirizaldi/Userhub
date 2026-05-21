using UserHub.Application.Roles.Commands.CreateRole;
using UserHub.Application.Roles.Commands.UpdateRole;
using UserHub.Application.Roles.Queries.GetRoleById;
using UserHub.Application.Roles.Queries.GetRoles;
using UserHub.Application.Roles.Queries.LookupRoles;

namespace UserHub.Application.Abstractions.Persistence;

public interface IRoleRepository
{
    Task<IReadOnlyList<RoleLookupDto>> GetLookupAsync(CancellationToken cancellationToken);

    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken);

    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken);

    Task<IReadOnlyList<int>> GetExistingIdsAsync(IReadOnlyList<int> ids, CancellationToken cancellationToken);

    Task<(IReadOnlyList<RoleListItemDto> Items, int TotalCount)> ListAsync(
        int page, int pageSize, string? search, CancellationToken cancellationToken);

    Task<RoleDto?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<int> AddAsync(CreateRoleData data, CancellationToken cancellationToken);

    Task<bool> UpdateAsync(int id, UpdateRoleData data, CancellationToken cancellationToken);

    Task<bool> SoftDeleteAsync(int id, DateTime utcNow, CancellationToken cancellationToken);
}
