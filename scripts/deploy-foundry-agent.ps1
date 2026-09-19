[CmdletBinding()]
param(
    [string]$SubscriptionId = $env:AZURE_SUBSCRIPTION_ID,
    [string]$TenantId = $env:ENTRA_TENANT_ID,
    [string]$Location = $env:AZURE_LOCATION,
    [string]$ResourceGroup = $env:FOUNDRY_BROKER_RESOURCE_GROUP,
    [string]$BrokerClientId = $env:BROKER_CLIENT_ID,
    [string]$CaseApiBaseUrl = $env:API_BASE_URL,
    [string]$CaseApiScope = $env:ENTRA_API_SCOPE,
    [string]$SpaBaseUrl = $env:SPA_BASE_URL,
    [string]$FoundryResourceGroup = $env:FOUNDRY_RESOURCE_GROUP,
    [string]$FoundryAccountName = $env:FOUNDRY_ACCOUNT_NAME,
    [string]$FoundryModelDeployment = $env:FOUNDRY_MODEL_DEPLOYMENT,
    [string]$FoundryProjectName = "oge-refining-case-agent",
    [string]$AgentName = "oge-refining-case-agent"
)

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path $PSScriptRoot -Parent
$envPath = Join-Path $repoRoot ".env"
$az = (Get-Command az.cmd -ErrorAction SilentlyContinue).Source

if (-not $az) {
    $az = (Get-Command az -ErrorAction SilentlyContinue).Source
}
if (-not $az) {
    throw "Azure CLI was not found."
}

if (Test-Path -LiteralPath $envPath) {
    foreach ($line in Get-Content -LiteralPath $envPath) {
        if ($line -match "^\s*([^#][^=]*)=(.*)$") {
            $name = $matches[1].Trim()
            if ([string]::IsNullOrWhiteSpace([Environment]::GetEnvironmentVariable($name, "Process"))) {
                [Environment]::SetEnvironmentVariable($name, $matches[2].Trim(), "Process")
            }
        }
    }
}

if ([string]::IsNullOrWhiteSpace($SubscriptionId)) { $SubscriptionId = $env:AZURE_SUBSCRIPTION_ID }
if ([string]::IsNullOrWhiteSpace($TenantId)) { $TenantId = $env:ENTRA_TENANT_ID }
if ([string]::IsNullOrWhiteSpace($Location)) { $Location = $env:AZURE_LOCATION }
if ([string]::IsNullOrWhiteSpace($ResourceGroup)) { $ResourceGroup = $env:FOUNDRY_BROKER_RESOURCE_GROUP }
if ([string]::IsNullOrWhiteSpace($ResourceGroup)) { $ResourceGroup = "rg-oge-refining-case-foundry-agent" }
if ([string]::IsNullOrWhiteSpace($BrokerClientId)) { $BrokerClientId = $env:BROKER_CLIENT_ID }
if ([string]::IsNullOrWhiteSpace($CaseApiBaseUrl)) { $CaseApiBaseUrl = $env:API_BASE_URL }
if ([string]::IsNullOrWhiteSpace($CaseApiScope)) { $CaseApiScope = $env:ENTRA_API_SCOPE }
if ([string]::IsNullOrWhiteSpace($SpaBaseUrl)) { $SpaBaseUrl = $env:SPA_BASE_URL }
if ([string]::IsNullOrWhiteSpace($FoundryResourceGroup)) { $FoundryResourceGroup = $env:FOUNDRY_RESOURCE_GROUP }
if ([string]::IsNullOrWhiteSpace($FoundryAccountName)) { $FoundryAccountName = $env:FOUNDRY_ACCOUNT_NAME }
if ([string]::IsNullOrWhiteSpace($FoundryModelDeployment)) { $FoundryModelDeployment = $env:FOUNDRY_MODEL_DEPLOYMENT }

$required = @{
    AZURE_SUBSCRIPTION_ID = $SubscriptionId
    ENTRA_TENANT_ID = $TenantId
    AZURE_LOCATION = $Location
    BROKER_CLIENT_ID = $BrokerClientId
    API_BASE_URL = $CaseApiBaseUrl
    ENTRA_API_SCOPE = $CaseApiScope
    SPA_BASE_URL = $SpaBaseUrl
    FOUNDRY_RESOURCE_GROUP = $FoundryResourceGroup
    FOUNDRY_ACCOUNT_NAME = $FoundryAccountName
    FOUNDRY_MODEL_DEPLOYMENT = $FoundryModelDeployment
}
foreach ($setting in $required.GetEnumerator()) {
    if ([string]::IsNullOrWhiteSpace($setting.Value)) {
        throw "Set $($setting.Key) in .env or pass the corresponding parameter."
    }
}

