namespace Oge.Refining.CaseApp.Application.Cases;

public enum CaseStatus
{
    Open,
    InProgress,
    OnHold,
    Closed
}

public enum CaseSeverity
{
    Informational,
    Minor,
    Moderate,
    Major,
    Critical
}

public enum CaseType
{
    Incident,
    NearMiss,
    NonConformance,
    SafetyObservation,
    MaintenanceIssue,
    EnvironmentalConcern
}

public sealed record CaseSummary(
    Guid Id,
    string CaseNumber,
    string Title,
    string RefineryName,
    string ProcessUnitName,
    CaseType Type,
    CaseStatus Status,
    CaseSeverity Severity,
    DateTimeOffset ReportedAt,
    DateTimeOffset UpdatedAt);

public sealed record CaseTimelineEntry(
    Guid Id,
    string EntryType,
    string AuthorName,
    DateTimeOffset OccurredAt,
    string Content);

public sealed record CaseDetail(
    CaseSummary Summary,
    string Description,
    bool RegulatoryNotifiable,
    IReadOnlyList<CaseTimelineEntry> Timeline);

public sealed record CaseQuery(
    string? Search = null,
    CaseStatus? Status = null,
    CaseSeverity? Severity = null,
    CaseType? Type = null,
    int Page = 1,
    int PageSize = 20);

public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalCount);

public interface ICaseQueryService
{
    Task<PagedResult<CaseSummary>> ListAsync(
        CaseQuery query,
        CancellationToken cancellationToken = default);

    Task<CaseDetail?> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<CaseDetail?> GetByReferenceAsync(
        string reference,
        CancellationToken cancellationToken = default);
}