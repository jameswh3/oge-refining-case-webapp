using './main.bicep'

param location = 'northcentralus'
param staticWebAppLocation = 'eastus2'
param entraAdminObjectId = readEnvironmentVariable('ENTRA_ADMIN_OBJECT_ID')
param entraAdminName = readEnvironmentVariable('ENTRA_ADMIN_NAME')
param tenantId = readEnvironmentVariable('ENTRA_TENANT_ID')
param apiClientId = readEnvironmentVariable('ENTRA_API_CLIENT_ID')
