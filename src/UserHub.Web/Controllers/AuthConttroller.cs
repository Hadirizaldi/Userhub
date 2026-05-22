using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserHub.Application.Auth.Commands.Login;
using UserHub.Application.Auth.Commands.Logout;
using UserHub.Application.Auth.Commands.LogoutAll;
using UserHub.Application.Auth.Commands.Refresh;
using UserHub.Application.Auth.Queries.GetMe;
using UserHub.Application.Auth.Queries.GetSessions;
using UserHub.Application.Users.Queries.GetUsers;

namespace UserHub.Web.Controllers;

[ApiController]
[Route("api/v1/auth")]
[Tags("Auth")]
public sealed class AuthController : ControllerBase
{
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        [FromServices] LoginService service,
        CancellationToken cancellationToken
    )
    {
        var result = await service.HandleAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("refresh")]
    [ProducesResponseType(typeof(RefreshResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshRequest request,
        [FromServices] RefreshTokenService service,
        CancellationToken cancellationToken)
    {
        var result = await service.HandleAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Logout(
        [FromBody] LogoutRequest request,
        [FromServices] LogoutService service,
        CancellationToken cancellationToken)
    {
        await service.HandleAsync(request, cancellationToken);
        return NoContent();
    }

    [Authorize]
    [HttpPost("logout-all")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LogoutAll(
        [FromServices] LogoutAllService service,
        CancellationToken cancellationToken)
    {
        await service.HandleAsync(cancellationToken);
        return NoContent();
    }

    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserListItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Me(
        [FromServices] GetMeService service,
        CancellationToken cancellationToken)
    {
        var result = await service.HandleAsync(cancellationToken);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("sessions")]
    [ProducesResponseType(typeof(IReadOnlyList<SessionListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Sessions(
        [FromServices] GetSessionsService service,
        CancellationToken cancellationToken)
    {
        var result = await service.HandleAsync(cancellationToken);
        return Ok(result);
    }

}