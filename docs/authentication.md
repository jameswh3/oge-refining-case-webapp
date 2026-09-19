# Authentication and OBO

This is the authoritative description of identity and token handling in the case app. Deployment steps are in [Azure deployment](azure-deployment.md); connector-specific setup is in [Power Platform and Refining Case Buddy](power-platform.md).

## API token validation

The SPA, Power Platform connector, and hosted MCP clients call the Case API on behalf of a signed-in user. Their access token must contain the delegated `case.read` scope.

The API validates:

- Signature and expiry
- Tenant and issuer
- Delegated `case.read` scope
- Audience matching the API registration

Microsoft Entra and Power Platform can represent the same API audience as either `api://<API-CLIENT-ID>` or the client-ID GUID. The API accepts only those two canonical values for its registration.

## Microsoft Graph on-behalf-of flow

`GET /api/v1/me` preserves the caller's identity while calling Microsoft Graph:

1. The client sends a delegated Case API token.
2. The API validates the token and uses it as the OBO user assertion.
3. The API obtains a client assertion from its user-assigned Azure managed identity.
4. Entra exchanges both assertions for a delegated Microsoft Graph token.
5. The API calls Microsoft Graph `/v1.0/me` and returns the caller profile.

OBO requires both the user assertion and confidential-client authentication. This project uses managed-identity workload federation for the latter, not an API client secret.

The federated credential on the API registration uses:

| Claim | Value |
|---|---|
| Issuer | Tenant v2 issuer, `https://login.microsoftonline.com/<TENANT-ID>/v2.0` |
| Subject | User-assigned managed identity principal ID |
| Audience | `api://AzureADTokenExchange` |

The API requests `api://AzureADTokenExchange/.default` from managed identity and sends the returned JWT as its OBO `client_assertion`.

## Identity boundaries

There are three distinct delegated tokens:

| Token | Used for |
|---|---|
| Case API token | SPA, connector, or MCP client calling the API |
| Power Platform token | SPA opening the embedded Copilot Studio agent |
| Microsoft Graph token | API calling Graph through OBO |

These tokens are not interchangeable. The connector's login bootstrap is also separate from the API-to-Graph OBO exchange.

## Environment behavior

| Environment | Incoming API identity | Graph OBO |
|---|---|---|
| Local development | Explicit development identity, or JWT when enabled | Unavailable without an Azure workload identity |
| Azure App Service | Valid delegated JWT required | Managed-identity federated client assertion |

The managed identity must exist before its federated credential can be configured because the credential subject is the identity's principal ID.

## Embedded agent identity

The SPA first attempts to acquire `https://api.powerplatform.com/.default` silently for the signed-in MSAL account. When interaction is required, **Authorize Case Buddy** starts a popup for that same account. The Microsoft 365 Agents SDK receives the user token directly; there is no iframe login, Direct Line secret, or backend token broker.

The connector separately obtains a delegated Case API token when an agent action runs. See [Power Platform and Refining Case Buddy](power-platform.md).

## Prohibited configuration

- User passwords, refresh tokens, raw access tokens, authorization headers, or cookies in configuration or logs
- API client secrets in source, user secrets, Key Vault, or App Service settings
- Reusing a Graph token for the Case API or a Case API token for Graph
- Disabling tenant, audience, issuer, expiry, or `case.read` validation
- Application-permission Graph access for `/me`, which must preserve delegated user context