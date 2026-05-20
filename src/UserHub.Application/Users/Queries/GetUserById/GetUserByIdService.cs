using UserHub.Application.Users.Queries.GetUsers;
using UserHub.Application.Abstractions.Persistence;
using UserHub.Domain.Common.Exceptions;

namespace UserHub.Application.Users.Queries.GetUserById;

public sealed class GetUserByIdService(IUserRepository userRepository)
{
    public async Task<UserListItemDto> HandleAsync(
        GetUserByIdRequest request,
        CancellationToken cancellationToken )
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken);
        return user ?? throw NotFoundException.For("User", request.Id);
    }
}