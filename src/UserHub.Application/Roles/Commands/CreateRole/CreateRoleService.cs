using FluentValidation;
using UserHub.Application.Abstractions.Audit;
using UserHub.Application.Abstractions.Persistence;
using UserHub.Application.Abstractions.Time;
using UserHub.Application.AuditLogs;
using UserHub.Application.Roles.Queries.GetRoleById;
using UserHub.Domain.Common;
using UserHub.Domain.Common.Exceptions;

namespace UserHub.Application.Roles.Commands.CreateRole;

public sealed class CreateRoleService(
    IValidator<CreateRoleRequest> validator,
    IRoleRepository roleRepository,
    IClock clock,
    IAuditLogger auditLogger)
{
    public async Task<RoleDto> HandleAsync(
        CreateRoleRequest request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var name = request.Name.Trim();

        if (await roleRepository.ExistsByNameAsync(name, cancellationToken))
        {
            throw new ConflictException(
                ErrorCodes.RoleNameTaken,
                $"A role with name '{name}' already exists.");
        }

        var now = clock.UtcNow;
        var data = new CreateRoleData(name, now, now);
        var id = await roleRepository.AddAsync(data, cancellationToken);

        await auditLogger.LogAsync(
            new AuditEntry("role.create", "role", id), cancellationToken);

        return await roleRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Newly created role not found.");
    }
}
