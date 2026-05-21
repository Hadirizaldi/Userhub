using Microsoft.EntityFrameworkCore;
using UserHub.Application.Abstractions.Persistence;
using UserHub.Application.Users.Commands.ChangeUserRole;
using UserHub.Application.Users.Commands.ChangeUserStatus;
using UserHub.Application.Users.Commands.ChangeUserPassword;
using UserHub.Application.Users.Commands.CreateUser;
using UserHub.Application.Users.Commands.UpdateUser;
using UserHub.Application.Users.Queries.GetUsers;
using UserHub.Infrastructure.Persistence.Entities;

namespace UserHub.Infrastructure.Persistence.Repositories;

public sealed class UserRepository(AppDbContext db) : IUserRepository
{
    public async Task<(IReadOnlyList<UserListItemDto> Items, int TotalCount)> ListAsync(
        int page,
        int pageSize,
        string? search,
        CancellationToken cancellationToken )
    {
        var query = db.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = $"%{search.Trim()}%";
            query = query.Where(u =>
                EF.Functions.ILike(u.Fullname, s) ||
                EF.Functions.ILike(u.Email, s) ||
                EF.Functions.ILike(u.Nip, s)
            );
        }

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new UserListItemDto(
                u.Id,
                u.Nip,
                u.Fullname,
                u.Email,
                u.Phone,
                u.StatusId,
                u.Status.Name,
                u.ConditionStatusId,
                u.ConditionStatus.Name,
                u.Role.Select(r => (int?)r.Id).FirstOrDefault(),
                u.Role.Select(r => r.Name).FirstOrDefault(),
                u.CreatedAt
            ))
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    // Email uniqueness must include soft-deleted users (decision: deleted email cannot be reused).
    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken) =>
        db.Users.IgnoreQueryFilters().AnyAsync(u => u.Email == email.ToLower(), cancellationToken);

    public async Task<int> AddAsync(CreateUserData data, CancellationToken cancellationToken)
    {

        var role = await db.Roles.FindAsync([data.RoleId], cancellationToken)
            ?? throw new InvalidOperationException($"Role with id {data.RoleId} not found.");

        var entity = new Users
        {
            Nip = data.Nip,
            Fullname = data.Fullname,
            Email = data.Email,
            Password = data.PasswordHash,
            Phone = data.Phone,
            StatusId = data.StatusId,
            ConditionStatusId = data.ConditionStatusId,
            CreatedAt = data.CreatedAt,
            UpdatedAt = data.UpdatedAt
        };

        entity.Role.Add(role);
        db.Users.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    public Task<UserListItemDto?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        db.Users
            .AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new UserListItemDto(
                u.Id,
                u.Nip,
                u.Fullname,
                u.Email,
                u.Phone,
                u.StatusId,
                u.Status.Name,
                u.ConditionStatusId,
                u.ConditionStatus.Name,
                u.Role.Select(r => (int?)r.Id).FirstOrDefault(),
                u.Role.Select(r => r.Name).FirstOrDefault(),
                u.CreatedAt
            ))
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<bool> UpdateAsync(int id, UpdateUserData data, CancellationToken cancellationToken)
    {
        var entity = await db.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (entity is null) return false;

        entity.Fullname = data.Fullname;
        entity.Phone = data.Phone;
        entity.UpdatedAt = data.UpdatedAt;

        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public Task<int?> GetStatusIdAsync(int userId, CancellationToken cancellationToken) =>
        db.Users.AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => (int?) u.StatusId)
            .FirstOrDefaultAsync(cancellationToken);


    public async Task<bool> ChangeRoleAsync(
        int userId, ChangeUserRoleData data, CancellationToken cancellationToken)
    {
        var entity = await db.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if(entity is null) return false;

        var role = await db.Roles.FindAsync([data.RoleId], cancellationToken);
        if(role is null) return false;

        var existingRole = entity.Role.ToList();
        if( existingRole.Count == 1 && existingRole[0].Id == data.RoleId) return true;

        entity.Role.Clear();
        entity.Role.Add(role);
        entity.UpdatedAt = data.UpdatedAt;

        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ChangeStatusAsync(
        int userId, ChangeUserStatusData data, CancellationToken cancellationToken)
    {
        var entity = await db.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (entity is null) return false;

        if (entity.StatusId == data.StatusId) return true;

        entity.StatusId = data.StatusId;
        entity.UpdatedAt = data.UpdatedAt;

        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ChangePasswordAsync(
        int userId, ChangeUserPasswordData data, CancellationToken cancellationToken)
    {
        var entity = await db.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (entity is null) return false;

        entity.Password = data.PasswordHash;
        entity.UpdatedAt = data.UpdatedAt;

        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> SoftDeleteAsync(int userId, DateTime utcNow, CancellationToken cancellationToken)
    {
        var entity = await db.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (entity is null) return false;

        entity.DeletedAt = utcNow;
        entity.UpdatedAt = utcNow;

        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> RestoreAsync(int userId, DateTime utcNow, CancellationToken cancellationToken)
    {
        var entity = await db.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == userId && u.DeletedAt != null, cancellationToken);
        if (entity is null) return false;

        entity.DeletedAt = null;
        entity.UpdatedAt = utcNow;

        await db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
