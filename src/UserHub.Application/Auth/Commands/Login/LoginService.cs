using FluentValidation;
using UserHub.Application.Abstractions.Audit;
using UserHub.Application.Abstractions.Auth;
using UserHub.Application.Abstractions.Persistence;
using UserHub.Application.Abstractions.Time;
using UserHub.Application.Auth;
using UserHub.Application.AuditLogs;
using UserHub.Domain.Common;
using UserHub.Domain.Common.Exceptions;
using Microsoft.Extensions.Options;
using UserHub.Application.Abstractions.Security;

namespace UserHub.Application.Auth.Commands.Login;

public sealed class LoginService(
    IValidator<LoginRequest> validator,
    IUserRepository userRepository,
    ISessionRepository sessionRepository,
    IJwtService jwtService,
    IPasswordHasher passwordHasher,
    IRefreshTokenGenerator refreshTokenGenerator,
    IClientInfoAccessor clientInfoAccessor,
    IReferenceDataCatalog referenceDataCatalog,
    IClock clock,
    IAuditLogger auditLogger,
    IOptions<JwtOptions> jwtOptions)
{
    private readonly JwtOptions _jwt = jwtOptions.Value;

    public async Task<LoginResult> HandleAsync(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var email = request.Email.Trim().ToLowerInvariant();
        var creds = await userRepository.GetCredentialsByEmailAsync(email, cancellationToken);

        if (creds is null || !passwordHasher.Verify(request.Password, creds.PasswordHash))
        {
            await auditLogger.LogAsync(
                new AuditEntry("auth.login_failed", "user", creds?.Id,
                    new { email, reason = "invalid_credentials" }),
                cancellationToken);
            throw new UnauthorizedException(ErrorCodes.InvalidCredentials, "Invalid email or password.");
        }

        if (creds.StatusId != referenceDataCatalog.ActiveUserStatusId)
        {
            await auditLogger.LogAsync(
                new AuditEntry("auth.login_failed", "user", creds.Id,
                    new { email, reason = "inactive" }),
                cancellationToken);
            throw new UnauthorizedException(ErrorCodes.UserInactive, "Account is not active.");
        }

        var (plaintext, hash) = refreshTokenGenerator.Generate();
        var now = clock.UtcNow;
        var info = clientInfoAccessor.Current;

        var data = new CreateLoginData
        (
            UserId : creds.Id,
            IpAddress : info.IpAddress,
            UserAgent : info.UserAgent,
            RefreshTokenHash : hash,
            RefreshTokenExpiresAt : now.AddDays(_jwt.RefreshTokenDays),
            UtcNow : now
        );

        await sessionRepository.CreateLoginAsync(data, cancellationToken);

        var accessToken = jwtService.GenerateAccessToken(creds.Id, email, creds.RoleName);

        return new LoginResult
        (
            AccessToken : accessToken,
            RefreshToken : plaintext,
            ExpiresIn : (_jwt.AccessTokenMinutes * 60)
        );
    }
}