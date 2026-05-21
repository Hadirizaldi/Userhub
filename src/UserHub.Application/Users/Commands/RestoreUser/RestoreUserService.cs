using UserHub.Application.Abstractions.Persistence;
using UserHub.Application.Abstractions.Time;
using UserHub.Application.Users.Queries.GetUsers;
using UserHub.Domain.Common.Exceptions;

namespace UserHub.Application.Users.Commands.RestoreUser;

public sealed class RestoreUserService(
    IUserRepository userRepository,
    IClock clock)
{
    public async Task<UserListItemDto> HandleAsync(int id, CancellationToken cancellationToken)
    {
        var success = await userRepository.RestoreAsync(id, clock.UtcNow, cancellationToken);
        if (!success) throw NotFoundException.For("DeletedUser", id);

        return await userRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("User not found after restore.");
    }
}
