using Microsoft.AspNetCore.Mvc;
using UserHub.Application.Common.Pagination;
using UserHub.Application.Roles.Commands.CreateRole;
using UserHub.Application.Roles.Commands.DeleteRole;
using UserHub.Application.Roles.Commands.UpdateRole;
using UserHub.Application.Roles.Queries.GetRoleById;
using UserHub.Application.Roles.Queries.GetRoles;
using UserHub.Application.Roles.Queries.LookupRoles;

namespace UserHub.Web.Controllers;

[ApiController]
[Route("api/v1/roles")]
[Tags("Roles")]
public sealed class RolesController : ControllerBase
{
    [HttpGet("lookup")]
    [ProducesResponseType(typeof(IReadOnlyList<RoleLookupDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Lookup(
        [FromServices] LookupRolesService service,
        CancellationToken cancellationToken)
    {
        var result = await service.HandleAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<RoleListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        [FromQuery] GetRolesRequest request,
        [FromServices] GetRolesService service,
        CancellationToken cancellationToken)
    {
        var result = await service.HandleAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(RoleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        int id,
        [FromServices] GetRoleByIdService service,
        CancellationToken cancellationToken)
    {
        var result = await service.HandleAsync(id, cancellationToken);
        return Ok(result);
    }

    // TODO: [Authorize(Roles = "admin")] when auth is implemented
    [HttpPost]
    [ProducesResponseType(typeof(RoleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateRoleRequest request,
        [FromServices] CreateRoleService service,
        CancellationToken cancellationToken)
    {
        var created = await service.HandleAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // TODO: [Authorize(Roles = "admin")] when auth is implemented
    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(RoleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateRoleRequest request,
        [FromServices] UpdateRoleService service,
        CancellationToken cancellationToken)
    {
        var result = await service.HandleAsync(id, request, cancellationToken);
        return Ok(result);
    }

    // TODO: [Authorize(Roles = "admin")] when auth is implemented
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(
        int id,
        [FromServices] DeleteRoleService service,
        CancellationToken cancellationToken)
    {
        await service.HandleAsync(id, cancellationToken);
        return NoContent();
    }
}
