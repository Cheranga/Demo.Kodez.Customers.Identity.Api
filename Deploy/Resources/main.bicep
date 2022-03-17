param buildNumber string
param location string = resourceGroup().location
param sgName string
param tableNames string

@allowed([
  'nonprod'
  'prod'
])
param envType string = 'nonprod'

module storageAccountModule 'StorageAccount/template.bicep' = {
  name: '${buildNumber}-storage-account'
  params: {
    name: sgName
    tables: tableNames
    location: location
    storageType:envType
  }
}
