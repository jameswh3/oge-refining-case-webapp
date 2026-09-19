using Oge.Refining.CaseApp.Application.Cases;

namespace Oge.Refining.CaseApp.Infrastructure.Data;

public sealed class CaseRecord
{
    public Guid Id { get; set; }
    public required string CaseNumber { get; set; }
    public required string Title { get; set; }
    public required string RefineryName { get; set; }
    public required string ProcessUnitName { get; set; }
    public CaseType Type { get; set; }
    public CaseStatus Status { get; set; }
    public CaseSeverity Severity { get; set; }
    public DateTime ReportedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public required string Description { get; set; }
    public bool RegulatoryNotifiable { get; set; }
    public ICollection<CaseTimelineRecord> Timeline { get; set; } = [];
}

public sealed class CaseTimelineRecord
{
    public Guid Id { get; set; }
    public Guid CaseId { get; set; }
    public required string EntryType { get; set; }
    public required string AuthorName { get; set; }
    public DateTime OccurredAt { get; set; }
    public required string Content { get; set; }
    public CaseRecord? Case { get; set; }
}