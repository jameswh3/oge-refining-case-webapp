using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Oge.Refining.CaseApp.Application.Identity;
using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Options;

namespace Oge.Refining.CaseApp.Infrastructure.Identity;

public sealed class GraphOboCallerProfileService(
    HttpClient httpClient,
    IOptions<OboOptions> options) : ICallerProfileService
{
    private static readonly string[] AssertionScopes = ["api://AzureADTokenExchange/.default"];
    private readonly OboOptions options = options.Value;

    public async Task<CallerProfile> GetAsync(
        string userAssertion,
        CancellationToken cancellationToken = default)
    {
        var credential = string.IsNullOrWhiteSpace(options.ManagedIdentityClientId)
            ? new ManagedIdentityCredential()
            : new ManagedIdentityCredential(options.ManagedIdentityClientId);
        var clientAssertion = await credential.GetTokenAsync(
            new TokenRequestContext(AssertionScopes),
            cancellationToken);

        using var tokenRequest = new HttpRequestMessage(
            HttpMethod.Post,
            $"https://login.microsoftonline.com/{options.TenantId}/oauth2/v2.0/token")
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = options.ClientId,
                ["grant_type"] = "urn:ietf:params:oauth:grant-type:jwt-bearer",
                ["assertion"] = userAssertion,
                ["requested_token_use"] = "on_behalf_of",
                ["scope"] = options.GraphScope,
                ["client_assertion_type"] =
                    "urn:ietf:params:oauth:client-assertion-type:jwt-bearer",
                ["client_assertion"] = clientAssertion.Token
            })
        };
        using var tokenResponse = await httpClient.SendAsync(tokenRequest, cancellationToken);
        tokenResponse.EnsureSuccessStatusCode();
        var token = await tokenResponse.Content.ReadFromJsonAsync<OboTokenResponse>(
            cancellationToken: cancellationToken)
            ?? throw new HttpRequestException("Entra returned an empty OBO token response.");

        using var graphRequest = new HttpRequestMessage(
            HttpMethod.Get,
            "https://graph.microsoft.com/v1.0/me?$select=id,displayName,userPrincipalName");
        graphRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
        using var graphResponse = await httpClient.SendAsync(graphRequest, cancellationToken);
        graphResponse.EnsureSuccessStatusCode();
        var profile = await graphResponse.Content.ReadFromJsonAsync<GraphProfile>(
            cancellationToken: cancellationToken)
            ?? throw new HttpRequestException("Microsoft Graph returned an empty profile response.");

        return new CallerProfile(profile.Id, profile.DisplayName, profile.UserPrincipalName);
    }

    private sealed record OboTokenResponse(
        [property: JsonPropertyName("access_token")] string AccessToken);

    private sealed record GraphProfile(
        string Id,
        string DisplayName,
        string? UserPrincipalName);
}