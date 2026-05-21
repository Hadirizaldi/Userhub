using UserHub.Application.Roles.Queries.LookupRoles;

namespace UserHub.Application.Abstractions.Persistence;

public interface IRoleRepository
{
    Task<IReadOnlyList<RoleLookupDto>> GetLookupAsync(CancellationToken cancellationToken);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken);
}