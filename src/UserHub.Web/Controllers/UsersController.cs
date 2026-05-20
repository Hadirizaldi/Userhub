using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserHub.Application.Common.Pagination;
using UserHub.Application.Users.Queries.GetUsers;
using UserHub.Application.Users.Commands.CreateUser;

namespace UserHub.Web.Controllers;

[ApiController]
[Route("v1/users")]
[Tags("Users")]
public sealed class UsersController(ISender sender) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<UserListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        [FromQuery] GetUserQuery query,
        CancellationToken cancellationToken
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(UserListItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create (
        [FromBody] CreateUserCommand command,
        CancellationToken cancellationToken
    )
    {
        var created = await sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id=created.Id }, created);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UserListItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(int id)
    {

        // TODO: implement get user by id
        return NotFound();
    }
}