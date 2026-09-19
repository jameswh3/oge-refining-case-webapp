using Oge.Refining.CaseApp.Application.Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Oge.Refining.CaseApp.Api.Controllers;

[ApiController]
[Authorize(Policy = "CaseRead")]
[Route("api/v1/cases")]
public sealed class CasesController(ICaseQueryService caseQueryService) : ControllerBase
{
    [HttpGet(Name = "ListCases")]
    [ProducesResponseType<PagedResult<CaseSummary>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<CaseSummary>>> ListCases(
        [FromQuery] string? search,
        [FromQuery] CaseStatus? status,
        [FromQuery] CaseSeverity? severity,
        [FromQuery] CaseType? type,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        if (page < 1 || pageSize is < 1 or > 100)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid pagination",
                detail: "Page must be at least 1 and pageSize must be between 1 and 100.");
        }

        var result = await caseQueryService.ListAsync(
            new CaseQuery(search, status, severity, type, page, pageSize),
            cancellationToken);
        return Ok(result);
    }

    [HttpGet("{reference}", Name = "GetCase")]
    [ProducesResponseType<CaseDetail>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CaseDetail>> GetCase(
        string reference,
        CancellationToken cancellationToken)
    {
        var result = await caseQueryService.GetByReferenceAsync(reference, cancellationToken);
        return result is null
            ? Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Case not found",
                detail: $"No case exists with ID or case number '{reference}'.")
            : Ok(result);
    }
}