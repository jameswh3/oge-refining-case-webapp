using Oge.Refining.CaseApp.Application.Cases;
using Oge.Refining.CaseApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Oge.Refining.CaseApp.Infrastructure.Cases;

public sealed class EfCoreCaseQueryService(CaseDbContext dbContext) : ICaseQueryService
{
    public async Task<PagedResult<CaseSummary>> ListAsync(
        CaseQuery query,
        CancellationToken cancellationToken = default)
    {
        var cases = dbContext.Cases.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            cases = cases.Where(item =>
                item.CaseNumber.Contains(search) ||
                item.Title.Contains(search) ||
                item.RefineryName.Contains(search) ||
                item.ProcessUnitName.Contains(search) ||
                item.Description.Contains(search));
        }

        if (query.Status is not null) cases = cases.Where(item => item.Status == query.Status);
        if (query.Severity is not null) cases = cases.Where(item => item.Severity == query.Severity);
        if (query.Type is not null) cases = cases.Where(item => item.Type == query.Type);

        var totalCount = await cases.CountAsync(cancellationToken);
        var records = await cases
            .OrderByDescending(item => item.UpdatedAt)
            .ThenBy(item => item.CaseNumber)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToArrayAsync(cancellationToken);
        var items = records
            .Select(item => new CaseSummary(
                item.Id,
                item.CaseNumber,
                item.Title,
                item.RefineryName,
                item.ProcessUnitName,
                item.Type,
                item.Status,
                item.Severity,
                AsUtc(item.ReportedAt),
                AsUtc(item.UpdatedAt)))
            .ToArray();

        return new PagedResult<CaseSummary>(items, query.Page, query.PageSize, totalCount);
    }

    public async Task<CaseDetail?> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await GetByReferenceAsync(id.ToString(), cancellationToken);
    }

    public async Task<CaseDetail?> GetByReferenceAsync(
        string reference,
        CancellationToken cancellationToken = default)
    {
        var normalizedReference = reference.Trim();
        var isId = Guid.TryParse(normalizedReference, out var id);
        var item = await dbContext.Cases
            .AsNoTracking()
            .Include(record => record.Timeline)
            .SingleOrDefaultAsync(
                record => isId ? record.Id == id : record.CaseNumber == normalizedReference,
                cancellationToken);
        if (item is null) return null;

        var summary = new CaseSummary(
            item.Id,
            item.CaseNumber,
            item.Title,
            item.RefineryName,
            item.ProcessUnitName,
            item.Type,
            item.Status,
            item.Severity,
            AsUtc(item.ReportedAt),
            AsUtc(item.UpdatedAt));
        var timeline = item.Timeline
            .OrderBy(entry => entry.OccurredAt)
            .Select(entry => new CaseTimelineEntry(
                entry.Id,
                entry.EntryType,
                entry.AuthorName,
                AsUtc(entry.OccurredAt),
                entry.Content))
            .ToArray();

        return new CaseDetail(summary, item.Description, item.RegulatoryNotifiable, timeline);
    }

    private static DateTimeOffset AsUtc(DateTime value) =>
        new(DateTime.SpecifyKind(value, DateTimeKind.Utc));
}