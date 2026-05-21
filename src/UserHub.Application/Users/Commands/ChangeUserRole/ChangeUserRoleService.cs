using FluentValidation;
using UserHub.Application.Abstractions.Persistence;
using UserHub.Application.Abstractions.Time;
using UserHub.Application.Users.Queries.GetUsers;
using UserHub.Domain.Common.Exceptions;
using UserHub.Domain.Users.Policies;

namespace UserHub.Application.Users.Commands.ChangeUserRole;


public sealed class ChangeUserRoleService(
    IValidator<ChangeUserRoleRequest> validator,
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IReferenceDataCatalog referenceDataCatalog,
    IClock clock,
    RoleChangePolicy roleChangePolicy )
{
    public async Task<UserListItemDto> HandleAsync(
        int id, ChangeUserRoleRequest request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var statusId = await userRepository.GetStatusIdAsync(id, cancellationToken);
        if (statusId is null) throw NotFoundException.For("User", id);

        roleChangePolicy.EnsureCanChange(statusId.Value, referenceDataCatalog.ActiveUserStatusId);

        if (!await roleRepository.ExistsAsync(request.RoleId, cancellationToken))
            throw NotFoundException.For("Role", request.RoleId);

        var data = new ChangeUserRoleData(
            RoleId: request.RoleId,
            UpdatedAt: clock.UtcNow
        );

        var success = await userRepository.ChangeRoleAsync(id, data, cancellationToken);
        if (!success) throw NotFoundException.For("User", id);

        return await userRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("User not found after role change.");
    }
}