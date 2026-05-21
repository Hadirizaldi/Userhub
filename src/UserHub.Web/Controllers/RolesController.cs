using Microsoft.AspNetCore.Mvc;
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
}
