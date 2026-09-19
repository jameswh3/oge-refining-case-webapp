targetScope = 'resourceGroup'

@description('Primary Azure region for App Service, SQL, identity, and monitoring.')
param location string = 'northcentralus'

@description('Static Web Apps region. East US 2 supports Microsoft.Web/staticSites.')
param staticWebAppLocation string = 'eastus2'

@description('Microsoft Entra object ID of the user who bootstraps Azure SQL.')
param entraAdminObjectId string

@description('Display name or UPN of the Azure SQL Entra administrator.')
param entraAdminName string

@description('Microsoft Entra tenant ID.')
param tenantId string

@description('Case API application registration client ID.')
param apiClientId string

var suffix = uniqueString(subscription().id, resourceGroup().name)
var apiName = 'oge-refining-case-api-${suffix}'
var planName = 'oge-refining-case-plan-${suffix}'
var identityName = 'oge-refining-case-api-id-${suffix}'
var sqlName = 'ogerefiningcase${suffix}'
var databaseName = 'OgeRefiningCases'
var staticWebAppName = 'oge-refining-case-web-${suffix}'
var workspaceName = 'oge-refining-case-logs-${suffix}'
var insightsName = 'oge-refining-case-insights-${suffix}'

resource identity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
  name: identityName
  location: location
}

resource workspace 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name: workspaceName
  location: location
  properties: {
    retentionInDays: 30
    features: {
      enableLogAccessUsingOnlyResourcePermissions: true
    }
  }
  #disable-next-line BCP187
  sku: {
    name: 'PerGB2018'
  }
}

resource insights 'Microsoft.Insights/components@2020-02-02' = {
  name: insightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: workspace.id
  }
}

resource plan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: planName
  location: location
  kind: 'app'
  sku: {
    name: 'F1'
    tier: 'Free'
    capacity: 1
  }
  properties: {}
}

resource staticWebApp 'Microsoft.Web/staticSites@2023-12-01' = {
  name: staticWebAppName
  location: staticWebAppLocation
  sku: {
    name: 'Free'
    tier: 'Free'
  }
  properties: {
    allowConfigFileUpdates: true
  }
}

resource sqlServer 'Microsoft.Sql/servers@2023-08-01' = {
  name: sqlName
  location: location
  properties: {
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
    restrictOutboundNetworkAccess: 'Disabled'
    version: '12.0'
    administrators: {
      administratorType: 'ActiveDirectory'
      principalType: 'User'
      login: entraAdminName
      sid: entraAdminObjectId
      tenantId: tenantId
      azureADOnlyAuthentication: true
    }
  }
}

resource database 'Microsoft.Sql/servers/databases@2023-08-01' = {
  parent: sqlServer
  name: databaseName
  location: location
  sku: {
    name: 'Basic'
    tier: 'Basic'
    capacity: 5
  }
  properties: {
    maxSizeBytes: 2147483648
    zoneRedundant: false
  }
}

resource api 'Microsoft.Web/sites@2023-12-01' = {
  name: apiName
  location: location
  kind: 'app'
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${identity.id}': {}
    }
  }
  properties: {
    serverFarmId: plan.id
    httpsOnly: true
    publicNetworkAccess: 'Enabled'
    clientAffinityEnabled: false
    siteConfig: {
      alwaysOn: false
      ftpsState: 'Disabled'
      http20Enabled: true
      netFrameworkVersion: 'v8.0'
      minimumElasticInstanceCount: 0
      healthCheckPath: '/api/v1/health'
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Production'
        }
        {
          name: 'WEBSITE_RUN_FROM_PACKAGE'
          value: '1'
        }
        {
          name: 'Authentication__Enabled'
          value: 'true'
        }
        {
          name: 'Authentication__TenantId'
          value: tenantId
        }
        {
          name: 'Authentication__ClientId'
          value: apiClientId
        }
        {
          name: 'Authentication__Audience'
          value: 'api://${apiClientId}'
        }
        {
          name: 'Authentication__RequiredScope'
          value: 'case.read'
        }
        {
          name: 'Database__Provider'
          value: 'SqlServer'
        }
        {
          name: 'ConnectionStrings__Cases'
          value: 'Server=tcp:${sqlServer.properties.fullyQualifiedDomainName},1433;Initial Catalog=${databaseName};Authentication=Active Directory Managed Identity;User Id=${identity.properties.clientId};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
        }
        {
          name: 'Obo__TenantId'
          value: tenantId
        }
        {
          name: 'Obo__ClientId'
          value: apiClientId
        }
        {
          name: 'Obo__ManagedIdentityClientId'
          value: identity.properties.clientId
        }
        {
          name: 'Obo__GraphScope'
          value: 'https://graph.microsoft.com/.default'
        }
        {
          name: 'Cors__AllowedOrigins__0'
          value: 'https://${staticWebApp.properties.defaultHostname}'
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: insights.properties.ConnectionString
        }
      ]
    }
  }
}

output apiAppName string = api.name
output apiUrl string = 'https://${api.properties.defaultHostName}'
output managedIdentityName string = identity.name
output managedIdentityClientId string = identity.properties.clientId
output managedIdentityPrincipalId string = identity.properties.principalId
output sqlServerName string = sqlServer.name
output sqlServerFqdn string = sqlServer.properties.fullyQualifiedDomainName
output sqlDatabaseName string = database.name
output staticWebAppName string = staticWebApp.name
output staticWebAppUrl string = 'https://${staticWebApp.properties.defaultHostname}'
