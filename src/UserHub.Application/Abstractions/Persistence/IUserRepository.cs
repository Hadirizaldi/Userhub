using UserHub.Application.Users.Queries.GetUsers;
using UserHub.Application.Users.Commands.CreateUser;

namespace UserHub.Application.Abstractions.Persistence;

public interface IUserRepository
{
    Task<(IReadOnlyList<UserListItemDto> Items, int TotalCount)> ListAsync(
        int Page,
        int PageSize,
        string? Search,
        CancellationToken cancellationToken );

    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken);

    Task<int> AddAsync(CreateUserData data, CancellationToken cancellationToken);

    Task<UserListItemDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
}