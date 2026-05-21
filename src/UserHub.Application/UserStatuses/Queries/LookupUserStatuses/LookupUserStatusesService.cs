using UserHub.Application.Abstractions.Persistence;

namespace UserHub.Application.UserStatuses.Queries.LookupUserStatuses;

public sealed class LookupUserStatusesService(IUserStatusRepository userStatusRepository)
{
    public Task<IReadOnlyList<UserStatusesDto>> HandleAsync(CancellationToken cancellationToken)
        => userStatusRepository.GetLookupAsync(cancellationToken);
}
