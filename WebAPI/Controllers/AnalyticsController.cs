using Application.Features.Analytics;
using Domain.Entities.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("")]
public class AnalyticsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AnalyticsController(IMediator mediator) => _mediator = mediator;

    [HttpGet("/spending-analytics")]
    [ProducesResponseType(typeof(SpendingAnalyticsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetSpendingAnalytics(
        [FromQuery(Name = "catcode")] string? categoryCode,
        [FromQuery(Name = "start-date")] DateTime? startDate,
        [FromQuery(Name = "end-date")] DateTime? endDate,
        [FromQuery(Name = "direction")] Direction? direction,
        CancellationToken ct)
    {
        var query = new GetSpendingAnalyticsQuery(categoryCode, startDate, endDate, direction);
        var response = await _mediator.Send(query, ct);
        return Ok(response);
    }
}

