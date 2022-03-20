param location string = resourceGroup().location
param planName string

@allowed([
  'nonprod'
  'prod'
])
param planType string = 'nonprod'

var sku = {
  nonprod: 'S1'
  prod: 'S2'
}

var capacity = {
  nonprod: 1
  prod: 2
}



resource asp_resource 'Microsoft.Web/serverfarms@2021-01-15' = {
  name: planName
  kind: 'app'
  location: location
  sku: {
    capacity: capacity[planType]
    name: sku[planType]    
  }
}
