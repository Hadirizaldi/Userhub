using UserHub.Application.Abstractions.Auth;
using UserHub.Application.Abstractions.Persistence;
using UserHub.Application.Users.Queries.GetUsers;
using UserHub.Domain.Common.Exceptions;


namespace UserHub.Application.Auth.Queries.GetMe;

public sealed class GetMeService (
    ICurrentUserAccessor currentUser,
    IUserRepository userRepository
)
{
    public async Task<UserListItemDto> HandleAsync(CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(currentUser.UserId, cancellationToken);
        if (user is null) throw NotFoundException.For("User", currentUser.UserId);
        
        return user;
    }
}