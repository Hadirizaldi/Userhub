using Microsoft.AspNetCore.Mvc;
using UserHub.Application.UserStatuses.Queries.LookupUserStatuses;

namespace UserHub.Web.Controllers;

[ApiController]
[Route("api/v1/user-statuses")]
[Tags("UserStatuses")]
public sealed class UserStatusesController : ControllerBase
{
    [HttpGet("lookup")]
    [ProducesResponseType(typeof(IReadOnlyList<UserStatusesDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Lookup(
        [FromServices] LookupUserStatusesService service,
        CancellationToken cancellationToken)
    {
        var result = await service.HandleAsync(cancellationToken);
        return Ok(result);
    }
}
