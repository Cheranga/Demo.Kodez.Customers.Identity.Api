param appConfigName string
param rbacAccess object

var dataReader = '516239f1-63e1-4d78-a4de-a74fb236a071'

resource role 'Microsoft.Authorization/roleDefinitions@2018-01-01-preview' existing = {
  name: dataReader
  scope: subscription()
}

resource appConfig 'Microsoft.AppConfiguration/configurationStores@2021-10-01-preview' existing = {  
  name: appConfigName
}

resource roleAssignment 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = [for item in rbacAccess.items:{
  name: guid(resourceGroup().id, uniqueString('${item.objectId}-${item.friendlyName}'), role.id)
  scope: appConfig
  properties: {
    roleDefinitionId: role.id
    principalId: item.objectId
    principalType: 'ServicePrincipal'
  }
}]
