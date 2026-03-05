targetScope = 'subscription'

@minLength(1)
@maxLength(64)
@description('Environment name used for resource naming (e.g. dev, staging, prod).')
param environmentName string

@minLength(1)
@description('Primary Azure region for all resources.')
param location string

@description('Override the default resource group name.')
param resourceGroupName string = ''

@description('Override the default Cosmos DB account name.')
param cosmosAccountName string = ''

@description('Name of the Cosmos DB database.')
param cosmosDatabaseName string = 'inventory'

var tags = {
  'azd-env-name': environmentName
}
var token = uniqueString(subscription().id, environmentName, location)
var prefix = '${environmentName}-${token}'

// Resource Group
resource rg 'Microsoft.Resources/resourceGroups@2024-03-01' = {
  name: !empty(resourceGroupName) ? resourceGroupName : 'rg-${prefix}'
  location: location
  tags: tags
}

// Monitoring — Log Analytics + Application Insights
module monitoring 'modules/monitoring.bicep' = {
  scope: rg
  params: {
    logAnalyticsName: 'log-${prefix}'
    appInsightsName: 'appi-${prefix}'
    location: location
    tags: tags
  }
}

// Storage account for the Function App
module storage 'modules/storage-account.bicep' = {
  scope: rg
  params: {
    // Storage names: lowercase alphanumeric only, 3-24 chars
    name: 'st${uniqueString(rg.id, environmentName)}'
    location: location
    tags: tags
  }
}

// Function App (Consumption plan, .NET isolated worker)
module functionApp 'modules/function-app.bicep' = {
  scope: rg
  params: {
    functionAppName: 'func-${prefix}'
    appServicePlanName: 'asp-${prefix}'
    storageAccountName: storage.outputs.name
    appInsightsConnectionString: monitoring.outputs.appInsightsConnectionString
    location: location
    tags: tags
    functionAppRuntime: 'dotnet-isolated'
    functionAppRuntimeVersion: '10.0'
  }
}

// Cosmos DB — serverless account with containers for all labs
module cosmosDb 'modules/cosmos-db.bicep' = {
  scope: rg
  params: {
    name: !empty(cosmosAccountName) ? cosmosAccountName : 'cosmos-${prefix}'
    location: location
    tags: tags
    databaseName: cosmosDatabaseName
  }
}

// Outputs
output resourceGroupName string = rg.name
output functionAppName string = functionApp.outputs.functionAppName
output functionAppHostname string = functionApp.outputs.defaultHostname
output cosmosDbEndpoint string = cosmosDb.outputs.endpoint
output cosmosDbAccountName string = cosmosDb.outputs.accountName
output cosmosDatabaseName string = cosmosDb.outputs.databaseName
output productsContainerName string = cosmosDb.outputs.productsContainerName
output leasesContainerName string = cosmosDb.outputs.leasesContainerName
