using MediatR;
using UserHub.Application.Abstractions.Persistence;
using UserHub.Application.Common.Pagination;

namespace UserHub.Application.Users.Queries.GetUsers;

public sealed class GetUsersHandler(IUserRepository userRepository) : IRequestHandler<GetUserQuery, PagedResult<UserListItemDto>>
{
    public async Task <PagedResult<UserListItemDto>> Handle(
        GetUserQuery request,
        CancellationToken cancellationToken )
    {
        var (items, total) = await userRepository.ListAsync(
            request.Page,
            request.PageSize,
            request.Search,
            cancellationToken
        );

        return new PagedResult<UserListItemDto>(items, request.Page, request.PageSize, total);
    }
}