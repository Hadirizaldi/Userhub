using Microsoft.EntityFrameworkCore;
using UserHub.Application.Abstractions.Persistence;
using UserHub.Application.ConditionStatuses.Queries.LookupConditionStatuses;

namespace UserHub.Infrastructure.Persistence.Repositories;

public sealed class ConditionStatusRepository(AppDbContext db) : IConditionStatusRepository
{
    public async Task<IReadOnlyList<ConditionStatusesDto>> GetLookupAsync(CancellationToken cancellationToken)
    {
        return await db.ConditionStatuses
            .AsNoTracking()
            .OrderBy(r => r.Name)
            .Select(r => new ConditionStatusesDto(r.Id, r.Name))
            .ToListAsync(cancellationToken);
    }
}

