using FluentValidation;
using UserHub.Application.Abstractions.Persistence;
using UserHub.Application.Abstractions.Time;
using UserHub.Domain.Common;
using UserHub.Domain.Common.Exceptions;
using UserHub.Domain.Users.Policies;

namespace UserHub.Application.Users.Commands.BulkAssignRoles;

public sealed class BulkAssignRolesService(
    IValidator<BulkAssignRolesRequest> validator,
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IReferenceDataCatalog referenceDataCatalog,
    IClock clock,
    RoleChangePolicy roleChangePolicy)
{
    public async Task<BulkAssignRolesResult> HandleAsync(
        BulkAssignRolesRequest request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var userIds = request.Assignments.Select(a => a.UserId).ToList();
        var roleIds = request.Assignments.Select(a => a.RoleId).Distinct().ToList();

        var userStatuses = await userRepository.GetStatusByIdsAsync(userIds, cancellationToken);
        var missingUserIds = userIds.Where(id => !userStatuses.ContainsKey(id)).ToList();
        if (missingUserIds.Count > 0)
        {
            throw new NotFoundException(
                ErrorCodes.UsersNotFound,
                $"User(s) not found: [{string.Join(", ", missingUserIds)}].");
        }

        var existingRoleIds = await roleRepository.GetExistingIdsAsync(roleIds, cancellationToken);
        var missingRoleIds = roleIds.Except(existingRoleIds).ToList();
        if (missingRoleIds.Count > 0)
        {
            throw new NotFoundException(
                ErrorCodes.RolesNotFound,
                $"Role(s) not found: [{string.Join(", ", missingRoleIds)}].");
        }

        var activeStatusId = referenceDataCatalog.ActiveUserStatusId;
        foreach (var (userId, statusId) in userStatuses)
        {
            roleChangePolicy.EnsureCanChange(statusId, activeStatusId);
        }

        var updatedCount = await userRepository.BulkAssignRolesAsync(
            request.Assignments.Select(a => (a.UserId, a.RoleId)).ToList(),
            clock.UtcNow,
            cancellationToken);

        return new BulkAssignRolesResult(updatedCount);
    }
}
