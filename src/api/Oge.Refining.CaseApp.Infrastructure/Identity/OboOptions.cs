namespace Oge.Refining.CaseApp.Infrastructure.Identity;

public sealed class OboOptions
{
    public required string TenantId { get; init; }
    public required string ClientId { get; init; }
    public string? ManagedIdentityClientId { get; init; }
    public string GraphScope { get; init; } = "https://graph.microsoft.com/.default";
}