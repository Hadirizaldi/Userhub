using FluentValidation;
using UserHub.Application.Abstractions.Persistence;
using UserHub.Application.Abstractions.Time;
using UserHub.Application.Users.Queries.GetUsers;
using UserHub.Domain.Common.Exceptions;

namespace UserHub.Application.Users.Commands.ChangeUserStatus;

public sealed class ChangeUserStatusService(
    IValidator<ChangeUserStatusRequest> validator,
    IUserRepository userRepository,
    IUserStatusRepository userStatusRepository,
    ISessionRepository sessionRepository,
    IReferenceDataCatalog referenceDataCatalog,
    IClock clock)
{
    public async Task<UserListItemDto> HandleAsync(
        int id, ChangeUserStatusRequest request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        if (!await userStatusRepository.ExistsAsync(request.StatusId, cancellationToken))
            throw NotFoundException.For("UserStatus", request.StatusId);

        var data = new ChangeUserStatusData(request.StatusId, clock.UtcNow);
        var success = await userRepository.ChangeStatusAsync(id, data, cancellationToken);
        if (!success) throw NotFoundException.For("User", id);
        if (request.StatusId != referenceDataCatalog.ActiveUserStatusId)
            await sessionRepository.RevokeAllForUserAsync(id, clock.UtcNow, cancellationToken);

        return await userRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("User not found after status change.");
    }
}
