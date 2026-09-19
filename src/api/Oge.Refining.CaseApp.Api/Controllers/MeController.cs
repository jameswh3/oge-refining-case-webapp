using Oge.Refining.CaseApp.Application.Identity;
using Azure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace Oge.Refining.CaseApp.Api.Controllers;

[ApiController]
[Authorize(Policy = "CaseRead")]
[Route("api/v1/me")]
public sealed class MeController(ICallerProfileService callerProfileService) : ControllerBase
{
    [HttpGet(Name = "GetCurrentCaller")]
    [ProducesResponseType<CallerProfile>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<CallerProfile>> GetCurrentCaller(
        CancellationToken cancellationToken)
    {
        var authorization = Request.Headers[HeaderNames.Authorization].ToString();
        if (!authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Bearer token unavailable",
                detail: "The incoming delegated access token is required for OBO.");
        }

        try
        {
            var profile = await callerProfileService.GetAsync(
                authorization["Bearer ".Length..].Trim(),
                cancellationToken);
            return Ok(profile);
        }
        catch (CredentialUnavailableException)
        {
            return Problem(
                statusCode: StatusCodes.Status503ServiceUnavailable,
                title: "Managed identity unavailable",
                detail: "OBO requires the federated managed identity configured on the Azure API host.");
        }
        catch (HttpRequestException)
        {
            return Problem(
                statusCode: StatusCodes.Status503ServiceUnavailable,
                title: "Downstream identity service unavailable",
                detail: "The OBO exchange or Microsoft Graph request could not be completed.");
        }
    }
}