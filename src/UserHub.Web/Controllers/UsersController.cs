using Microsoft.AspNetCore.Mvc;
using UserHub.Application.Common.Pagination;
using UserHub.Application.Users.Commands.CreateUser;
using UserHub.Application.Users.Queries.GetUsers;
using UserHub.Application.Users.Queries.GetUserById;
using UserHub.Application.Users.Commands.UpdateUser;

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
}
