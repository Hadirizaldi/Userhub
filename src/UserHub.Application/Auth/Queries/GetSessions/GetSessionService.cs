using UserHub.Application.Abstractions.Auth;
using UserHub.Application.Abstractions.Persistence;

namespace UserHub.Application.Auth.Queries.GetSessions;

public sealed class GetSessionsService(
    ICurrentUserAccessor currentUser,
    ISessionRepository sessionRepository
)
{
    public Task<IReadOnlyList<SessionListItemDto>> HandleAsync(CancellationToken cancellationToken) =>
        sessionRepository.GetActiveByUserAsync(currentUser.UserId, cancellationToken);
}