$identityName = "id-oge-refining-case-foundry-agent"
$environmentName = "cae-oge-refining-case-foundry-agent"
$containerAppName = "oge-refining-case-foundry-agent"
$federatedCredentialName = "oge-refining-case-foundry-agent-mi"
$subscriptionHashBytes = [System.Security.Cryptography.SHA256]::Create().ComputeHash(
    [System.Text.Encoding]::UTF8.GetBytes($SubscriptionId)
)
$subscriptionSuffix = ([System.BitConverter]::ToString($subscriptionHashBytes) -replace "-", "").Substring(0, 8).ToLowerInvariant()
$registryName = "acrogerefiningcase$subscriptionSuffix"
$imageTag = (Get-Date).ToUniversalTime().ToString("yyyyMMddHHmmss")
$imageName = "oge-refining-case-foundry-agent:$imageTag"

Push-Location $repoRoot
try {
    & $az account set --subscription $SubscriptionId
    if ($LASTEXITCODE -ne 0) { throw "Unable to select the Azure subscription." }

    foreach ($provider in @("Microsoft.App", "Microsoft.ContainerRegistry", "Microsoft.ManagedIdentity", "Microsoft.OperationalInsights", "Microsoft.CognitiveServices")) {
        & $az provider register --namespace $provider --wait --output none
        if ($LASTEXITCODE -ne 0) { throw "Resource provider registration failed for $provider." }
    }

    & $az group create --name $ResourceGroup --location $Location --output none
    if ($LASTEXITCODE -ne 0) { throw "Unable to create the broker resource group." }

    $identity = & $az identity create `
        --name $identityName `
        --resource-group $ResourceGroup `
        --location $Location `
        --output json | ConvertFrom-Json
    if ($LASTEXITCODE -ne 0) { throw "Unable to create the broker managed identity." }

    $registry = & $az acr create `
        --name $registryName `
        --resource-group $ResourceGroup `
        --location $Location `
        --sku Basic `
        --admin-enabled false `
        --output json | ConvertFrom-Json
    if ($LASTEXITCODE -ne 0) { throw "Unable to create the container registry." }

    & $az containerapp env show --name $environmentName --resource-group $ResourceGroup --output none 2>$null
    if ($LASTEXITCODE -ne 0) {
        & $az containerapp env create `
            --name $environmentName `
            --resource-group $ResourceGroup `
            --location $Location `
            --output none
        if ($LASTEXITCODE -ne 0) { throw "Unable to create the Container Apps environment." }
    }

    $account = & $az cognitiveservices account show `
        --name $FoundryAccountName `
        --resource-group $FoundryResourceGroup `
        --output json | ConvertFrom-Json
    if ($LASTEXITCODE -ne 0) { throw "The parent Foundry account was not found." }

    & $az cognitiveservices account project show `
        --name $FoundryAccountName `
        --project-name $FoundryProjectName `
        --resource-group $FoundryResourceGroup `
        --output none 2>$null
    if ($LASTEXITCODE -ne 0) {
        & $az cognitiveservices account project create `
            --name $FoundryAccountName `
            --project-name $FoundryProjectName `
            --resource-group $FoundryResourceGroup `
            --location $account.location `
            --display-name "OGE Refining Case Agent" `
            --description "Read-only Foundry agent for the OGE Refining Case application." `
            --output none
        if ($LASTEXITCODE -ne 0) { throw "Unable to create the Foundry project." }
    }

    $project = & $az cognitiveservices account project show `
        --name $FoundryAccountName `
        --project-name $FoundryProjectName `
        --resource-group $FoundryResourceGroup `
        --output json | ConvertFrom-Json
    $foundryEndpoint = "https://$($account.properties.customSubDomainName).services.ai.azure.com/api/projects/$FoundryProjectName"

    & $az role assignment create `
        --assignee-object-id $identity.principalId `
        --assignee-principal-type ServicePrincipal `
        --role "Foundry User" `
        --scope $project.id `
        --output none
    if ($LASTEXITCODE -ne 0) { throw "Unable to grant Foundry User to the broker identity." }
    & $az role assignment create `
        --assignee-object-id $identity.principalId `
        --assignee-principal-type ServicePrincipal `
        --role AcrPull `
        --scope $registry.id `
        --output none
    if ($LASTEXITCODE -ne 0) { throw "Unable to grant AcrPull to the broker identity." }

    $brokerApp = & $az ad app list --filter "appId eq '$BrokerClientId'" --output json | ConvertFrom-Json | Select-Object -First 1
    if (-not $brokerApp) { throw "The broker app registration was not found." }
    & $az ad app update --id $brokerApp.id --display-name "OGE Refining Case Foundry Agent Broker" --output none
    if ($LASTEXITCODE -ne 0) { throw "Unable to rename the broker app registration." }

    $credentials = & $az ad app federated-credential list --id $brokerApp.id --output json | ConvertFrom-Json
    $credential = $credentials | Where-Object name -eq $federatedCredentialName
    if (-not $credential) {
        $credentialPath = "C:\temp\oge-refining-case-foundry-fic.json"
        New-Item -ItemType Directory -Path (Split-Path $credentialPath -Parent) -Force | Out-Null
        @{
            name = $federatedCredentialName
            issuer = "https://login.microsoftonline.com/$TenantId/v2.0"
            subject = $identity.principalId
            description = "Trust the OGE Refining Case Foundry broker managed identity."
            audiences = @("api://AzureADTokenExchange")
        } | ConvertTo-Json | Set-Content -LiteralPath $credentialPath -Encoding utf8
        try {
            & $az ad app federated-credential create --id $brokerApp.id --parameters $credentialPath --output none
            if ($LASTEXITCODE -ne 0) { throw "Unable to create the broker federated credential." }
        }
        finally {
            Remove-Item -LiteralPath $credentialPath -Force -ErrorAction SilentlyContinue
        }
    }

    & $az acr build `
        --registry $registryName `
        --image $imageName `
        (Join-Path $repoRoot "src\foundry-agent") `
        --output none
    if ($LASTEXITCODE -ne 0) { throw "The Foundry broker container build failed." }

    $environmentVariables = @(
        "ENTRA_TENANT_ID=$TenantId",
        "BROKER_CLIENT_ID=$BrokerClientId",
        "BROKER_MANAGED_IDENTITY_CLIENT_ID=$($identity.clientId)",
        "BROKER_REQUIRED_SCOPE=agent.invoke",
        "CASE_API_BASE_URL=$($CaseApiBaseUrl.TrimEnd('/'))",
        "CASE_API_SCOPE=$CaseApiScope",
        "FOUNDRY_PROJECT_ENDPOINT=$foundryEndpoint",
        "FOUNDRY_MODEL_DEPLOYMENT=$FoundryModelDeployment",
        "FOUNDRY_AGENT_NAME=$AgentName",
        "AZURE_CLIENT_ID=$($identity.clientId)",
        "CORS_ALLOWED_ORIGINS=$($SpaBaseUrl.TrimEnd('/'))"
    )
    & $az containerapp create `
        --name $containerAppName `
        --resource-group $ResourceGroup `
        --environment $environmentName `
        --image "$($registry.loginServer)/$imageName" `
        --user-assigned $identity.id `
        --registry-server $registry.loginServer `
        --registry-identity $identity.id `
        --ingress external `
        --target-port 8000 `
        --transport auto `
        --cpu 0.5 `
        --memory 1.0Gi `
        --min-replicas 0 `
        --max-replicas 1 `
        --env-vars $environmentVariables `
        --output none
    if ($LASTEXITCODE -ne 0) { throw "Unable to create the Foundry broker Container App." }

    $fqdn = & $az containerapp show `
        --name $containerAppName `
        --resource-group $ResourceGroup `
        --query properties.configuration.ingress.fqdn `
        --output tsv
    $brokerUrl = "https://$fqdn"
    $health = Invoke-WebRequest -Uri "$brokerUrl/health" -UseBasicParsing -TimeoutSec 90
    if ($health.StatusCode -ne 200) { throw "The broker health probe failed." }

    $preflightHeaders = @{
        Origin = $SpaBaseUrl.TrimEnd('/')
        "Access-Control-Request-Method" = "POST"
        "Access-Control-Request-Headers" = "authorization,content-type"
    }
    $preflight = Invoke-WebRequest `
        -Uri "$brokerUrl/api/chat" `
        -Method Options `
        -Headers $preflightHeaders `
        -UseBasicParsing `
        -TimeoutSec 30
    if ($preflight.StatusCode -ne 200 -or $preflight.Headers["Access-Control-Allow-Origin"] -ne $SpaBaseUrl.TrimEnd('/')) {
        throw "The broker CORS preflight did not authorize the SPA origin."
    }

    Write-Host "Foundry broker deployment complete."
    Write-Host "Broker URL: $brokerUrl"
    Write-Host "Health: $($health.Content)"
}
finally {
    Pop-Location
}