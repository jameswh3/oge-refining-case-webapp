[CmdletBinding()]
param(
    [string]$ResourceGroup = $env:AZURE_RESOURCE_GROUP,
    [string]$SubscriptionId = $env:AZURE_SUBSCRIPTION_ID,
    [string]$Location = $env:AZURE_LOCATION,
    [string]$TenantId = $env:ENTRA_TENANT_ID,
    [string]$ApiClientId = $env:ENTRA_API_CLIENT_ID,
    [string]$ApiAppObjectId = $env:ENTRA_API_APP_OBJECT_ID,
    [string]$ApiScope = $env:ENTRA_API_SCOPE,
    [string]$SpaClientId = $env:ENTRA_SPA_CLIENT_ID,
    [string]$SpaAppObjectId = $env:ENTRA_SPA_APP_OBJECT_ID,
    [string]$BrokerBaseUrl = $env:BROKER_BASE_URL,
    [string]$BrokerClientId = $env:BROKER_CLIENT_ID,
    [string]$CopilotStudioEnvironmentId = $env:COPILOT_STUDIO_ENVIRONMENT_ID,
    [string]$CopilotStudioSchemaName = $env:COPILOT_STUDIO_SCHEMA_NAME
)

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path $PSScriptRoot -Parent
$az = "C:\Program Files\Microsoft SDKs\Azure\CLI2\wbin\az.cmd"
$federatedCredentialName = "oge-refining-case-api-mi"

if (-not (Test-Path $az)) {
    throw "Azure CLI was not found at $az."
}
foreach ($requiredSetting in @{
    AZURE_RESOURCE_GROUP = $ResourceGroup
    AZURE_SUBSCRIPTION_ID = $SubscriptionId
    AZURE_LOCATION = $Location
    ENTRA_TENANT_ID = $TenantId
    ENTRA_API_CLIENT_ID = $ApiClientId
    ENTRA_API_APP_OBJECT_ID = $ApiAppObjectId
    ENTRA_API_SCOPE = $ApiScope
    ENTRA_SPA_CLIENT_ID = $SpaClientId
    ENTRA_SPA_APP_OBJECT_ID = $SpaAppObjectId
}.GetEnumerator()) {
    if ([string]::IsNullOrWhiteSpace($requiredSetting.Value)) {
        throw "Set $($requiredSetting.Key) or pass the corresponding parameter."
    }
}
if ([string]::IsNullOrWhiteSpace($BrokerBaseUrl) -or [string]::IsNullOrWhiteSpace($BrokerClientId)) {
    throw "Set BROKER_BASE_URL and BROKER_CLIENT_ID or pass the corresponding parameters."
}
if ([string]::IsNullOrWhiteSpace($CopilotStudioEnvironmentId) -or [string]::IsNullOrWhiteSpace($CopilotStudioSchemaName)) {
    throw "Set COPILOT_STUDIO_ENVIRONMENT_ID and COPILOT_STUDIO_SCHEMA_NAME or pass the corresponding parameters."
}

