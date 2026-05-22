using FluentValidation;
using UserHub.Application.Abstractions.Auth;
using UserHub.Application.Abstractions.Persistence;
using UserHub.Application.Abstractions.Time;
using UserHub.Application.Users.Queries.GetUsers;
using UserHub.Domain.Common.Exceptions;
using UserHub.Domain.Users.Policies;

namespace UserHub.Application.Users.Commands.UpdateUser;

public sealed class UpdateUserService (
    IValidator<UpdateUserRequest> validator,
    IUserRepository userRepository,
    IClock clock,
    PhonePolicy phonePolicy,
    ICurrentUserAccessor currentUser
)
{
    public async Task<UserListItemDto> HandleAsync(
        int id,
        UpdateUserRequest request,
        CancellationToken cancellationToken
    )
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        if (currentUser.UserId != id && !currentUser.IsAdmin)
            throw new ForbiddenException("FORBIDDEN", "You can only update your own profile.");

        var data = new UpdateUserData(
            Fullname: request.Fullname.Trim(),
            Phone: phonePolicy.Normalize(request.Phone),
            UpdatedAt: clock.UtcNow
        );

        var updated = await userRepository.UpdateAsync(id, data, cancellationToken);
        if (!updated) throw NotFoundException.For("User", id);

        var dto = await userRepository.GetByIdAsync(id, cancellationToken);
        return dto ?? throw new InvalidOperationException("User updated but not found.");
    }
}