using UserHub.Application.Abstractions.Persistence;
using UserHub.Application.Abstractions.Time;
using UserHub.Domain.Common.Exceptions;

namespace UserHub.Application.Users.Commands.DeleteUser;

public sealed class DeleteUserService(
    IUserRepository userRepository,
    ISessionRepository sessionRepository,
    IClock clock)
{
    public async Task HandleAsync(int id, CancellationToken cancellationToken)
    {
        var success = await userRepository.SoftDeleteAsync(id, clock.UtcNow, cancellationToken);
        if (!success) throw NotFoundException.For("User", id);

        await sessionRepository.RevokeAllForUserAsync(id, clock.UtcNow, cancellationToken);
    }
}
