using UserHub.Application.UserStatuses.Queries.LookupUserStatuses;

namespace UserHub.Application.Abstractions.Persistence;

public interface IUserStatusRepository
{
    Task<IReadOnlyList<UserStatusesDto>> GetLookupAsync(CancellationToken cancellationToken);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken);
}
