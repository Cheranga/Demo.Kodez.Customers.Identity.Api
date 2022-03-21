@secure()
param productionId string

@secure()
param stagingId string

@secure()
param azureAppConfigurationId string
param kvName string


resource apiToKeyVaultAccess 'Microsoft.KeyVault/vaults/accessPolicies@2021-11-01-preview' = {
  name: '${kvName}/add'
  properties: {
    accessPolicies: [
      {
        tenantId: subscription().tenantId
        objectId: productionId
        permissions: {
          secrets: [
            'get'
            'list'
          ]
        }
      }
      {
        tenantId: subscription().tenantId
        objectId: stagingId
        permissions: {
          secrets: [
            'get'
            'list'
          ]
        }
      }
      {
        tenantId: subscription().tenantId
        objectId: azureAppConfigurationId
        permissions: {
          secrets: [
            'get'
            'list'
          ]
        }
      }
    ]
  }
}
