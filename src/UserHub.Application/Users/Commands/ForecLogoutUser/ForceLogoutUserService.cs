using UserHub.Application.Abstractions.Persistence;
using UserHub.Application.Abstractions.Time;
using UserHub.Domain.Common.Exceptions;

namespace UserHub.Application.Users.Commands.ForceLogoutUser;

public sealed class ForceLogoutUserService(
    IUserRepository userRepository,
    ISessionRepository sessionRepository,
    IClock clock
)
{
    public async Task HandleAsync(int id, CancellationToken cancellationToken)
    {
        var statusId = await userRepository.GetStatusIdAsync(id, cancellationToken);
        if (statusId is null) throw NotFoundException.For("User", id);

        await sessionRepository.RevokeAllForUserAsync(id, clock.UtcNow, cancellationToken);
    }
}