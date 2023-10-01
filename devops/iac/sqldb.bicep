//******************************************************************
//INPUT PARAMETERS 
//******************************************************************


param sharedResourceGroupName string = resourceGroup().name

//DEPLOYMENT ENVIRONMENT
param envCode string = 'd'

// Generic DB Settings
param serverName string = ''
param sqlDBName string = ''
param maxSizeBytes int = 2147483648
param databaseCollation string = 'SQL_Latin1_General_CP1_CI_AS'
param dbSkuName string = 'Basic'
param dbSkuTier string = 'Basic'
param dbSkuCapacity int = 5

//Diagnostic settings
param logAnalyticsWorkspaceName string = format('dnb-monitor-d-log-01')
param diagnosticLogRetentionPeriod int = 30
param diagnosticSettingPurpose string = 'monitoring'

//RESOURCE TAGS
var resourceTags = {
  description: 'SQL database for the dnb - its de nicest bank!'
  criticality: 'Low'
  sla: '24hours'
  confidentiality: 'Private'
  opsCommitment: 'To be defined'
  opsTeam: 'To be defined'
  ownership: 'dnb'
  creator: 'devops-arm-d'
  costCenter: 'NA'
  businessUnit: 'NA'
  environment: 'd'
}

//******************************************************************
//VARIABLES
//******************************************************************
var parentResourceTags =  {
    environment: resourceTags.environment
    opsCommitment: resourceTags.opsCommitment
    opsTeam: resourceTags.opsTeam
    ownership: resourceTags.ownership
    creator: resourceTags.creator
    costCenter: resourceTags.costCenter
    businessUnit: resourceTags.businessUnit
}

var diagnosticLogRetentionForLifecycleManagement = 0 

//******************************************************************
//RESOURCES & MODULES 
//******************************************************************
resource sharedResourceGroup 'Microsoft.Resources/resourceGroups@2022-09-01' existing = {
    name: sharedResourceGroupName
    scope:subscription()
}

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2020-03-01-preview' existing = {
  name: logAnalyticsWorkspaceName
  scope: resourceGroup(sharedResourceGroupName)
}

resource sqlServer 'Microsoft.Sql/servers@2019-06-01-preview' existing = {
  name: serverName
}

//******************************************************************
resource sqlDatabase 'Microsoft.Sql/servers/databases@2020-08-01-preview' = {
  name: sqlDBName
  parent: sqlServer
  location: resourceGroup().location
  tags: resourceTags
  sku: {
    name: dbSkuName
    tier: dbSkuTier
    capacity: dbSkuCapacity
  }
  properties: {
    collation: databaseCollation
    maxSizeBytes: maxSizeBytes
  }
}

//******************************************************************
var diagnosticSettingResourceTags =  {
  description: 'Diagnostic settings for SQL Database ${sqlDBName}'
  criticality: 'Low'
  sla: 'Not applicable'
  confidentiality: 'Public'
}

// Diagnostic Settings with connection to Log Analytics worksspace
resource diagnosticSetting 'microsoft.insights/diagnosticSettings@2021-05-01-preview' = {
  scope: sqlDatabase
  name: format('{0}.{1}-diagset',sqlDBName,diagnosticSettingPurpose) 
  tags: union(diagnosticSettingResourceTags,parentResourceTags)
  properties: {
    workspaceId: logAnalyticsWorkspace.id
     logs: [
      {
        category: 'SQLInsights'
        enabled: true
        retentionPolicy: {
          days: diagnosticLogRetentionForLifecycleManagement 
          enabled: true
        }
      }

      {
        category: 'AutomaticTuning'
        enabled: true
        retentionPolicy: {
          days: diagnosticLogRetentionForLifecycleManagement
          enabled: true
        }
      }
      {
        category: 'QueryStoreRuntimeStatistics'
        enabled: true
        retentionPolicy: {
          days: diagnosticLogRetentionForLifecycleManagement
          enabled: true
        }
      }
      {
        category: 'QueryStoreWaitStatistics'
        enabled: true
        retentionPolicy: {
          days: diagnosticLogRetentionForLifecycleManagement
          enabled: true
        }
      }
      {
        category: 'Errors'
        enabled: true
        retentionPolicy: {
          days: diagnosticLogRetentionForLifecycleManagement
          enabled: true
        }
      }
        {
        category: 'DatabaseWaitStatistics'
        enabled: true
        retentionPolicy: {
          days: diagnosticLogRetentionForLifecycleManagement
          enabled: true
        }
        }
        {
        category: 'Timeouts'
        enabled: true
        retentionPolicy: {
          days: diagnosticLogRetentionForLifecycleManagement
          enabled: true
        }
      }
      {
        category: 'Blocks'
        enabled: true
        retentionPolicy: {
          days: diagnosticLogRetentionForLifecycleManagement
          enabled: true
        }
      }
      {
        category: 'Deadlocks'
        enabled: true
        retentionPolicy: {
          days: diagnosticLogRetentionForLifecycleManagement
          enabled: true
        }
      }
    ]
    metrics: [
      {
        category: 'Basic'
        enabled: true
      }
      {
        category: 'InstanceAndAppAdvanced'
        enabled: true
      }
      {
        category: 'WorkloadManagement'
        enabled: true
      }
    ]
  }
}
