using MediatR;
using UserHub.Application.Abstractions.Persistence;
using UserHub.Application.Abstractions.Security;
using UserHub.Application.Abstractions.Time;
using UserHub.Application.Users.Queries.GetUsers;
using UserHub.Domain.Common.Exceptions;
using UserHub.Domain.Users.Policies;

namespace UserHub.Application.Users.Commands.CreateUser;

public sealed class CreateUserHandler(
    IUserRepository userRepository,
    INipGenerator nipGenerator,
    IPasswordHasher passwordHasher,
    IReferenceDataCatalog referenceDataCatalog,
    IClock clock,
    PhonePolicy phonePolicy) : IRequestHandler<CreateUserCommand, UserListItemDto>
{
    public async Task<UserListItemDto> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var email = command.Email.Trim().ToLowerInvariant();

        if (await userRepository.ExistsByEmailAsync(email, cancellationToken))
        {
            throw new ConflictException("EMAIL_ALREADY_TAKEN", $"User with email '{email}' already exists.");
        }

        var nip = await nipGenerator.GenerateAsync(cancellationToken);
        var passwordHash = passwordHasher.Hash(command.Password);
        var phone = phonePolicy.Normalize(command.Phone);

        var data = new CreateUserData(
            Nip: nip,
            Fullname: command.Fullname.Trim(),
            Email: email,
            PasswordHash: passwordHash,
            Phone: phone,
            StatusId: referenceDataCatalog.ActiveUserStatusId,
            ConditionStatusId: referenceDataCatalog.ActiveConditionStatusId,
            CreatedAt: clock.UtcNow,
            UpdatedAt: clock.UtcNow
        );

        var id = await userRepository.AddAsync(data, cancellationToken);

        var created = await userRepository.GetByIdAsync(id, cancellationToken);

        return created ?? throw new InvalidOperationException("newly created user not found");
    }
}