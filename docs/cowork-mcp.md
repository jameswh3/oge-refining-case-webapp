# Copilot Cowork plugin

The Case API exposes a read-only MCP server over Streamable HTTP at `<API-BASE-URL>/mcp`. This folder contains the Cowork app-package template and tool description, and `scripts/package-cowork.ps1` builds the installable `.zip`. OAuth credentials are configured in the Microsoft Enterprise Token Store; secrets are not included in the repository or package.

The MCP server provides `search_cases` and `get_case`, both read-only, non-destructive, and idempotent. See [the API MCP host](../src/api/Oge.Refining.CaseApp.Api/Program.cs) and [tool implementation](../src/api/Oge.Refining.CaseApp.Api/Mcp/CaseTools.cs).

## 1. Deploy and verify the MCP endpoint

Deploy the API to a public HTTPS host and set:

```text
MCP_SERVER_URL=https://<API-HOST>/mcp
```

The endpoint must be reachable by Microsoft 365. Confirm the protected-resource metadata is available at `https://<API-HOST>/.well-known/oauth-protected-resource/mcp` and advertises the correct Entra authorization server and `case.read` scope. Check that authenticated MCP `tools/list` returns both tools. The package script rejects non-HTTPS endpoints and URLs that do not end in `/mcp`.

## 2. Register the OAuth client and create its token-store configuration

Register an OAuth client with Microsoft Entra ID, then configure the authorization-code flow for the API:

| Setting | Value |
|---|---|
| Authorization endpoint | `https://login.microsoftonline.com/<TENANT-ID>/oauth2/v2.0/authorize` |
| Token endpoint | `https://login.microsoftonline.com/<TENANT-ID>/oauth2/v2.0/token` |
| Scope | `offline_access api://<API-CLIENT-ID>/case.read` |
| Redirect URI | `https://teams.microsoft.com/api/platform/v1.0/oAuthRedirect` |

Add the redirect URI to the OAuth client's allowed redirect URIs. Grant it delegated `case.read` access to the Case API and grant tenant consent as required. The tenant, API application ID, scope, authorization endpoint, and token endpoint must match the deployed API's Entra configuration.

In Microsoft 365 Agents Toolkit or the Teams developer portal, create an OAuth configuration in the Microsoft Enterprise Token Store using the OAuth client ID, any required client secret, endpoints, and scope above. For Cowork, make the registration available to **Any Microsoft 365 Organization** as directed by the platform. Keep client secrets in the token store only. Record the generated OAuth configuration ID; this is the manifest's `referenceId`.

See Microsoft's guides for [agent connector authorization](https://learn.microsoft.com/microsoftteams/platform/m365-apps/agent-connectors), [OAuth authentication](https://learn.microsoft.com/microsoft-365/copilot/extensibility/plugin-authentication-oauth), and the [Cowork plugin package format](https://learn.microsoft.com/microsoft-365/copilot/cowork/cowork-plugin-development).

## 3. Build the app package

The app manifest is based on the Microsoft 365 manifest schema v1.28 at [cowork/appPackage/manifest.template.json](../cowork/appPackage/manifest.template.json). The package includes the manifest, required 192×192 and 32×32 PNG icons, and [the tool description](../cowork/tools/case-app-tools.json). The manifest points at the deployed MCP endpoint and OAuth configuration.

Create an app-package GUID and provide real public HTTPS URLs for your developer site, privacy policy, and terms of use:

```powershell
.\scripts\package-cowork.ps1 `
  -AppId "11111111-2222-4333-8444-555555555555" `
  -McpServerUrl "https://<API-HOST>/mcp" `
  -OAuthReferenceId "<OAUTH-CONFIGURATION-ID>" `
  -DeveloperName "<ORGANIZATION-NAME>" `
  -DeveloperWebsiteUrl "https://<YOUR-SITE>" `
  -PrivacyUrl "https://<YOUR-SITE>/privacy" `
  -TermsOfUseUrl "https://<YOUR-SITE>/terms"
```

The command creates `artifacts/cowork/oge-refining-case-cowork.zip` with the package files at the ZIP root. Override `-OutputPath` to choose another location. The app-package GUID identifies the package and is distinct from the Entra API and OAuth client IDs.

## 4. Install and publish

For personal testing, install the generated package with the Microsoft 365 Agents Toolkit CLI after signing in:

```powershell
npm install -g @microsoft/m365agentstoolkit-cli
atk auth login
atk install --file-path "C:\path\to\oge-refining-case-cowork.zip" --scope Personal
```

For tenant discovery, an admin uploads the `.zip` from **Microsoft 365 admin center > Manage apps > Upload custom app > Add agent**. The plugin then appears in **Cowork > Sources & Skills > Plugins** according to tenant policy. For the exact current upload workflow and any tenant-specific prerequisites, see Microsoft's [Cowork plugin development guide](https://learn.microsoft.com/microsoft-365/copilot/cowork/cowork-plugin-development).

After installation, test OAuth sign-in, `search_cases`, and `get_case` against the deployed service. Anonymous MCP requests should return `401`; never put access tokens, refresh tokens, or OAuth client secrets in the package or source control.