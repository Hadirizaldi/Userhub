using FluentValidation;
using UserHub.Application.Abstractions.Persistence;
using UserHub.Application.Abstractions.Security;
using UserHub.Application.Abstractions.Time;
using UserHub.Domain.Common.Exceptions;

namespace UserHub.Application.Users.Commands.ChangeUserPassword;

public sealed class ChangeUserPasswordService(
    IValidator<ChangeUserPasswordRequest> validator,
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ISessionRepository sessionRepository,
    IClock clock)
{
    public async Task HandleAsync(
        int id, ChangeUserPasswordRequest request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var data = new ChangeUserPasswordData(
            PasswordHash: passwordHasher.Hash(request.NewPassword),
            UpdatedAt: clock.UtcNow);

        var success = await userRepository.ChangePasswordAsync(id, data, cancellationToken);
        if (!success) throw NotFoundException.For("User", id);

        await sessionRepository.RevokeAllForUserAsync(id, clock.UtcNow, cancellationToken);

    }
}
