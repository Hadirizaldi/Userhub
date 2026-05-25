using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserHub.Application.Common.Pagination;
using UserHub.Application.Users.Commands.CreateUser;
using UserHub.Application.Users.Queries.GetUsers;
using UserHub.Application.Users.Queries.GetUserById;
using UserHub.Application.Users.Commands.UpdateUser;
using UserHub.Application.Users.Commands.ChangeUserRole;
using UserHub.Application.Users.Commands.ChangeUserStatus;
using UserHub.Application.Users.Commands.ChangeUserPassword;
using UserHub.Application.Users.Commands.DeleteUser;
using UserHub.Application.Users.Commands.RestoreUser;
using UserHub.Application.Users.Commands.BulkAssignRoles;
using UserHub.Web.Auth;
using UserHub.Application.Users.Queries.GetUserActivity;

namespace UserHub.Web.Controllers;

[ApiController]
[Route("api/v1/users")]
[Tags("Users")]
public sealed class UsersController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<UserListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        [FromQuery] GetUsersRequest request,
        [FromServices] GetUsersService service,
        CancellationToken cancellationToken)
    {
        var result = await service.HandleAsync(request, cancellationToken);
        return Ok(result);
    }

    [Authorize(Policy = AuthPolicies.AdminOnly)]
    [HttpPost]
    [ProducesResponseType(typeof(UserListItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateUserRequest request,
        [FromServices] CreateUserService service,
        CancellationToken cancellationToken)
    {
        var created = await service.HandleAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UserListItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        int id,
        [FromServices] GetUserByIdService service,
        CancellationToken cancellationToken)
    {
        var result = await service.HandleAsync(new GetUserByIdRequest(id), cancellationToken);

        return Ok(result);
    }

    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(UserListItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateUserRequest request,
        [FromServices] UpdateUserService service,
        CancellationToken cancellationToken)
    {
        var result = await service.HandleAsync(id, request, cancellationToken);
        return Ok(result);
    }

    [Authorize(Policy = AuthPolicies.AdminOnly)]
    [HttpPatch("{id:int}/role")]
    [ProducesResponseType(typeof(UserListItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeRole(
        int id,
        [FromBody] ChangeUserRoleRequest request,
        [FromServices] ChangeUserRoleService service,
        CancellationToken cancellationToken
    )
    {
        var result = await service.HandleAsync(id, request, cancellationToken);
        return Ok(result);
    }

    [Authorize(Policy = AuthPolicies.AdminOnly)]
    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(typeof(UserListItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeStatus(
        int id,
        [FromBody] ChangeUserStatusRequest request,
        [FromServices] ChangeUserStatusService service,
        CancellationToken cancellationToken)
    {
        var result = await service.HandleAsync(id, request, cancellationToken);
        return Ok(result);
    }

    [Authorize(Policy = AuthPolicies.AdminOnly)]
    [HttpPost("{id:int}/password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangePassword(
        int id,
        [FromBody] ChangeUserPasswordRequest request,
        [FromServices] ChangeUserPasswordService service,
        CancellationToken cancellationToken)
    {
        await service.HandleAsync(id, request, cancellationToken);
        return NoContent();
    }

    [Authorize(Policy = AuthPolicies.AdminOnly)]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        int id,
        [FromServices] DeleteUserService service,
        CancellationToken cancellationToken)
    {
        await service.HandleAsync(id, cancellationToken);
        return NoContent();
    }

    [Authorize(Policy = AuthPolicies.AdminOnly)]
    [HttpPost("{id:int}/restore")]
    [ProducesResponseType(typeof(UserListItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Restore(
        int id,
        [FromServices] RestoreUserService service,
        CancellationToken cancellationToken)
    {
        var result = await service.HandleAsync(id, cancellationToken);
        return Ok(result);
    }

    [Authorize(Policy = AuthPolicies.AdminOnly)]
    [HttpPost("role-assignments")]
    [ProducesResponseType(typeof(BulkAssignRolesResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> BulkAssignRoles(
        [FromBody] BulkAssignRolesRequest request,
        [FromServices] BulkAssignRolesService service,
        CancellationToken cancellationToken)
    {
        var result = await service.HandleAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("activity")]
    [ProducesResponseType(typeof(PagedResult<UserActivityDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Activity(
        [FromQuery] GetUserActivityRequest request,
        [FromServices] GetUserActivityService service,
        CancellationToken cancellationToken)
    {   
        var result = await service.HandleAsync(request, cancellationToken);
        return Ok(result);
    }
}
