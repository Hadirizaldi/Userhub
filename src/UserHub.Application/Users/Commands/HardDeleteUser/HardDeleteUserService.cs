using UserHub.Application.Abstractions.Persistence;
using UserHub.Domain.Common.Exceptions;
using UserHub.Domain.Roles;
using UserHub.Domain.Users.Policies;

namespace UserHub.Application.Users.Commands.HardDeleteUser;

public sealed class HardDeleteUserService(
    IUserRepository userRepository,
    AdminProtectionPolicy adminProtectionPolicy)
{
    public async Task HandleAsync(int id, CancellationToken cancellationToken)
    {
        var info = await userRepository.GetForHardDeleteAsync(id, cancellationToken)
            ?? throw NotFoundException.For("User", id);

        if (!info.IsDeleted && info.RoleName == RoleNames.Admin)
        {
            var activeAdmins = await userRepository.CountActiveByRoleNameAsync(
                RoleNames.Admin, cancellationToken);
            adminProtectionPolicy.EnsureNotLastAdmin(activeAdmins);
        }

        await userRepository.HardDeleteAsync(id, cancellationToken);
    }
}