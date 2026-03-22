@description('Name of the Cosmos DB account.')
param name string

@description('Azure region for the Cosmos DB account.')
param location string = resourceGroup().location

@description('Tags to apply to the Cosmos DB account.')
param tags object = {}

@description('Name of the database to create.')
param databaseName string

@description('Name of the assets container.')
param assetsContainerName string = 'assets'

@description('Partition key path for the assets container.')
param assetsPartitionKeyPath string = '/department'

@description('Name of the leases container (for change feed).')
param leasesContainerName string = 'leases'

@description('Partition key path for the leases container.')
param leasesPartitionKeyPath string = '/id'

resource cosmosAccount 'Microsoft.DocumentDB/databaseAccounts@2024-05-15' = {
  name: name
  location: location
  tags: tags
  kind: 'GlobalDocumentDB'
  properties: {
    databaseAccountOfferType: 'Standard'
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
    }
    locations: [
      {
        locationName: location
        failoverPriority: 0
        isZoneRedundant: false
      }
    ]
    capabilities: [
      { name: 'EnableServerless' }
    ]
    enableAutomaticFailover: false
    enableMultipleWriteLocations: false
    publicNetworkAccess: 'Enabled'
  }
}

resource database 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2024-05-15' = {
  parent: cosmosAccount
  name: databaseName
  properties: {
    resource: {
      id: databaseName
    }
  }
}

resource assetsContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2024-05-15' = {
  parent: database
  name: assetsContainerName
  properties: {
    resource: {
      id: assetsContainerName
      partitionKey: {
        paths: [assetsPartitionKeyPath]
        kind: 'Hash'
      }
      uniqueKeyPolicy: {
        uniqueKeys: [
          { paths: ['/assetTag'] }
        ]
      }
      indexingPolicy: {
        indexingMode: 'consistent'
        includedPaths: [
          { path: '/assetTag/?' }
          { path: '/name/?' }
          { path: '/department/?' }
          { path: '/status/?' }
          { path: '/type/?' }
        ]
        excludedPaths: [
          { path: '/*' }
        ]
      }
    }
  }
}

resource leasesContainer 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2024-05-15' = {
  parent: database
  name: leasesContainerName
  properties: {
    resource: {
      id: leasesContainerName
      partitionKey: {
        paths: [leasesPartitionKeyPath]
        kind: 'Hash'
      }
      indexingPolicy: {
        indexingMode: 'consistent'
        includedPaths: []
        excludedPaths: [
          { path: '/*' }
        ]
      }
    }
  }
}

output endpoint string = cosmosAccount.properties.documentEndpoint
output accountName string = cosmosAccount.name
output accountId string = cosmosAccount.id
output databaseName string = database.name
output assetsContainerName string = assetsContainer.name
output leasesContainerName string = leasesContainer.name
