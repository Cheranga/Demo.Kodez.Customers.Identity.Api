param buildNumber string
param environmentName string
param location string = resourceGroup().location
param apiName string
param tableNames string


@allowed([
  'nonprod'
  'prod'
])
param envType string = 'nonprod'

var appInsName = 'ins-${apiName}-${environmentName}'
var sgName = replace('sg${apiName}${environmentName}','-','')

// Storage Account
module storageAccountModule 'StorageAccount/template.bicep' = {
  name: '${buildNumber}-storage-account'
  params: {
    name: sgName
    tables: tableNames
    location: location
    storageType:envType
  }
}

// Application Insights
module applicationInsightsModule 'ApplicationInsights/template.bicep' = {
  name: '${buildNumber}-application-insights'
  params: {
    name: appInsName
    location: location
  }
}
