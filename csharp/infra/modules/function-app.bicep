@description('Name of the Function App.')
param functionAppName string

@description('Name of the App Service Plan.')
param appServicePlanName string

@description('Name of the storage account used by the Function App.')
param storageAccountName string

@description('Application Insights connection string.')
param appInsightsConnectionString string

@description('Azure region for the resources.')
param location string = resourceGroup().location

@description('Tags to apply to all resources.')
param tags object = {}

@description('Runtime for the Function App.')
@allowed(['dotnet-isolated', 'python', 'java', 'node', 'powershell'])
param functionAppRuntime string = 'dotnet-isolated'

@description('Runtime version (e.g. 10.0 for .NET 10, 3.11 for Python).')
param functionAppRuntimeVersion string = '10.0'

@description('Additional app settings for the Function App.')
param extraAppSettings appSettingType[] = []

type appSettingType = {
  name: string
  value: string
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-05-01' existing = {
  name: storageAccountName
}

// Consumption plan (Y1) on Linux
resource appServicePlan 'Microsoft.Web/serverfarms@2024-04-01' = {
  name: appServicePlanName
  location: location
  tags: tags
  kind: 'linux'
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
  }
  properties: {
    reserved: true
  }
}

var storageConnectionString = 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${storageAccount.listKeys().keys[0].value}'

var defaultAppSettings = [
  { name: 'AzureWebJobsStorage', value: storageConnectionString }
  { name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING', value: storageConnectionString }
  { name: 'WEBSITE_CONTENTSHARE', value: toLower(functionAppName) }
  { name: 'FUNCTIONS_EXTENSION_VERSION', value: '~4' }
  { name: 'FUNCTIONS_WORKER_RUNTIME', value: functionAppRuntime }
  { name: 'APPLICATIONINSIGHTS_CONNECTION_STRING', value: appInsightsConnectionString }
]

resource functionApp 'Microsoft.Web/sites@2024-04-01' = {
  name: functionAppName
  location: location
  tags: tags
  kind: 'functionapp,linux'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      appSettings: concat(defaultAppSettings, extraAppSettings)
      linuxFxVersion: '${toUpper(functionAppRuntime)}|${functionAppRuntimeVersion}'
      use32BitWorkerProcess: false
      ftpsState: 'FtpsOnly'
      minTlsVersion: '1.2'
    }
  }
}

output functionAppId string = functionApp.id
output functionAppName string = functionApp.name
output principalId string = functionApp.identity.principalId
output defaultHostname string = functionApp.properties.defaultHostName
