using FluentValidation;
using UserHub.Application.Abstractions.Persistence;
using UserHub.Application.Abstractions.Time;
using UserHub.Application.Roles.Queries.GetRoleById;
using UserHub.Domain.Common;
using UserHub.Domain.Common.Exceptions;
using UserHub.Domain.Roles.Policies;

namespace UserHub.Application.Roles.Commands.UpdateRole;

public sealed class UpdateRoleService(
    IValidator<UpdateRoleRequest> validator,
    IRoleRepository roleRepository,
    IClock clock,
    RoleProtectionPolicy roleProtectionPolicy)
{
    public async Task<RoleDto> HandleAsync(
        int id, UpdateRoleRequest request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var existing = await roleRepository.GetByIdAsync(id, cancellationToken)
            ?? throw NotFoundException.For("Role", id);

        roleProtectionPolicy.EnsureMutable(existing.IsSystem);

        var name = request.Name.Trim();

        if (!string.Equals(existing.Name, name, StringComparison.OrdinalIgnoreCase)
            && await roleRepository.ExistsByNameAsync(name, cancellationToken))
        {
            throw new ConflictException(
                ErrorCodes.RoleNameTaken,
                $"A role with name '{name}' already exists.");
        }

        var data = new UpdateRoleData(name, clock.UtcNow);
        var success = await roleRepository.UpdateAsync(id, data, cancellationToken);
        if (!success) throw NotFoundException.For("Role", id);

        return await roleRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Role not found after update.");
    }
}
