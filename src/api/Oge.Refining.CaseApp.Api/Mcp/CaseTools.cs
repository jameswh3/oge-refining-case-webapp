using System.ComponentModel;
using Oge.Refining.CaseApp.Application.Cases;
using Microsoft.AspNetCore.Authorization;
using ModelContextProtocol.Server;

namespace Oge.Refining.CaseApp.Api.Mcp;

[McpServerToolType]
[Authorize(Policy = "CaseRead")]
public sealed class CaseTools(ICaseQueryService caseQueryService)
{
    [McpServerTool(
        Name = "search_cases",
        Title = "Search Cases",
        ReadOnly = true,
        Destructive = false,
        Idempotent = true,
        OpenWorld = false,
        UseStructuredContent = true)]
    [Description("Searches and filters refinery cases. Results are ordered by most recently updated.")]
    public Task<PagedResult<CaseSummary>> SearchCasesAsync(
        [Description("Text to find in case numbers, titles, facilities, units, or descriptions.")] string? search = null,
        [Description("Case workflow status.")] CaseStatus? status = null,
        [Description("Case severity.")] CaseSeverity? severity = null,
        [Description("Case type.")] CaseType? type = null,
        [Description("One-based result page.")] int page = 1,
        [Description("Results per page, from 1 through 100.")] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(pageSize, 100);

        return caseQueryService.ListAsync(
            new CaseQuery(search, status, severity, type, page, pageSize),
            cancellationToken);
    }

    [McpServerTool(
        Name = "get_case",
        Title = "Get Case Details",
        ReadOnly = true,
        Destructive = false,
        Idempotent = true,
        OpenWorld = false,
        UseStructuredContent = true)]
    [Description("Gets one refinery case, including its description and timeline.")]
    public Task<CaseDetail?> GetCaseAsync(
        [Description("The case ID returned by search_cases.")] Guid id,
        CancellationToken cancellationToken = default) =>
        caseQueryService.GetAsync(id, cancellationToken);
}