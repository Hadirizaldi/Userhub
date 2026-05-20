using FluentValidation;
using UserHub.Application.Abstractions.Persistence;
using UserHub.Application.Common.Pagination;

namespace UserHub.Application.Users.Queries.GetUsers;

public sealed class GetUsersService(
    IValidator<GetUsersRequest> validator,
    IUserRepository userRepository)
{
    public async Task<PagedResult<UserListItemDto>> HandleAsync(
        GetUsersRequest request,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var (items, total) = await userRepository.ListAsync(
            request.Page,
            request.PageSize,
            request.Search,
            cancellationToken);

        return new PagedResult<UserListItemDto>(items, request.Page, request.PageSize, total);
    }
}
