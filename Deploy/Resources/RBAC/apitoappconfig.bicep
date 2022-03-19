param appConfigName string
param friendlyName string
param apiName string

@allowed([
  'resourcegroup'
  'subscription'
])
param resourceScope string = 'resourcegroup'

var dataReader = '516239f1-63e1-4d78-a4de-a74fb236a071'

resource productionSlot 'Microsoft.Web/sites@2021-03-01' existing = {
  name: apiName
  scope: resourceGroup()
}

resource stagingSlot 'Microsoft.Web/sites/slots@2021-03-01' existing = {
  name: '${apiName}/Staging'
  scope: resourceGroup()
}

resource role 'Microsoft.Authorization/roleDefinitions@2018-01-01-preview' existing = {
  name: dataReader
  scope: subscription()
}

resource appConfig 'Microsoft.AppConfiguration/configurationStores@2021-10-01-preview' existing = {  
  name: appConfigName
  scope: resourceScope == 'resourcegroup' ? resourceGroup() : subscription()
}

resource roleAssignmentProd 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid(resourceGroup().id, uniqueString('${productionSlot}-${friendlyName}'), role.id)
  scope: appConfig
  properties: {
    roleDefinitionId: role.id
    principalId: productionSlot.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

resource roleAssignmentStaging 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid(resourceGroup().id, uniqueString('${stagingSlot}-${friendlyName}'), role.id)
  scope: appConfig
  properties: {
    roleDefinitionId: role.id
    principalId: stagingSlot.identity.principalId
    principalType: 'ServicePrincipal'
  }
}
