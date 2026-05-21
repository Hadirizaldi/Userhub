using Microsoft.EntityFrameworkCore;
using UserHub.Application.Abstractions.Persistence;
using UserHub.Application.UserStatuses.Queries.LookupUserStatuses;

namespace UserHub.Infrastructure.Persistence.Repositories;

public sealed class UserStatusRepository(AppDbContext db) : IUserStatusRepository
{
    public async Task<IReadOnlyList<UserStatusesDto>> GetLookupAsync(CancellationToken cancellationToken)
    {
        return await db.UserStatuses
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .Select(s => new UserStatusesDto(s.Id, s.Name))
            .ToListAsync(cancellationToken);
    }

    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken) =>
        db.UserStatuses.AnyAsync(s => s.Id == id, cancellationToken);
}
