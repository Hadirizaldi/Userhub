using FluentValidation;
using Microsoft.Extensions.Options;
using UserHub.Application.Abstractions.Auth;
using UserHub.Application.Abstractions.Persistence;
using UserHub.Application.Abstractions.Time;
using UserHub.Application.Auth;
using UserHub.Domain.Common.Exceptions;

namespace UserHub.Application.Auth.Commands.Refresh;

public sealed class RefreshTokenService(
    IValidator<RefreshRequest> validator,
    ISessionRepository sessionRepository,
    IJwtService jwtService,
    IRefreshTokenGenerator refreshTokenGenerator,
    IClock clock,
    IOptions<JwtOptions> jwtOptions)
{
    private readonly JwtOptions _jwt = jwtOptions.Value;

    public async Task<RefreshResult> HandleAsync(
        RefreshRequest request,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var hash = refreshTokenGenerator.Hash(request.RefreshToken);

        var lookup = await sessionRepository.GetByRefreshTokenHashAsync(hash, cancellationToken);
        if (lookup is null)
            throw new UnauthorizedException("INVALID_REFRESH_TOKEN", "Invalid refresh token.");

        var now = clock.UtcNow;

        if(lookup.TokenRevokedAt is not null)
        {
            await sessionRepository.RevokeAllForUserAsync(lookup.UserId, now, cancellationToken);
            throw new UnauthorizedException("INVALID_REFRESH_TOKEN", "Refresh token reuse detected. All sessions revoked.");
        }

        if(lookup.SessionRevokedAt is not null)
        {
            throw new UnauthorizedException("INVALID_REFRESH_TOKEN", "Session has been revoked.");
        }

        if(lookup.TokenExpiresAt <= now)
        {
            throw new UnauthorizedException("REFRESH_TOKEN_EXPIRED", "Refresh token has expired.");
        }

        var (newPlaintext, newHash) = refreshTokenGenerator.Generate();

        var rotateData = new RotateRefreshTokenData
        (
            OldRefreshTokenId: lookup.RefreshTokenId,
            SessionId: lookup.SessionId,
            NewRefreshTokenHash: newHash,
            NewRefreshTokenExpiresAt: now.AddDays(_jwt.RefreshTokenDays),
            UtcNow: now
        );

        await sessionRepository.RotateRefreshTokenAsync(rotateData, cancellationToken);
    
        var accessToken = jwtService.GenerateAccessToken(lookup.UserId, lookup.Email, lookup.RoleName);
        return new RefreshResult(
            AccessToken: accessToken,
            RefreshToken: newPlaintext,
            ExpiresIn: _jwt.AccessTokenMinutes * 60
            );
    }
}