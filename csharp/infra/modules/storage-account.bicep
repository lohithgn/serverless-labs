@description('Name of the storage account (lowercase alphanumeric, 3-24 chars).')
@minLength(3)
@maxLength(24)
param name string

@description('Azure region for the storage account.')
param location string = resourceGroup().location

@description('Tags to apply to the storage account.')
param tags object = {}

@description('Access tier for blob storage.')
@allowed(['Hot', 'Cool'])
param accessTier string = 'Hot'

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: name
  location: location
  tags: tags
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    accessTier: accessTier
    allowBlobPublicAccess: false
    allowSharedKeyAccess: true
    minimumTlsVersion: 'TLS1_2'
    supportsHttpsTrafficOnly: true
    networkAcls: {
      bypass: 'AzureServices'
      defaultAction: 'Allow'
    }
  }
}

output name string = storageAccount.name
output primaryEndpoints object = storageAccount.properties.primaryEndpoints
