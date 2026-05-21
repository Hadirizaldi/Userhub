using UserHub.Application.ConditionStatuses.Queries.LookupConditionStatuses;

namespace UserHub.Application.Abstractions.Persistence;

public interface IConditionStatusRepository
{
    Task<IReadOnlyList<ConditionStatusesDto>> GetLookupAsync(CancellationToken cancellationToken); 
}