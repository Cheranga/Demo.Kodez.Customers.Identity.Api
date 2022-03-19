param location string = resourceGroup().location
param azConfigName string
param apiEnvironment string
param configurations object
param featureFlags object
param keyVaultReferences object

resource azureAppConfiguration 'Microsoft.AppConfiguration/configurationStores@2021-03-01-preview' = {
  name: azConfigName
  location: location
  sku: {
    name: 'Standard'
  }
  identity: {
    type: 'SystemAssigned'
  }
}

// Configurations
resource appconfigurations 'Microsoft.AppConfiguration/configurationStores/keyValues@2021-03-01-preview' = [for item in configurations.items: {
  name: '${item.name}$${apiEnvironment}'
  properties: {
    value: item.value
  }
  parent: azureAppConfiguration
}]

// Key vault references
resource keyVaultConfigurations 'Microsoft.AppConfiguration/configurationStores/keyValues@2020-07-01-preview' = [for item in keyVaultReferences.items: {
  name: '${item.name}$${apiEnvironment}'
  parent: azureAppConfiguration
  properties: {
    value: string({
      uri: item.value
    })
    contentType: 'application/vnd.microsoft.appconfig.keyvaultref+json;charset=utf-8'
  }
}]

// Feature flags
resource appFeatures 'Microsoft.AppConfiguration/configurationStores/keyValues@2021-03-01-preview' = [for item in featureFlags.items: {
  name: '.appconfig.featureflag~2f${item.id}$${apiEnvironment}'
  properties: {
    value: string(item)
    contentType: 'application/vnd.microsoft.appconfig.ff+json;charset=utf-8'
  }
  parent: azureAppConfiguration
}]


output AppConfigurationUrl string = azureAppConfiguration.properties.endpoint
