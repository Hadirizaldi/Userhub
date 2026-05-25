using FluentValidation;
using UserHub.Application.Abstractions.Audit;
using UserHub.Application.Abstractions.Persistence;
using UserHub.Application.Abstractions.Security;
using UserHub.Application.Abstractions.Time;
using UserHub.Application.AuditLogs;
using UserHub.Application.Users.Queries.GetUsers;
using UserHub.Domain.Common;
using UserHub.Domain.Common.Exceptions;
using UserHub.Domain.Users.Policies;

namespace UserHub.Application.Users.Commands.CreateUser;

public sealed class CreateUserService(
    IValidator<CreateUserRequest> validator,
    IUserRepository userRepository,
    INipGenerator nipGenerator,
    IPasswordHasher passwordHasher,
    IReferenceDataCatalog referenceDataCatalog,
    IClock clock,
    PhonePolicy phonePolicy,
    IAuditLogger auditLogger)
{
    public async Task<UserListItemDto> HandleAsync(
        CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var email = request.Email.Trim().ToLowerInvariant();

        if (await userRepository.ExistsByEmailAsync(email, cancellationToken))
        {
            throw new ConflictException(
                ErrorCodes.EmailAlreadyTaken,
                $"A user with email '{email}' already exists.");
        }

        var nip = await nipGenerator.GenerateAsync(cancellationToken);
        var passwordHash = passwordHasher.Hash(request.Password);
        var phone = phonePolicy.Normalize(request.Phone);
        var now = clock.UtcNow;

        var data = new CreateUserData(
            Nip: nip,
            Fullname: request.Fullname.Trim(),
            Email: email,
            PasswordHash: passwordHash,
            Phone: phone,
            StatusId: referenceDataCatalog.ActiveUserStatusId,
            ConditionStatusId: referenceDataCatalog.ActiveConditionStatusId,
            RoleId: referenceDataCatalog.EmployeeRoleId,
            CreatedAt: now,
            UpdatedAt: now);

        var id = await userRepository.AddAsync(data, cancellationToken);

        await auditLogger.LogAsync(
            new AuditEntry("user.create", "user", id), cancellationToken);

        var created = await userRepository.GetByIdAsync(id, cancellationToken);
        return created ?? throw new InvalidOperationException("Newly created user not found.");
    }
}
