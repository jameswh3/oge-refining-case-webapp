namespace Oge.Refining.CaseApp.Application.Identity;

public sealed record CallerProfile(
    string ObjectId,
    string DisplayName,
    string? UserPrincipalName);

public interface ICallerProfileService
{
    Task<CallerProfile> GetAsync(
        string userAssertion,
        CancellationToken cancellationToken = default);
}