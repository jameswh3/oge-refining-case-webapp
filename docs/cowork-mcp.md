# Copilot Cowork MCP connector

The API exposes a remote, read-only MCP server over Streamable HTTP at `<API-BASE-URL>/mcp`. It supports JSON-RPC 2.0 `tools/list` and `tools/call`, structured output, delegated Microsoft Entra authentication, and RFC 9728 protected-resource metadata.

The available tools are:

| Tool | Purpose |
|---|---|
| `search_cases` | Search, filter, and page through case summaries |
| `get_case` | Retrieve one case and its timeline |

Both tools are annotated as read-only, non-destructive, and idempotent.

## Plugin package

Copy `cowork/tools/case-app-tools.json` into the Microsoft 365 app package and reference it from the app manifest:

```json
{
  "agentConnectors": [
    {
      "id": "oge-refining-case-content",
      "displayName": "Refinery Cases",
      "description": "Search and retrieve refinery case reports.",
      "toolSource": {
        "remoteMcpServer": {
          "mcpServerUrl": "<API-BASE-URL>/mcp",
          "mcpToolDescription": {
            "file": "./tools/case-app-tools.json"
          },
          "authorization": {
            "type": "OAuthPluginVault",
            "referenceId": "<AUTH-CONFIG-ID>"
          }
        }
      }
    }
  ]
}
```

## OAuth Plugin Vault

Create a single-tenant OAuth client registration and the matching OAuth client configuration in the Teams developer portal.

| Setting | Value |
|---|---|
| Base URL | `<API-BASE-URL>/mcp` |
| Authorization endpoint | `https://login.microsoftonline.com/<TENANT-ID>/oauth2/v2.0/authorize` |
| Token and refresh endpoint | `https://login.microsoftonline.com/<TENANT-ID>/oauth2/v2.0/token` |
| Scope | `offline_access api://<API-CLIENT-ID>/case.read` |
| Redirect URI | Value supplied by the Microsoft 365 OAuth configuration experience |

Grant the OAuth client delegated `case.read` permission to the Case API. Store its client secret only in the Microsoft Enterprise token store through the Teams developer portal or Microsoft 365 Agents Toolkit. Put the generated authorization configuration ID in the app manifest.

The API publishes protected-resource metadata at `<API-BASE-URL>/.well-known/oauth-protected-resource/mcp`. Verify that it advertises the expected tenant authorization server and `case.read` scope before packaging the connector. See [Authentication and OBO](authentication.md) for API token validation.