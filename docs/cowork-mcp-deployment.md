# Copilot Cowork plugin packaging and deployment

This guide covers packaging and deploying **OGE Refining Cases** into Copilot Cowork. It assumes the remote MCP service is already deployed and available over HTTPS; server implementation and Azure infrastructure deployment are out of scope.

The steps describe the repository's packaging workflow and Microsoft 365 installation process, reviewed on November 12, 2026. They do not confirm that the plugin has already been installed in a particular tenant.

For what can run unattended versus what needs a person, see [Automation and manual responsibilities](#8-automation-and-manual-responsibilities).

## 1. What the plugin contains

The plugin is a Microsoft 365 app-package ZIP that connects Cowork to an existing data source. It contains configuration and metadata, not server code or case data.

This repository uses a **connector-only** package: one remote MCP connector, with no custom skills.

| Package input | Purpose |
| --- | --- |
| [manifest.template.json](../cowork/appPackage/manifest.template.json) | App identity, publisher details, remote service URL, and OAuth configuration reference. |
| [color.png](../cowork/appPackage/color.png) | 192-by-192 color app icon. |
| [outline.png](../cowork/appPackage/outline.png) | 32-by-32 outline app icon. |
| [case-app-tools.json](../cowork/tools/case-app-tools.json) | Descriptions and input schemas for the `search_cases` and `get_case` tools available to Cowork. |
| [package-cowork.ps1](../scripts/package-cowork.ps1) | Substitutes deployment-specific values and builds the installable ZIP. |

The manifest uses schema version `1.28` and currently sets app version `1.0.0`. Its user-facing app name is **OGE Refining Cases**; the connector display name is **Refinery Cases**, with connector ID `oge-refining-case-content`.

## 2. Gather the deployment inputs

Before packaging, obtain:

| Input | Where it is used |
| --- | --- |
| Public HTTPS MCP URL, such as `https://<API-HOST>/mcp` | The connector's remote service address. It must be reachable by Microsoft 365. |
| OAuth configuration ID from the Microsoft Enterprise Token Store | The manifest's `authorization.referenceId`. |
| Stable app-package GUID | The manifest's `id`; identifies this plugin across package updates. |
| Publisher name and HTTPS website, privacy, and terms URLs | Required publisher metadata in the manifest. |
| Target Microsoft 365 tenant and pilot users/groups | Determines where the plugin is installed and who can use it. |

The app-package GUID, OAuth client ID, and OAuth configuration ID are different identifiers. In particular, do not pass an Entra application client ID as `-OAuthReferenceId`.

[.env.example](../.env.example) catalogs relevant values such as `MCP_SERVER_URL`, `COWORK_OAUTH_CLIENT_ID`, and `COWORK_OAUTH_REFERENCE_ID`. The packaging script does not read `.env`; supply its parameters explicitly.

### OAuth prerequisite

The package uses `OAuthPluginVault`. Before building it:

1. Register/configure a Cowork OAuth client with delegated permission to `api://<API-CLIENT-ID>/case.read`, and obtain tenant consent as required.
2. Add `https://teams.microsoft.com/api/platform/v1.0/oAuthRedirect` as an allowed redirect URI for that client.
3. Use Microsoft 365 Agents Toolkit or the Teams developer portal to create an OAuth configuration in the Microsoft Enterprise Token Store:

   | Setting | Value |
   | --- | --- |
   | Authorization endpoint | `https://login.microsoftonline.com/<TENANT-ID>/oauth2/v2.0/authorize` |
   | Token endpoint | `https://login.microsoftonline.com/<TENANT-ID>/oauth2/v2.0/token` |
   | Scopes | `offline_access api://<API-CLIENT-ID>/case.read` |
   | Client credentials | Cowork OAuth client ID and any required secret |
   | Usage by organization | **Any Microsoft 365 Organization**, as directed by the Cowork platform guide |

4. Save the generated OAuth configuration ID for the packaging command.

That organization-usage setting does not itself grant other tenants access to the data source. Keep OAuth client secrets in the token store, never in the ZIP or source control. Installing the plugin does not grant API consent.

## 3. Build the plugin ZIP

From the repository root, run:

```powershell
.\scripts\package-cowork.ps1 `
  -AppId "11111111-2222-4333-8444-555555555555" `
  -McpServerUrl "https://<API-HOST>/mcp" `
  -OAuthReferenceId "<OAUTH-CONFIGURATION-ID>" `
  -DeveloperName "<ORGANIZATION-NAME>" `
  -DeveloperWebsiteUrl "https://<YOUR-SITE>" `
  -PrivacyUrl "https://<YOUR-SITE>/privacy" `
  -TermsOfUseUrl "https://<YOUR-SITE>/terms" `
  -OutputPath "C:\temp\oge-refining-case-cowork.zip"
```

Replace all sample values, including the sample package GUID. Use real public HTTPS publisher URLs.

The script validates the URL formats, requires the service path to end in `/mcp`, checks required inputs, and writes these connector settings into the generated manifest:

```json
"remoteMcpServer": {
  "mcpServerUrl": "https://<API-HOST>/mcp",
  "mcpToolDescription": {
    "file": "./tools/case-app-tools.json"
  },
  "authorization": {
    "type": "OAuthPluginVault",
    "referenceId": "<OAUTH-CONFIGURATION-ID>"
  }
}
```

The output ZIP has this layout:

```text
oge-refining-case-cowork.zip
  manifest.json
  color.png
  outline.png
  tools/
    case-app-tools.json
```

The generated manifest must be at the ZIP root, not in an enclosing directory. Upload the generated ZIP, not the unchanged template.

The explicit output path keeps generated files outside the worktree. Without `-OutputPath`, the script defaults to `artifacts\cowork\oge-refining-case-cowork.zip`. It stages files under `$env:TEMP` and removes that staging directory afterward; ensure the staging location is under `C:\temp` when following this repository's temporary-file policy.

**Building is not publishing:** the script does not upload the plugin, test the remote connection, validate OAuth credentials, or perform Microsoft platform schema validation.

## 4. Install for personal testing

If custom app installation is permitted by your tenant, use the Microsoft 365 Agents Toolkit CLI:

```powershell
npm install -g @microsoft/m365agentstoolkit-cli
atk --version
atk auth login
atk install --file-path "C:\temp\oge-refining-case-cowork.zip" --scope Personal
```

Sign in to the intended Microsoft 365 tenant. Save the returned `TitleId` and `AppId` for later management.

A personal installation does not deploy the plugin to the organization. Use it to test the package and OAuth sign-in before requesting tenant rollout.

## 5. Deploy through the Microsoft 365 admin center

Open the [Microsoft 365 admin center](https://admin.microsoft.com) with an appropriate administrator role.

### Upload and assign

1. Go to **Agents > Tools > Registry > Upload**.
2. Upload the plugin package using the wizard's supported package input.
3. Review the included connector.
4. Choose **Specific users or groups** for a pilot, or **All users** for broader deployment.
5. Review the configuration and select **Install**.

Microsoft's Cowork package-development guide also documents **Manage apps > Upload custom app > ... > Add agent** for uploading the ZIP and listing it for discovery. Use the available package-upload workflow in your tenant; navigation varies with the admin experience rollout.

### Automated tenant catalog submission

The newer [Work IQ Dev Tools plugin guide](https://microsoft.github.io/wiqd/getting-started/build-a-plugin/#publishing-and-existing-projects) documents an alternative to manually uploading the package: submit a provisioned plugin to the organization catalog with `wiqd plugin publish`.

For a configured WIQD plugin project, use the same explicit environment throughout:

```powershell
wiqd plugin provision --env dev
wiqd plugin package --env dev
wiqd plugin validate --env dev --mode package
wiqd plugin publish --env dev
```

These are not drop-in commands for this repository's current ZIP-only packaging workflow. Adopting them requires a configured WIQD/Agents Toolkit project and environment. Its YAML `publish:` lifecycle must use `copilotAgent/publish` with `scope: tenant`, mapping publication outputs `titleId` to `M365_PUBLISHED_TITLE_ID` and `appId` to `M365_PUBLISHED_APP_ID`. Keep those publication IDs distinct from provisioning IDs. Updating the CLI alone does not update an existing project's lifecycle configuration.

**Submission is automated; approval remains an administrator action.** A successful publish submits the plugin for approval, not immediate discovery by all users. After approval, verify availability and assignments in the admin center and test in Cowork.

`wiqd plugin share --scope users` or `--scope tenant` is a separate direct-access operation, not organization catalog publication. The tooling is in preview and its plugin command surface is alpha; confirm the installed release's commands and project configuration before adoption.

### Unattended pipeline authentication

The [WIQD pipeline authentication guide](https://microsoft.github.io/wiqd/getting-started/ci-cd-authentication/) documents core-backed provisioning, publishing, and sharing in trusted CI:

- Set `CI_ENABLED=true` exactly and inject credentials from the pipeline's secret store.
- The documented modes use delegated credentials, not an app-only/workload identity. Tenant catalog publishing requires Graph `AppCatalog.ReadWrite.All` and appropriate consent/user permissions.
- Username/password mode remains subject to MFA and Conditional Access; it is not a recommendation to weaken tenant controls.
- The documented refresh-token mode is explicitly marked **not released or fully validated**. Do not treat it as a production-ready solution.
- Do not run `wiqd auth login` in CI; that command requires an interactive terminal. Core-backed lifecycle commands acquire pipeline credentials through their documented provider mechanism.

Automating submission does not remove tenant approval, required user sign-in, or final Cowork acceptance checks. No WIQD lifecycle or CI deployment has been added or exercised in this repository by this documentation change.

### Find the deployed plugin

- **Agents > Tools > Plugins**: find **OGE Refining Cases**, inspect its details, and manage its **Users** availability.
- **Agents > All agents**: the package may also appear here, depending on the tenant experience.
- **Agents > Settings > Agent and plugin access**: check whether organization-published plugins are allowed by publisher policy.

Start with a small pilot group, verify the experience, and then expand availability. Plugin assignment controls availability in Microsoft 365; users still need authorization to the connected data source.

## 6. Verify in Copilot Cowork

With an assigned pilot account:

1. Open **Cowork > Sources & Skills > Plugins**.
2. Find **OGE Refining Cases**, including in **Discover** when published for tenant discovery, and enable/install it if prompted.
3. Complete the connector's OAuth sign-in and consent flow.
4. Try: **"Find critical in-progress refinery cases."**
5. Ask for details of one returned case to exercise both search and retrieval.

Confirm that the expected case information is returned through **Refinery Cases**, not just that the plugin's name is visible.

Availability depends on tenant policy, assignments, and applicable Cowork/Microsoft 365 Copilot licensing. A successful package upload alone does not verify that users can connect.

## 7. Update or troubleshoot the package

For an update:

1. Keep the existing app-package GUID.
2. Increment `version` in [manifest.template.json](../cowork/appPackage/manifest.template.json). The packaging script has no version parameter.
3. Rebuild with the correct environment's service URL and OAuth reference.
4. Use the tenant's app-update workflow, then repeat pilot verification before broad rollout.

| Problem | Check |
| --- | --- |
| Package upload fails | Manifest schema, substituted values, root ZIP layout, icon files, and included tool-description path. |
| Plugin is missing | Correct tenant, personal versus tenant installation, pilot assignment, and the applicable Cowork plugin view. |
| Plugin is blocked | Publisher-category policy and per-plugin user availability. |
| Sign-in fails | Token-store OAuth configuration, redirect URI, scope, and tenant consent. |
| Plugin is visible but cannot retrieve cases | Correct service URL, remote service availability, and the user's data-source authorization. |

## 8. Automation and manual responsibilities

**Bottom line:** ZIP creation is already automated here. Microsoft also documents automated tenant catalog submission through `wiqd plugin publish`, but this repository has not adopted that lifecycle. Administrator approval, organizational decisions, required interactive sign-in/consent, and final Cowork acceptance remain human checkpoints.

"Manual in this workflow" does **not** mean Microsoft makes automation impossible. It means this repository has no implementation of a supported automated path for that operation. A CLI command also does not necessarily mean unattended CI: it can still require an authenticated user or an approval.

### Step-by-step breakdown

| Guide step | Automation status | What can run automatically | What needs a person in this workflow |
| --- | --- | --- | --- |
| Section 1: app name, connector metadata, icons, and tool descriptions | Partly automatable | Reuse the checked-in files; lint JSON and check icon dimensions and referenced paths. Additional checks are not currently scripted here. | Author/review the branding, descriptions, and exposed capabilities when they change. |
| Section 2: collect the service URL and existing identifiers | Automatable with configuration | Read approved values from deployment outputs, pipeline variables, or a configuration store and pass them to the script. | Initially confirm the correct environment, tenant, and resource. |
| Section 2: create the package GUID | Automatable, once | Generate a GUID, save it in persistent deployment configuration, and reuse it for every update. | Decide whether this is an update to an existing plugin or a separate plugin identity. Do not generate a new GUID on every build. |
| Section 2: publisher website, privacy, and terms URLs | Human-owned content; automated reuse | Inject approved names/URLs and check their format and reachability. | Publish and approve the actual publisher/legal content. A URL check cannot establish legal adequacy. |
| Section 2: choose the tenant and pilot users/groups | Human decision; potentially scripted lookup | Resolve approved group/user identifiers from configuration or directory queries. | Approve the deployment audience and later expansion to all users. |
| OAuth prerequisite 1: Entra client registration and delegated permission configuration | Automatable with additional tooling | Use authorized Microsoft Graph/Azure CLI automation to create or update the client and its requested API permissions. No such Cowork registration script is included here. | An authorized owner approves the client and requested permissions. Requested permissions are not the same as granted consent. |
| OAuth prerequisite 1: grant tenant consent | Approval-gated | An authorized administrator can grant consent through supported administrative tooling; this need not always be a portal click. | The permission grant must be authorized under tenant policy. This repository does not automate that approval or grant. |
| OAuth prerequisite 2: register the redirect URI | Automatable with additional tooling | Update the Entra application's redirect URI configuration through Graph/Azure CLI. | Review the target client and callback on initial setup. |
| OAuth prerequisite 3: create the token-store OAuth configuration | Toolkit-automatable; not implemented here | Agents Toolkit supports an `oauth/register` provisioning action. A configured Toolkit project can create the auth configuration and capture its ID instead of portal entry. | Supply approved OAuth settings and authenticate with the necessary rights. Using the Teams developer portal instead is a manual alternative. |
| OAuth prerequisite 4: save the OAuth configuration ID | Automatable | Capture provisioning output into environment-specific deployment configuration and pass it as `-OAuthReferenceId`. | If using portal registration, initially copy and verify the returned ID. |
| Section 3: substitute values and build the ZIP | **Already automated** | Run `package-cowork.ps1` with all parameters. It generates the manifest, copies the package files, compresses them, and cleans up staging. No user prompts are needed when inputs are supplied. | Approve the input values before release; no manual ZIP assembly is required. |
| Section 3: package validation | Partly automated today | The script checks required files, URL formats, and selected inputs. ZIP-layout checks and compatible Microsoft platform validation could be added as pipeline gates. | Resolve validation failures and approve release. The current script does not perform full platform validation. |
| Section 4: install/check Agents Toolkit CLI | Automatable | Install the CLI on a build/developer machine and check its version. | Approve the tooling/version used by the team. No need to install it again for every package. |
| Section 4: `atk auth login` | Interactive in the documented flow | Reuse an existing valid authenticated session where permitted. An unattended identity would require separately supported tooling and tenant configuration. | Complete initial sign-in, MFA, Conditional Access, or reauthentication when required. Never script around those controls. |
| Section 4: personal package installation and returned IDs | CLI-automatable after authentication | Run `atk install --scope Personal` and capture the returned identifiers. | Ensure installation is permitted and the authenticated account is the intended pilot user. This does not assign the plugin tenant-wide. |
| Section 5: upload/submit the package to the tenant catalog | **Automatable with WIQD; not implemented here** | A configured project can run `wiqd plugin publish --env dev` with a tenant-scoped `copilotAgent/publish` lifecycle. This submits the plugin for administrator approval. | An administrator approves the submission. The portal upload wizard remains an alternative; personal CLI installation is not equivalent to catalog submission. |
| Section 5: review components, choose audience, and install | Approval-gated; manual in this repository's rollout | WIQD can separately grant direct access through sharing, but catalog submission does not establish approval or the intended installation/assignment state. | Review the connector and approve availability, pilot scope, and installation in the admin experience. |
| Section 5: inspect deployment, assignments, and publisher policy | Manual in this workflow; checks may be automatable | Supported tenant management/reporting interfaces could be used for inventory and assignment checks after their applicability is verified. | The administrator confirms the deployed plugin and its availability, and approves policy or audience changes. |
| Section 6, steps 1-2: discover and enable the plugin in Cowork | Manual acceptance check | Admin installation can reduce end-user installation work; it does not prove that the Cowork UI shows the expected plugin. | A pilot user confirms visibility and enables/installs it if prompted. |
| Section 6, step 3: connector OAuth sign-in/consent | **User interaction when required** | After initial authorization, the platform can obtain/refresh tokens according to its OAuth configuration and tenant policies. | The user completes required sign-in, MFA, or consent. Admin consent may remove some consent prompts, but not every user authentication requirement. |
| Section 6, steps 4-5: search and follow-up retrieval in Cowork | Manual end-to-end acceptance in this workflow | Repeatable service/connection checks can be scripted separately, but they do not prove Cowork selected and used the plugin correctly. | Run the pilot conversation and confirm both search and retrieval return the intended information through the connector. |
| Section 7, steps 1-3: retain identity, increment version, and rebuild | Automatable with a release process | Persist the package GUID, apply an approved version to the template, and rerun packaging. Version automation is not currently included in the script. | Decide when to release and approve metadata/tool-description changes. |
| Section 7, step 4: submit an update, verify, and broaden rollout | Automatable submission; approval-gated rollout | Use the configured publishing lifecycle for submission and automate rebuild/preflight checks. Verify existing app identity and version behavior before adopting it for updates here. | Approve the tenant change, repeat Cowork acceptance, and authorize expanded availability. |
| Section 7: troubleshoot failures | Mixed | Automatically check package contents, required values, URL reachability, and available deployment diagnostics. | Investigate consent/policy failures and incorrect Cowork behavior; approve configuration or permission changes. |

### What the repository currently automates

The existing [packaging script](../scripts/package-cowork.ps1) is the only dedicated Cowork automation described here. There is no checked-in Cowork Agents Toolkit provisioning lifecycle or publishing pipeline.

In particular:

- The script does not create the Entra client or token-store OAuth configuration.
- Toolkit's `oauth/register` action would require adding and configuring a Toolkit provisioning project. It is not something the packaging script runs today.
- If Toolkit creates an auth configuration, its resulting ID must still be passed into this package's `agentConnectors` reference. Do not assume a separate generated agent/plugin manifest updates this template.
- Microsoft documents WIQD provisioning, package validation, catalog submission, and direct sharing. This repository does not yet configure those operations. Sharing a plugin with users is not the same as catalog publication.
- The documented automated submission path does not establish that this repository has an unattended publishing pipeline, automated assignments/updates, or Cowork UI tests.

### Recommended practical split

1. **One-time setup, with authorized owners:** approve publisher metadata, package identity, target tenant, pilot group, OAuth client, consent, and token-store configuration. Automate the resource-creation operations where supported; keep approval gates.
2. **Every release, unattended build:** load approved environment values, retain the GUID, apply the release version, build the ZIP, run package checks, and retain the release artifact. Only ZIP creation and its basic input checks are implemented today.
3. **Catalog submission, automatable after tooling setup:** use the portal today, or adopt the documented WIQD lifecycle to automate submission. Use only a supported, tenant-approved authentication mechanism for CI.
4. **Acceptance and rollout, human approval:** have an administrator approve the catalog submission and pilot availability, complete required user sign-in, verify search and retrieval in Cowork, then authorize broader availability.

This removes repetitive packaging work without presenting consent, tenant governance, or user acceptance as something the existing script handles.

## References

- [Build plugins for Copilot Cowork](https://learn.microsoft.com/en-us/copilot/cowork/cowork-plugin-development)
- [Manage plugins, skills, and MCP servers](https://learn.microsoft.com/en-us/microsoft-agent-365/admin/manage-plugins-skills-mcp-servers)
- [OAuth plugin authentication](https://learn.microsoft.com/en-us/microsoft-365/copilot/extensibility/plugin-authentication-oauth)
- [Microsoft 365 Agents Toolkit CLI](https://learn.microsoft.com/en-us/microsoftteams/platform/toolkit/microsoft-365-agents-toolkit-cli)
- [WIQD plugin publishing workflow](https://microsoft.github.io/wiqd/getting-started/build-a-plugin/#publishing-and-existing-projects)
- [WIQD publish command reference](https://microsoft.github.io/wiqd/cli/reference/#wiqd-plugin-publish)
- [WIQD pipeline authentication](https://microsoft.github.io/wiqd/getting-started/ci-cd-authentication/)
