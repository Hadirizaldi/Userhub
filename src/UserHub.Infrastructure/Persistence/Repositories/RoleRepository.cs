using Microsoft.EntityFrameworkCore;
using UserHub.Application.Abstractions.Persistence;
using UserHub.Application.Roles.Queries.LookupRoles;

namespace UserHub.Infrastructure.Persistence.Repositories;

public sealed class RoleRepository(AppDbContext db) : IRoleRepository
{
    public async Task<IReadOnlyList<RoleLookupDto>> GetLookupAsync(CancellationToken cancellationToken)
    {
        return await db.Roles
            .AsNoTracking()
            .OrderBy(r => r.Name)
            .Select(r => new RoleLookupDto(r.Id, r.Name))
            .ToListAsync(cancellationToken);
    }

    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken) =>
        db.Roles.AnyAsync(r => r.Id == id, cancellationToken);
}