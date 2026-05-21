using UserHub.Application.Abstractions.Persistence;

namespace UserHub.Application.ConditionStatuses.Queries.LookupConditionStatuses;

public sealed class LookupConditionStatusService(IConditionStatusRepository conditionStatusRepository)
{
    public Task<IReadOnlyList<ConditionStatusesDto>> HandleAsync(CancellationToken cancellationToken)
        => conditionStatusRepository.GetLookupAsync(cancellationToken);
}