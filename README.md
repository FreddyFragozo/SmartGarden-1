# SmartGarden
This project allow me to use an ESP32, an humidity sensor and a soil moisture sensor to control how much water do I use in a plant

## Prerequisites 
1. Computer with .NET 5.0 and VS Code
2. PlatformIO plugin installed in VS Code
3. Azure Account

## Device Schematic
<img src="images/schematic.svg" />

## How to Use
For the web application located in Azure/Web/App/src/appsettings.json configure the following values:

```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "PowerBIOptions": {
    "ApiUrl": "https://api.powerbi.com",
    "DashboardId": "[Power BI Dashboard ID]",
    "WorkspaceId": "[Power BI Workspace ID]",
    "DatasetId": "[Power BI Dataset ID]"
  },
  "AzureADOptions": {
    "TenantUrl": "https://login.microsoftonline.com/[Azure Tenant ID]/",
    "Scope": ["https://analysis.windows.net/powerbi/api/.default"],
    "AuthorityUrl": "https://login.microsoftonline.com/[Azure Tenant ID]/",
    "AuthenticationMode": "masteruser",
    "Password": "[Master Account Password]",
    "Username": "[Master Account Username]"
  },
  "IotHubOptions": {
    "ConnectionString": "[Iot Hub Connection String]"
  }
}

```
