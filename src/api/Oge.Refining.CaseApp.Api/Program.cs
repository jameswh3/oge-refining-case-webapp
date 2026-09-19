using Oge.Refining.CaseApp.Application.Cases;
using Oge.Refining.CaseApp.Application.Identity;
using Oge.Refining.CaseApp.Api.Authentication;
using Oge.Refining.CaseApp.Api.Mcp;
using Oge.Refining.CaseApp.Infrastructure.Cases;
using Oge.Refining.CaseApp.Infrastructure.Data;
using Oge.Refining.CaseApp.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using ModelContextProtocol.AspNetCore;
using ModelContextProtocol.AspNetCore.Authentication;
using ModelContextProtocol.Server;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();
builder.Services.AddApplicationInsightsTelemetry();
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddPolicy("Spa", policy =>
    {
        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
    });
});
var databaseProvider = builder.Configuration["Database:Provider"] ?? "SqlServer";
var connectionString = builder.Configuration.GetConnectionString("Cases")
    ?? throw new InvalidOperationException("ConnectionStrings:Cases is required.");
builder.Services.AddDbContext<CaseDbContext>(options =>
{
    if (databaseProvider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
    {
        options.UseSqlite(connectionString);
    }
    else if (databaseProvider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
    {
        options.UseSqlServer(connectionString);
    }
    else
    {
        throw new InvalidOperationException($"Unsupported database provider '{databaseProvider}'.");
    }
});
builder.Services.AddScoped<ICaseQueryService, EfCoreCaseQueryService>();
builder.Services.AddMcpServer()
    .WithHttpTransport()
    .AddAuthorizationFilters()
    .WithTools<CaseTools>();
builder.Services.Configure<OboOptions>(builder.Configuration.GetSection("Obo"));
builder.Services.AddHttpClient<ICallerProfileService, GraphOboCallerProfileService>();

var authentication = builder.Configuration.GetSection("Authentication");
var authenticationEnabled = authentication.GetValue<bool>("Enabled");
var requiredScope = authentication["RequiredScope"]
    ?? throw new InvalidOperationException("Authentication:RequiredScope is required.");

if (authenticationEnabled)
{
    var tenantId = authentication["TenantId"]
        ?? throw new InvalidOperationException("Authentication:TenantId is required.");
    var clientId = authentication["ClientId"]
        ?? throw new InvalidOperationException("Authentication:ClientId is required.");
    var audience = authentication["Audience"]
        ?? throw new InvalidOperationException("Authentication:Audience is required.");

    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Authority = $"https://login.microsoftonline.com/{tenantId}/v2.0";
            options.MapInboundClaims = false;
            options.TokenValidationParameters.ValidAudiences = [audience, clientId];
            options.TokenValidationParameters.ValidateIssuer = true;
            options.TokenValidationParameters.ValidIssuer =
                $"https://login.microsoftonline.com/{tenantId}/v2.0";
        });
}
else
{
    if (!builder.Environment.IsDevelopment() && !builder.Environment.IsEnvironment("Testing"))
    {
        throw new InvalidOperationException(
            "Authentication can only be disabled in Development or Testing.");
    }

    builder.Services
        .AddAuthentication(DevelopmentAuthenticationHandler.SchemeName)
        .AddScheme<AuthenticationSchemeOptions, DevelopmentAuthenticationHandler>(
            DevelopmentAuthenticationHandler.SchemeName,
            _ => { });
}

var mcpAuthorizationServer = builder.Configuration["Mcp:AuthorizationServer"]
    ?? throw new InvalidOperationException("Mcp:AuthorizationServer is required.");
var mcpScope = builder.Configuration["Mcp:Scope"]
    ?? throw new InvalidOperationException("Mcp:Scope is required.");
var mcpForwardAuthenticateScheme = builder.Configuration["Mcp:ForwardAuthenticateScheme"]
    ?? throw new InvalidOperationException("Mcp:ForwardAuthenticateScheme is required.");
builder.Services.AddAuthentication()
    .AddMcp(options =>
    {
        options.ForwardAuthenticate = mcpForwardAuthenticateScheme;
        options.ResourceMetadata = new()
        {
            ResourceName = "OGE Refining Case Web App",
            ResourceDocumentation = "https://github.com/modelcontextprotocol/csharp-sdk",
            AuthorizationServers = { mcpAuthorizationServer },
            ScopesSupported = { mcpScope }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CaseRead", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireAssertion(context => context.User
            .FindAll("scp")
            .SelectMany(claim => claim.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            .Contains(requiredScope, StringComparer.Ordinal));
    });
    options.AddPolicy("McpCaseRead", policy =>
    {
        policy.AddAuthenticationSchemes(McpAuthenticationDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser();
        policy.RequireAssertion(context => context.User
            .FindAll("scp")
            .SelectMany(claim => claim.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            .Contains(requiredScope, StringComparer.Ordinal));
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("Spa");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/api/v1/health");
app.MapMcp("/mcp").RequireAuthorization("McpCaseRead");

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CaseDbContext>();
    if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Testing"))
    {
        await dbContext.Database.EnsureCreatedAsync();
    }
    else
    {
        await dbContext.Database.MigrateAsync();
    }
}

app.Run();

public partial class Program;
