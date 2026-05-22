using UserHub.Application.Abstractions.Persistence;
using UserHub.Application.Abstractions.Time;
using UserHub.Domain.Common;
using UserHub.Domain.Common.Exceptions;
using UserHub.Domain.Roles.Policies;

namespace UserHub.Application.Roles.Commands.DeleteRole;

public sealed class DeleteRoleService(
    IRoleRepository roleRepository,
    IUserRepository userRepository,
    IClock clock,
    RoleProtectionPolicy roleProtectionPolicy)
{
    public async Task HandleAsync(int id, CancellationToken cancellationToken)
    {
        var existing = await roleRepository.GetByIdAsync(id, cancellationToken)
            ?? throw NotFoundException.For("Role", id);

        roleProtectionPolicy.EnsureMutable(existing.IsSystem);

        var userCount = await userRepository.CountByRoleAsync(id, cancellationToken);
        if (userCount > 0)
        {
            throw new ConflictException(
                ErrorCodes.RoleInUse,
                $"Cannot delete role: {userCount} user(s) still assigned. Reassign them first.");
        }

        var success = await roleRepository.SoftDeleteAsync(id, clock.UtcNow, cancellationToken);
        if (!success) throw NotFoundException.For("Role", id);
    }
}
