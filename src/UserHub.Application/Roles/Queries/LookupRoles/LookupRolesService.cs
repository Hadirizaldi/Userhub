using UserHub.Application.Abstractions.Persistence;

namespace UserHub.Application.Roles.Queries.LookupRoles;

public sealed class LookupRolesService (IRoleRepository roleRepository)
{
    public Task<IReadOnlyList<RoleLookupDto>> HandleAsync(CancellationToken cancellationToken)
        => roleRepository.GetLookupAsync(cancellationToken);
}