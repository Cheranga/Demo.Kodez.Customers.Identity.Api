param name string
param location string = resourceGroup().location
param tables string

var tablesArray = empty(tables)? [] : split(tables, ',')

@allowed([
  'nonprod'
  'prod'
])
param storageType string = 'nonprod'

var storageSku = {
  nonprod: 'Standard_LRS'
  prod: 'Standard_GRS'
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
  name: name
  location: location
  kind: 'StorageV2'
  sku: {
    name: storageSku[storageType]
  }
}

resource tableService 'Microsoft.Storage/storageAccounts/tableServices@2021-08-01' = if (!empty(tablesArray)) {
  name: '${name}/default'
  dependsOn: [
    storageAccount
  ]
  resource storageTable 'tables' = [for t in tablesArray: {
    name: t
  }]
}

