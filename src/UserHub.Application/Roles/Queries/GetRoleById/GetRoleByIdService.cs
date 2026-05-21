using UserHub.Application.Abstractions.Persistence;
using UserHub.Domain.Common.Exceptions;

namespace UserHub.Application.Roles.Queries.GetRoleById;

public sealed class GetRoleByIdService(IRoleRepository roleRepository)
{
    public async Task<RoleDto> HandleAsync(int id, CancellationToken cancellationToken)
    {
        return await roleRepository.GetByIdAsync(id, cancellationToken)
            ?? throw NotFoundException.For("Role", id);
    }
}
