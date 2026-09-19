using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Oge.Refining.CaseApp.Application.Cases;
using Oge.Refining.CaseApp.Application.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

namespace Oge.Refining.CaseApp.Api.Tests;

public sealed class CaseApiTests : IClassFixture<CaseApiFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly CaseApiFactory factory;
    private readonly HttpClient client;

    public CaseApiTests(CaseApiFactory factory)
    {
        this.factory = factory;
        client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        client.DefaultRequestHeaders.Add("X-Test-Scopes", "case.read");
    }

    [Fact]
    public async Task Health_ReturnsOk()
    {
        var response = await client.GetAsync("/api/v1/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ListCases_WithoutIdentity_ReturnsUnauthorized()
    {
        using var unauthenticatedClient = factory.CreateClient();

        var response = await unauthenticatedClient.GetAsync("/api/v1/cases");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ListCases_WithoutRequiredScope_ReturnsForbidden()
    {
        using var wrongScopeClient = factory.CreateClient();
        wrongScopeClient.DefaultRequestHeaders.Add("X-Test-Scopes", "profile.read");

        var response = await wrongScopeClient.GetAsync("/api/v1/cases");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task ListCases_FiltersByStatusAndSeverity()
    {
        var result = await client.GetFromJsonAsync<PagedResult<CaseSummary>>(
            "/api/v1/cases?status=Open&severity=Critical",
            JsonOptions);

        Assert.NotNull(result);
        var item = Assert.Single(result.Items);
        Assert.Equal("SYN-2026-0017", item.CaseNumber);
        Assert.Equal(1, result.TotalCount);
    }

    [Fact]
    public async Task DemoData_IncludesRefreshedCasePortfolio()
    {
        var result = await client.GetFromJsonAsync<PagedResult<CaseSummary>>(
            "/api/v1/cases?pageSize=20",
            JsonOptions);

        Assert.NotNull(result);
        Assert.Equal(8, result.TotalCount);
        Assert.Contains(result.Items, item => item.CaseNumber == "SYN-2026-0018" && item.Type == CaseType.EnvironmentalConcern);
        Assert.Contains(result.Items, item => item.CaseNumber == "SYN-2026-0019" && item.Type == CaseType.NearMiss);
        Assert.All(result.Items, item => Assert.True(item.ReportedAt >= new DateTimeOffset(2026, 8, 20, 0, 0, 0, TimeSpan.Zero)));

        var detail = await client.GetFromJsonAsync<CaseDetail>(
            "/api/v1/cases/SYN-2026-0018",
            JsonOptions);

        Assert.NotNull(detail);
        Assert.Equal(5, detail.Timeline.Count);
        Assert.Contains(detail.Timeline, entry => entry.EntryType == "LaboratoryResult" && entry.Content.Contains("9 ppm"));
        Assert.DoesNotContain(detail.Timeline, entry => entry.Content.Contains("demonstration purposes"));
    }

    [Fact]
    public async Task GetCase_ReturnsDetailAndTimeline()
    {
        var result = await client.GetFromJsonAsync<CaseDetail>(
            "/api/v1/cases/11111111-1111-1111-1111-111111111111",
            JsonOptions);

        Assert.NotNull(result);
        Assert.Equal("SYN-2026-0017", result.Summary.CaseNumber);
        Assert.Equal(10, result.Timeline.Count);
        Assert.True(result.RegulatoryNotifiable);
        Assert.Contains("SYN-2026-0009", result.Description);
        Assert.Contains(result.Timeline, entry => entry.EntryType == "ProcessData" && entry.Content.Contains("5.1 bar"));
        Assert.Contains(result.Timeline, entry => entry.EntryType == "Inspection" && entry.Content.Contains("spalled rollers"));
        Assert.Contains(result.Timeline, entry => entry.EntryType == "ProcedureReview" && entry.Content.Contains("MNT-14"));
        Assert.Contains(result.Timeline, entry => entry.EntryType == "ConfigurationHistory" && entry.Content.Contains("WO-4821"));
        Assert.Contains(result.Timeline, entry => entry.EntryType == "FunctionalTest" && entry.Content.Contains("4 seconds"));
        Assert.DoesNotContain(result.Timeline, entry => entry.EntryType is "RootCause" or "CorrectiveAction" or "VerificationPlan");
        Assert.DoesNotContain("root cause", JsonSerializer.Serialize(result), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetCase_AcceptsCaseNumber()
    {
        var result = await client.GetFromJsonAsync<CaseDetail>(
            "/api/v1/cases/SYN-2026-0009",
            JsonOptions);

        Assert.NotNull(result);
        Assert.Equal("44444444-4444-4444-4444-444444444444", result.Summary.Id.ToString());
        Assert.Equal("SYN-2026-0009", result.Summary.CaseNumber);
    }

    [Fact]
    public async Task Mcp_ListsAndInvokesCaseTools()
    {
        await using var transport = new HttpClientTransport(
            new HttpClientTransportOptions
            {
                Endpoint = new Uri(client.BaseAddress!, "/mcp")
            },
            client);
        await using var mcpClient = await McpClient.CreateAsync(transport);

        var tools = await mcpClient.ListToolsAsync();

        var searchTool = Assert.Single(tools, tool => tool.Name == "search_cases");
        Assert.True(searchTool.ProtocolTool.Annotations?.ReadOnlyHint);
        Assert.False(searchTool.ProtocolTool.Annotations?.DestructiveHint);
        Assert.False(searchTool.ProtocolTool.Annotations?.OpenWorldHint);
        Assert.NotNull(searchTool.ProtocolTool.OutputSchema);

        var getTool = Assert.Single(tools, tool => tool.Name == "get_case");
        Assert.True(getTool.ProtocolTool.Annotations?.ReadOnlyHint);
        Assert.False(getTool.ProtocolTool.Annotations?.DestructiveHint);
        Assert.False(getTool.ProtocolTool.Annotations?.OpenWorldHint);
        Assert.NotNull(getTool.ProtocolTool.OutputSchema);

        var result = await mcpClient.CallToolAsync(
            "search_cases",
            new Dictionary<string, object?>
            {
                ["status"] = "Open",
                ["severity"] = "Critical"
            });
        var content = Assert.Single(result.Content.OfType<TextContentBlock>());
        Assert.Contains("SYN-2026-0017", content.Text);
        Assert.NotNull(result.StructuredContent);

        var relatedCases = await mcpClient.CallToolAsync(
            "search_cases",
            new Dictionary<string, object?> { ["search"] = "P-204B" });
        var relatedContent = Assert.Single(relatedCases.Content.OfType<TextContentBlock>());
        Assert.Contains("SYN-2026-0017", relatedContent.Text);
        Assert.Contains("SYN-2026-0009", relatedContent.Text);

        var caseDetail = await mcpClient.CallToolAsync(
            "get_case",
            new Dictionary<string, object?>
            {
                ["id"] = "11111111-1111-1111-1111-111111111111"
            });
        var detailContent = Assert.Single(caseDetail.Content.OfType<TextContentBlock>());
        Assert.Contains("ProcedureReview", detailContent.Text);
        Assert.Contains("ConfigurationHistory", detailContent.Text);
        Assert.Contains("FunctionalTest", detailContent.Text);
        Assert.DoesNotContain("RootCause", detailContent.Text, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("direct cause", detailContent.Text, StringComparison.OrdinalIgnoreCase);
        Assert.NotNull(caseDetail.StructuredContent);
    }

    [Fact]
    public async Task Mcp_PublishesOAuthProtectedResourceMetadata()
    {
        var metadata = await client.GetFromJsonAsync<JsonElement>(
            "/.well-known/oauth-protected-resource/mcp");

        Assert.Equal(
            "http://localhost/mcp",
            metadata.GetProperty("resource").GetString());
        Assert.Contains(
            "https://login.microsoftonline.com/00000000-0000-4000-8000-000000000001/v2.0",
            metadata.GetProperty("authorization_servers")
                .EnumerateArray()
                .Select(value => value.GetString()));
        Assert.Contains(
            "api://00000000-0000-4000-8000-000000000002/case.read",
            metadata.GetProperty("scopes_supported")
                .EnumerateArray()
                .Select(value => value.GetString()));
    }

    [Fact]
    public async Task Mcp_WithoutIdentity_ReturnsOAuthChallenge()
    {
        using var unauthenticatedClient = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Post, "/mcp")
        {
            Content = JsonContent.Create(new
            {
                jsonrpc = "2.0",
                id = 1,
                method = "tools/list",
                @params = new { }
            })
        };
        request.Headers.Accept.ParseAdd("application/json, text/event-stream");

        var response = await unauthenticatedClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var challenge = Assert.Single(response.Headers.WwwAuthenticate);
        Assert.Equal("Bearer", challenge.Scheme);
        Assert.Contains("resource_metadata=", challenge.Parameter);
    }

    [Fact]
    public async Task GetCurrentCaller_ForwardsIncomingTokenAsOboAssertion()
    {
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "incoming-user-token");

        var result = await client.GetFromJsonAsync<CallerProfile>("/api/v1/me");

        Assert.NotNull(result);
        Assert.Equal("Test Caller", result.DisplayName);
        Assert.Equal("incoming-user-token", factory.CallerProfileService.LastAssertion);
    }

    [Fact]
    public async Task ListCases_RejectsInvalidPaginationWithProblemDetails()
    {
        var response = await client.GetAsync("/api/v1/cases?page=0&pageSize=101");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}

public sealed class CaseApiFactory : WebApplicationFactory<Program>
{
    private readonly string databasePath = Path.Combine(
        Path.GetTempPath(),
        $"case-app-tests-{Guid.NewGuid():N}.db");

    public FakeCallerProfileService CallerProfileService { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("ConnectionStrings:Cases", $"Data Source={databasePath}");
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<ICallerProfileService>();
            services.AddSingleton<ICallerProfileService>(CallerProfileService);
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthenticationHandler.SchemeName;
                    options.DefaultChallengeScheme = TestAuthenticationHandler.SchemeName;
                    options.DefaultForbidScheme = TestAuthenticationHandler.SchemeName;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                    TestAuthenticationHandler.SchemeName,
                    _ => { });
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (!disposing) return;

        SqliteConnection.ClearAllPools();
        if (File.Exists(databasePath)) File.Delete(databasePath);
    }
}

public sealed class FakeCallerProfileService : ICallerProfileService
{
    public string? LastAssertion { get; private set; }

    public Task<CallerProfile> GetAsync(
        string userAssertion,
        CancellationToken cancellationToken = default)
    {
        LastAssertion = userAssertion;
        return Task.FromResult(new CallerProfile(
            "00000000-0000-0000-0000-000000000002",
            "Test Caller",
            "caller@example.test"));
    }
}

public sealed class TestAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "Test";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("X-Test-Scopes", out var scopes))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var claims = new[]
        {
            new Claim("oid", "00000000-0000-0000-0000-000000000002"),
            new Claim("scp", scopes.ToString())
        };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, SchemeName));
        return Task.FromResult(AuthenticateResult.Success(
            new AuthenticationTicket(principal, SchemeName)));
    }
}