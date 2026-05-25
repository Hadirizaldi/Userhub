using FluentValidation;
using UserHub.Application.Abstractions.Persistence;
using UserHub.Application.Common.Pagination;

namespace UserHub.Application.Users.Queries.GetUserActivity;

public sealed class GetUserActivityService(
    IValidator<GetUserActivityRequest> validator,
    IUserRepository userRepository
)
{
    public async Task<PagedResult<UserActivityDto>> HandleAsync(
        GetUserActivityRequest request,
        CancellationToken cancellationToken
    )
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var (items, total) = await userRepository.ListActivityAsync(
            request.Page,
            request.PageSize,
            request.Search,
            request.IsLoggedIn,
            cancellationToken
        );

        return new PagedResult<UserActivityDto>(items, request.Page, request.PageSize, total);}
}
