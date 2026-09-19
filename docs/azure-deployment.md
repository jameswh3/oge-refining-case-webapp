# Azure deployment

The repository deploys a demonstration environment consisting of:

- ASP.NET Core API on App Service Free `F1`
- React SPA on Azure Static Web Apps
- Azure SQL Basic database
- User-assigned managed identity
- Log Analytics and Application Insights

The default tiers and networking are for lab use. Before production, choose an App Service tier without free-tier cold starts, move database migrations to a separate deployment identity, and use approved private SQL connectivity.

## Prerequisites

- Azure CLI authenticated to the target tenant and subscription
- Permission to deploy Azure resources and update the API and SPA Entra registrations
- Permission to configure the Azure SQL Microsoft Entra administrator
- .NET 8, Node.js/npm, and PowerShell 5.1 or later
- PowerShell Gallery access if the `SqlServer` module is not installed
- Any required security-policy approval for temporary public SQL access

Set these environment variables before deployment. The script also accepts corresponding parameters for the Azure and Entra values.

| Variable | Purpose |
|---|---|
| `AZURE_SUBSCRIPTION_ID` | Target Azure subscription ID |
| `AZURE_RESOURCE_GROUP` | Resource group to create or update |
| `AZURE_LOCATION` | Primary workload region, such as `northcentralus` |
| `ENTRA_TENANT_ID` | Microsoft Entra tenant ID |
| `ENTRA_API_CLIENT_ID` | Case API application client ID |
| `ENTRA_API_APP_OBJECT_ID` | Case API application object ID |
| `ENTRA_API_SCOPE` | Delegated scope, such as `api://<API-CLIENT-ID>/case.read` |
| `ENTRA_SPA_CLIENT_ID` | SPA application client ID |
| `ENTRA_SPA_APP_OBJECT_ID` | SPA application object ID |
| `ENTRA_ADMIN_OBJECT_ID` | Azure SQL Entra administrator object ID |
| `ENTRA_ADMIN_NAME` | Azure SQL Entra administrator display name or UPN |
| `BROKER_BASE_URL` | Foundry agent broker base URL |
| `BROKER_CLIENT_ID` | Foundry agent broker application client ID |
| `COPILOT_STUDIO_ENVIRONMENT_ID` | Copilot Studio environment ID |
| `COPILOT_STUDIO_SCHEMA_NAME` | Copilot Studio agent schema name |

`.env.example` is a value catalog; the deployment script does not load it.

## Deploy

From the repository root:

```powershell
az login --tenant <tenant-id>
.\scripts\deploy-azure.ps1
```

The script:

1. Selects the subscription and registers Azure resource providers.
2. Creates or updates the resource group and validates the Bicep template.
3. Deploys App Service, Static Web Apps, Azure SQL, managed identity, and monitoring.
4. Adds the managed identity's federated credential to the API registration.
5. Adds the deployed SPA URL to the SPA registration.
6. Creates the managed identity database user and grants runtime and migration roles.
7. Publishes and deploys the API.
8. Builds and deploys the SPA with the environment-specific Vite settings.
9. Generates `openapi/power-platform.deployed.swagger.json` and probes API health.

The Azure SQL external-user SID must be the managed identity client ID, not its principal/object ID. The script verifies and repairs that mapping.

## Verify

1. Confirm `/api/v1/health` returns `200`.
2. Confirm anonymous `/api/v1/cases` and `/mcp` requests return `401`.
3. Sign in to the SPA and verify case list, filters, and detail views.
4. Call `/api/v1/me` and confirm the signed-in Graph user.
5. Restart the API and verify cases remain available from Azure SQL.
6. List MCP tools with a delegated token, then call `search_cases` and `get_case`.
7. Review failed requests, dependencies, and exceptions in Application Insights.
8. Complete the checks in [Power Platform and Refining Case Buddy](power-platform.md) when deploying the agent.

## Retry and cleanup

The infrastructure deployment is idempotent, and the script avoids duplicating the API federated credential. It also creates temporary deployment artifacts and SQL firewall rules during execution. If a run fails, inspect the failed step and confirm temporary firewall access was removed before retrying.

Do not place Azure deployment tokens, SQL access tokens, OAuth tokens, or client secrets in repository files or command output.

## Known limits

- The free App Service plan can cold start and is appropriate only for this lab.
- The first production API start applies EF migrations. The managed identity therefore has DDL permissions; production hardening should move migrations to a separate deployment identity.

## Embedded agent troubleshooting

- Build the SPA through `scripts/deploy-azure.ps1` or provide all production `VITE_*` values explicitly. A build without `VITE_API_BASE_URL` calls `/api/v1/cases` on the Static Web App origin and receives `404`.
- Pass the Copilot Studio schema name as the Agents SDK `agentIdentifier` setting alongside `environmentId`.
- Under React 19, render the exported `ReactWebChat` component with `createRoot`. Do not call `renderWebChat`; it uses the removed `ReactDOM.render` API and fails with `default.render is not a function`.
- If a HAR contains the lazy-loaded Web Chat assets but no Power Platform or Copilot request, inspect the displayed local error. An MSAL `interaction_required` result should be handled through the user-initiated **Authorize Case Buddy** action.
