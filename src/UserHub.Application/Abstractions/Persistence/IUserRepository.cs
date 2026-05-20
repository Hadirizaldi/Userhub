using UserHub.Application.Users.Queries.GetUsers;

namespace UserHub.Application.Abstractions.Persistence;

public interface IUserRepository
{
    Task<(IReadOnlyList<UserListItemDto> Items, int TotalCount)> ListAsync(
        int Page,
        int PageSize,
        string? Search,
        CancellationToken cancellationToken );
}