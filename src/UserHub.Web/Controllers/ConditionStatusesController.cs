using Microsoft.AspNetCore.Mvc;
using UserHub.Application.ConditionStatuses.Queries.LookupConditionStatuses;

namespace UserHub.Web.Controllers;

[ApiController]
[Route("api/v1/condition-statuses")]
[Tags("Condition_Statuses")]
public sealed class ConditionStatusesController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ConditionStatusesDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Lookup(
        [FromServices] LookupConditionStatusService service,
        CancellationToken cancellationToken)
    {
        var result = await service.HandleAsync(cancellationToken);

        return Ok(result);
    }
}