Push-Location $repoRoot
try {
    & $az account set --subscription $SubscriptionId
    if ($LASTEXITCODE -ne 0) { throw "Unable to select Azure subscription $SubscriptionId." }

    foreach ($provider in @("Microsoft.Web", "Microsoft.Sql", "Microsoft.ManagedIdentity", "Microsoft.Insights")) {
        & $az provider register --namespace $provider --wait --output none
        if ($LASTEXITCODE -ne 0) { throw "Resource provider registration failed for $provider." }
    }

    & $az group show --name $ResourceGroup --output none 2>$null
    if ($LASTEXITCODE -ne 0) {
        & $az group create --name $ResourceGroup --location $Location --output none
        if ($LASTEXITCODE -ne 0) { throw "Unable to create resource group $ResourceGroup." }
    }

    & $az deployment group validate `
        --resource-group $ResourceGroup `
        --template-file infra/main.bicep `
        --parameters infra/main.bicepparam `
        --parameters location=$Location tenantId=$TenantId apiClientId=$ApiClientId `
        --output none
    if ($LASTEXITCODE -ne 0) {
        throw "Infrastructure validation failed. Check Microsoft.Web capacity and quota in North Central US."
    }

    $deployment = & $az deployment group create `
        --name "oge-refining-case" `
        --resource-group $ResourceGroup `
        --template-file infra/main.bicep `
        --parameters infra/main.bicepparam `
        --parameters location=$Location tenantId=$TenantId apiClientId=$ApiClientId `
        --output json | ConvertFrom-Json
    if ($LASTEXITCODE -ne 0) { throw "Infrastructure deployment failed." }

    $outputs = $deployment.properties.outputs
    $apiAppName = $outputs.apiAppName.value
    $apiUrl = $outputs.apiUrl.value
    $identityName = $outputs.managedIdentityName.value
    $identityClientId = $outputs.managedIdentityClientId.value
    $identityPrincipalId = $outputs.managedIdentityPrincipalId.value
    $sqlServerName = $outputs.sqlServerName.value
    $sqlServerFqdn = $outputs.sqlServerFqdn.value
    $databaseName = $outputs.sqlDatabaseName.value
    $staticWebAppName = $outputs.staticWebAppName.value
    $staticWebAppUrl = $outputs.staticWebAppUrl.value

    & $az sql server update `
        --resource-group $ResourceGroup `
        --name $sqlServerName `
        --set publicNetworkAccess=Enabled `
        --output none
    if ($LASTEXITCODE -ne 0) { throw "Azure SQL public network configuration failed." }
    & $az sql server wait --resource-group $ResourceGroup --name $sqlServerName --updated
    $sqlPublicNetworkAccess = & $az sql server show `
        --resource-group $ResourceGroup `
        --name $sqlServerName `
        --query publicNetworkAccess `
        --output tsv
    if ($sqlPublicNetworkAccess -ne "Enabled") {
        throw "Azure SQL public network access remains disabled by the subscription. In the portal, open SQL server '$sqlServerName', select Networking > Public access, choose Selected networks, and save before retrying."
    }

    & $az sql server firewall-rule create `
        --resource-group $ResourceGroup `
        --server $sqlServerName `
        --name "AllowAzureServices" `
        --start-ip-address "0.0.0.0" `
        --end-ip-address "0.0.0.0" `
        --output none
    if ($LASTEXITCODE -ne 0) { throw "Azure SQL firewall rule for Azure services failed." }

    $federatedCredentials = & $az ad app federated-credential list `
        --id $apiAppObjectId --output json | ConvertFrom-Json
    if (-not ($federatedCredentials | Where-Object name -eq $federatedCredentialName)) {
        $ficPath = Join-Path $env:TEMP "oge-refining-case-fic.json"
        @{
            name = $federatedCredentialName
            issuer = "https://login.microsoftonline.com/$TenantId/v2.0"
            subject = $identityPrincipalId
            description = "Trust the case API user-assigned managed identity."
            audiences = @("api://AzureADTokenExchange")
        } | ConvertTo-Json | Set-Content $ficPath -Encoding utf8
        & $az ad app federated-credential create --id $apiAppObjectId --parameters $ficPath --output none
        if ($LASTEXITCODE -ne 0) { throw "Federated identity credential creation failed." }
        Remove-Item $ficPath -Force
    }

    $spaPatchPath = Join-Path $env:TEMP "oge-refining-case-spa-patch.json"
    @{
        spa = @{
            redirectUris = @("http://localhost:5173", $staticWebAppUrl)
        }
    } | ConvertTo-Json -Depth 4 | Set-Content $spaPatchPath -Encoding utf8
    & $az rest --method PATCH `
        --url "https://graph.microsoft.com/v1.0/applications/$spaAppObjectId" `
        --headers "Content-Type=application/json" `
        --body "@$spaPatchPath" `
        --output none
    if ($LASTEXITCODE -ne 0) { throw "SPA redirect URI update failed." }
    Remove-Item $spaPatchPath -Force

    if (-not (Get-Command Invoke-Sqlcmd -ErrorAction SilentlyContinue)) {
        Install-Module SqlServer -Scope CurrentUser -Force -AllowClobber
    }
    Import-Module SqlServer

    $operatorIp = (Invoke-RestMethod "https://api.ipify.org").Trim()
    $firewallRuleName = "DeploymentOperator"
    & $az sql server firewall-rule create `
        --resource-group $ResourceGroup `
        --server $sqlServerName `
        --name $firewallRuleName `
        --start-ip-address $operatorIp `
        --end-ip-address $operatorIp `
        --output none
    if ($LASTEXITCODE -ne 0) { throw "Temporary SQL firewall rule creation failed." }

    try {
        $sqlAccessToken = & $az account get-access-token `
            --resource "https://database.windows.net/" `
            --query accessToken `
            --output tsv
        if ($LASTEXITCODE -ne 0) { throw "Unable to acquire an Azure SQL access token." }

        $identitySqlName = $identityName.Replace("]", "]]")
        $identitySqlLiteral = $identityName.Replace("'", "''")
        $sidBytes = ([guid]$identityClientId).ToByteArray() | ForEach-Object { $_.ToString("X2") }
        $sidHex = "0x$($sidBytes -join '')"
        $bootstrapSql = @"
IF EXISTS (
    SELECT 1
    FROM sys.database_principals
    WHERE name = N'$identitySqlLiteral' AND sid <> $sidHex
)
BEGIN
    IF IS_ROLEMEMBER('db_datareader', N'$identitySqlLiteral') = 1
        ALTER ROLE db_datareader DROP MEMBER [$identitySqlName];
    IF IS_ROLEMEMBER('db_datawriter', N'$identitySqlLiteral') = 1
        ALTER ROLE db_datawriter DROP MEMBER [$identitySqlName];
    IF IS_ROLEMEMBER('db_ddladmin', N'$identitySqlLiteral') = 1
        ALTER ROLE db_ddladmin DROP MEMBER [$identitySqlName];
    DROP USER [$identitySqlName];
END;
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'$identitySqlLiteral')
    CREATE USER [$identitySqlName] WITH SID = $sidHex, TYPE = E;
IF IS_ROLEMEMBER('db_datareader', N'$identitySqlLiteral') <> 1
    ALTER ROLE db_datareader ADD MEMBER [$identitySqlName];
IF IS_ROLEMEMBER('db_datawriter', N'$identitySqlLiteral') <> 1
    ALTER ROLE db_datawriter ADD MEMBER [$identitySqlName];
IF IS_ROLEMEMBER('db_ddladmin', N'$identitySqlLiteral') <> 1
    ALTER ROLE db_ddladmin ADD MEMBER [$identitySqlName];
"@
        Invoke-Sqlcmd `
            -ServerInstance "$sqlServerFqdn,1433" `
            -Database $databaseName `
            -AccessToken $sqlAccessToken `
            -Query $bootstrapSql `
            -Encrypt Mandatory `
            -TrustServerCertificate:$false
    }
    finally {
        & $az sql server firewall-rule delete `
            --resource-group $ResourceGroup `
            --server $sqlServerName `
            --name $firewallRuleName `
            --output none
    }

    $artifacts = Join-Path $repoRoot "artifacts"
    $apiPublish = Join-Path $artifacts "api"
    $apiZip = Join-Path $artifacts "api.zip"
    Remove-Item $apiPublish, $apiZip -Recurse -Force -ErrorAction SilentlyContinue
    New-Item $apiPublish -ItemType Directory -Force | Out-Null
    dotnet publish src/api/Oge.Refining.CaseApp.Api/Oge.Refining.CaseApp.Api.csproj `
        --configuration Release `
        --output $apiPublish
    if ($LASTEXITCODE -ne 0) { throw "API publish failed." }
    Compress-Archive -Path "$apiPublish\*" -DestinationPath $apiZip -Force
    & $az webapp deploy `
        --resource-group $ResourceGroup `
        --name $apiAppName `
        --src-path $apiZip `
        --type zip `
        --restart true `
        --output none
    if ($LASTEXITCODE -ne 0) { throw "API deployment failed." }

    Push-Location src/web
    try {
        $env:VITE_ENTRA_TENANT_ID = $TenantId
        $env:VITE_ENTRA_CLIENT_ID = $spaClientId
        $env:VITE_API_SCOPE = $apiScope
        $env:VITE_API_BASE_URL = $apiUrl
        $env:VITE_BROKER_BASE_URL = $BrokerBaseUrl.TrimEnd("/")
        $env:VITE_BROKER_SCOPE = "api://$BrokerClientId/agent.invoke"
        $env:VITE_COPILOT_STUDIO_ENVIRONMENT_ID = $CopilotStudioEnvironmentId
        $env:VITE_COPILOT_STUDIO_SCHEMA_NAME = $CopilotStudioSchemaName
        npm ci
        if ($LASTEXITCODE -ne 0) { throw "SPA dependency installation failed." }
        npm run build
        if ($LASTEXITCODE -ne 0) { throw "SPA build failed." }

        $deploymentToken = & $az staticwebapp secrets list `
            --name $staticWebAppName `
            --resource-group $ResourceGroup `
            --query properties.apiKey `
            --output tsv
        if ($LASTEXITCODE -ne 0) { throw "Unable to retrieve the Static Web Apps deployment token." }
        npx --yes @azure/static-web-apps-cli deploy dist `
            --deployment-token $deploymentToken `
            --env production
        if ($LASTEXITCODE -ne 0) { throw "SPA deployment failed." }
    }
    finally {
        Pop-Location
        Remove-Item Env:VITE_ENTRA_TENANT_ID, Env:VITE_ENTRA_CLIENT_ID, Env:VITE_API_SCOPE, Env:VITE_API_BASE_URL, Env:VITE_BROKER_BASE_URL, Env:VITE_BROKER_SCOPE, Env:VITE_COPILOT_STUDIO_ENVIRONMENT_ID, Env:VITE_COPILOT_STUDIO_SCHEMA_NAME -ErrorAction SilentlyContinue
    }

    $connector = Get-Content openapi/power-platform.swagger.json -Raw | ConvertFrom-Json
    $connector.host = ([uri]$apiUrl).Host
    $connector.securityDefinitions.oauth2.authorizationUrl = "https://login.microsoftonline.com/$TenantId/oauth2/v2.0/authorize"
    $connector.securityDefinitions.oauth2.tokenUrl = "https://login.microsoftonline.com/$TenantId/oauth2/v2.0/token"
    $connector.securityDefinitions.oauth2.scopes = [pscustomobject]@{ $ApiScope = "Read refinery case reports" }
    $connector.security[0].oauth2 = @($ApiScope)
    $connector | ConvertTo-Json -Depth 100 | Set-Content openapi/power-platform.deployed.swagger.json -Encoding utf8

    $health = Invoke-RestMethod "$apiUrl/api/v1/health" -TimeoutSec 120
    Write-Host "Deployment complete."
    Write-Host "API: $apiUrl"
    Write-Host "SPA: $staticWebAppUrl"
    Write-Host "Managed identity client ID: $identityClientId"
    Write-Host "Health: $health"
}
finally {
    Pop-Location
}