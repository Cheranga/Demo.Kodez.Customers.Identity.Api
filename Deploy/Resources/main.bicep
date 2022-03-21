param buildNumber string
param environmentName string
param apiEnvironmentName string
param location string = resourceGroup().location
param apiName string
param tableNames string
param sharedConfig string

@allowed([
  'nonprod'
  'prod'
])
param envType string = 'nonprod'

var appInsName = 'ins-${apiName}-${environmentName}'
var sgName = replace('sg${apiName}${environmentName}', '-', '')
var aspName = 'plan-${apiName}-${environmentName}'
var kvName = 'kv-${apiName}-${environmentName}'
var customerIdentityApiName = 'api-${apiName}-${environmentName}'
var identityConfigName = 'appcs-cc-${apiName}-${environmentName}'

// Storage Account
module storageAccountModule 'StorageAccount/template.bicep' = {
  name: '${buildNumber}-storage-account'
  params: {
    name: sgName
    tables: tableNames
    location: location
    storageType: envType
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

// Application Service Plan
module appServicePlanModule 'AppServicePlan/template.bicep' = {
  name: '${buildNumber}-application-service-plan'
  params: {
    planName: aspName
    location: location
    planType: envType
  }
}

// Key Vault
module keyVaultModule 'KeyVault/template.bicep' = {
  name: '${buildNumber}-key-vault'
  params: {
    keyVaultName: kvName
    location: location
    secretData: {
      items: [
        {
          name: 'TableStorageConnectionString'
          value: 'DefaultEndpointsProtocol=https;AccountName=${sgName};AccountKey=${listKeys(resourceId(resourceGroup().name, 'Microsoft.Storage/storageAccounts', sgName), '2019-04-01').keys[0].value};EndpointSuffix=core.windows.net'
        }
        {
          name: 'AppInsightsKey'
          value: applicationInsightsModule.outputs.appInsightsKey
        }
      ]
    }
  }
  dependsOn: [
    storageAccountModule
  ]
}

// Azure App Configuration
module azureAppConfigurationModule 'AppConfiguration/template.bicep' = {
  name: '${buildNumber}-azure-app-configuration'
  params: {
    location: location
    apiEnvironment: apiEnvironmentName
    azConfigName: identityConfigName
    configurations: {
      items: [
        {
          name: 'somesetting'
          value: 'some value'
        }
      ]
    }
    featureFlags: {
      items: [
        {
          id: 'NewUserPromotion'
          description: 'new user promotion'
          enabled: true
        }
      ]
    }
    keyVaultReferences: {
      items: [
        {
          name: 'TableConfig:Connection'
          value: '${keyVaultModule.outputs.keyVaultUri}secrets/TableStorageConnectionString'
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: '${keyVaultModule.outputs.keyVaultUri}secrets/AppInsightsKey'
        }
      ]
    }
  }
}

// Customer Identity API
module customerIdentityAPI 'API/template.bicep' = {
  name: '${buildNumber}-${apiName}-${environmentName}'
  params: {
    location: location
    apiEnvironment: apiEnvironmentName
    apiName: customerIdentityApiName
    identityApiConfigUrl: azureAppConfigurationModule.outputs.AppConfigurationUrl
    planName: aspName
    sharedConfigUrl: 'https://${sharedConfig}.azconfig.io'
  }
}

// Give API read access to the identity Azure app configuration
module rbacApiToCustApiConfig 'RBAC/apitoappconfig.bicep' = {
  name: '${buildNumber}-rbac-api-to-identity-config'
  params: {
    appConfigName: identityConfigName
    rbacAccess: {
      items: [
        {
          objectId: customerIdentityAPI.outputs.ProductionObjectId
          friendlyName: 'prod slot to azure app config'
        }
        {
          objectId: customerIdentityAPI.outputs.StagingObjectId
          friendlyName: 'staging slot to azure app config'
        }
      ]
    }
  }
  dependsOn: [
    customerIdentityAPI
    azureAppConfigurationModule
  ]
}

module accessToKeyVault 'KeyVault/accesspolicies.bicep' = {
  name: '${buildNumber}-access-to-keyvault'
  params: {
    azureAppConfigurationId: azureAppConfigurationModule.outputs.ObjectId
    kvName: kvName
    productionId: customerIdentityAPI.outputs.ProductionObjectId
    stagingId: customerIdentityAPI.outputs.StagingObjectId
  }
}
