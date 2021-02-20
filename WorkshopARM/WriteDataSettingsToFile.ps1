#####################################################################################
# 
# What does this script do?
# This script will output critical information from your Azure resources into the microservice projects
# User Secrets file. These outputs will also be put into the C:\aaworkshop\configuration.txt file
# Although this script is normally executed from the DeployToAzure_v1.ps1 script, it can be executed separately if you
# supply the parameters at runtime. If you wish to run the script separately, change the directory where this file is 
# location (in the command window below) and then do :
# .\WriteDataSettingsToFile.ps1 -YourInitials <yourinitials> -ResourceGroupName <resourcegroupfromAzure> -subscriptionId <subscIdfrom Azure>
#
#####################################################################################Param(
Param(
    [string] [Parameter(Mandatory=$true)] $YourInitials,
    [string] $ResourceGroupName,
    [string] $subscriptionId
)

# **********************************************************************************************
# Should we bounce this script execution?
# **********************************************************************************************
if (($YourInitials -eq '') -or `
    ($ResourceGroupName -eq '') -or `
	($subscriptionId -eq ''))
{
	Write-Host 'You must provide your Initials, Resource Group Name and Subscription ID, before executing' -foregroundcolor Yellow
	exit
}


####################################################
# Modify the file name and location 
####################################################
$filePath="c:\aaworkshop\"
$fileName="configuration.txt"

####################################################
# Switch to write configuration values to the screen.
# Set to False by default. 
# Set to 'True' to write values to screen.
####################################################
$WriteToScreen= "True"

####################################################
# DO NOT CHANGE VALUES BENEATH THIS LINE
####################################################

#If user not logged into Azure account, redirect to login screen
if ([string]::IsNullOrEmpty($(Get-AzureRmContext).Account)) 
{
    Connect-AzAccount 
    $VerbosePreference = "SilentlyContinue"
}


#Helpers
$quote='"'
$colon=':'
$comma=','
$https="https://"
$suffix="activateazure"

#CosmosDB database name
$DBName = -join($YourInitials,'-activateazure')

#If more than one under your account
Select-AzSubscription -SubscriptionId $subscriptionId
#Verify Current Subscription
Get-AzSubscription –SubscriptionId $subscriptionId

# Create new file
$File = New-Item $filePath$fileName -type file -force

Write-Host "----------------------------------------------------------------------------------------------"
Write-Host " Processing, please be patient..."                                     
Write-Host "----------------------------------------------------------------------------------------------" 
Write-Host 

Add-Content -Path $File  "//----------------------------------------------------------------------------------------------"  
Add-Content -Path $File  "//You will need the values below to perform your hands-on lab"  
Add-Content -Path $File  "//Paste these values into your UserSecrets file"  
Add-Content -Path $File  "//----------------------------------------------------------------------------------------------"  
Add-Content -Path $File  " " 
Write-Host

#Get the Storage account name 
#This is your initials plus activateazure  
#https://stackoverflow.com/questions/3896258/how-do-i-output-text-without-a-newline-in-powershell

####################################################
# Set project directory in which script will run
####################################################

# The following constucts set the executing directory
$currentdirectory = get-location

# Get executing directory
$MyDir = $PSScriptRoot
#Write-Host $MyDir

# Move 1 level up in hierarchy
$rootDirectory = $MyDir| Split-Path
#Write-Host "rootDirectory" $rootDirectory

$FullyQualifiedPath = $rootDirectory + "\"
#Write-Host "FullyQualifiedPath" $FullyQualifiedPath

$finalDirectory = Join-Path -Path $FullyQualifiedPath -ChildPath "Basket.Service"
Write-Host "Executing directory" $finalDirectory
Write-Host "----------------------------------------------------------------------------------------------"
Write-Host 


## ADD dotnet command to Path
# Add-Content -Path $Profile.CurrentUserAllHosts -Value '$Env:Path += ";$($env:LOCALAPPDATA)\Local\Microsoft\dotnet\dotnet.exe"'

# Write default ServiceUrls 
# robvet, 6-28-2018, removed "ServiceUrl" JSON tag hierarchy
#Add-Content -Path $File  '"ServiceUrl": {' 
Add-Content -Path $File  '"ApiGateway": "http://localhost:8084",' 
dotnet user-secrets set "ApiGateway" "http://localhost:8084" --project $finalDirectory

Add-Content -Path $File  '"Catalog": "http://localhost:8082",' 
dotnet user-secrets set "Catalog" "http://localhost:8082" --project $finalDirectory

Add-Content -Path $File  '"Basket": "http://localhost:8083",' 
dotnet user-secrets set "Basket" "http://localhost:8083" --project $finalDirectory

Add-Content -Path $File  '"Ordering": "http://localhost:8085",' 
dotnet user-secrets set "Ordering" "http://localhost:8085" --project $finalDirectory

Add-Content -Path $File  " " 
Add-Content -Path $File  " " 

#Build storage acccount settings
$storageAccountHeader="StorageAccount"
$storageKeyHeader="StorageKey"
$storageAccount="$quote$storageAccountHeader$quote$colon$quote$YourInitials$suffix$quote$comma"


Add-Content -Path $File  "//Azure Storage Account Name" 
Add-Content -Path $File  $storageAccount 
Add-Content -Path $File  " " 

if ($WriteToScreen -eq "True") 
{
    Write-Host "//Azure Storage Account Name"
    Write-Host $storageAccount 
    Write-Host 
}

#Get the Storage account key - primary key 
$accountName = "$YourInitials$suffix"
$storagePrimKey = (Get-AzureRmStorageAccountKey -ResourceGroupName $ResourceGroupName -Name $YourInitials$suffix).Value[0] 
$storageAccountKey="$quote$storageKeyHeader$quote$colon$quote$storagePrimKey$quote$comma"
$storageAccountKeywoHeader = "$quote$storagePrimKey$quote"

Add-Content -Path $File  "//----------------------------------------------------------------------------------------------"  
Add-Content -Path $File  "//Azure Storage Account Key" 
Add-Content -Path $File  "//----------------------------------------------------------------------------------------------"  

Add-Content -Path $File  $storageAccountKey 
Add-Content -Path $File  " " 
dotnet user-secrets set $quote$StorageAccountHeader$quote $accountName --project $finalDirectory
dotnet user-secrets set $quote$storageKeyHeader$quote $storageAccountKeywoHeader --project $finalDirectory

if ($WriteToScreen -eq "True") 
{
    Write-Host "//Azure Storage Account Key"
    Write-Host $storageAccountKey
    Write-Host 
}

#Following code fetches storage account connection string -- not needed now, but great to have
##Add-Content -Path $File  "//Azure Storage Connection string" 
##$storageConnectionString= "DefaultEndpointsProtocol=https;AccountName=$accountName;AccountKey=$storagePrimKey;EndpointSuffix=core.windows.net"
##Add-Content -Path $File  $storageConnectionString 

#Get the database connection string for the Catalog database
$catalogConnectionStringHeader="CatalogConnectionString"
$sqlLoginSuffix = "sqllogin"
$sqlPasswordSuffix = "pass@word1$"
$userId = "$YourInitials$sqlLoginSuffix"
$password = "$YourInitials$sqlPasswordSuffix"
$ServerName = "$YourInitials-activateazure.database.windows.net"
$catalogDBString= "Server=tcp:$YourInitials-activateazure.database.windows.net,1433;Initial Catalog=ActivateAzure.Catalog;Persist Security Info=False;User ID=$userId;Password=$password;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
$concatenatedCatalogString = "$quote$catalogConnectionStringHeader$quote$colon$quote$catalogDBString$quote$comma"


if ($WriteToScreen -eq "True") 
{
    Write-Host "//Azure Sql Database credentials" 
    Write-Host "SqlLogin=$userId"
    Write-Host "SqlPassword=$password" 
    Write-Host 
}

Add-Content -Path $File  "//----------------------------------------------------------------------------------------------"  
Add-Content -Path $File  "//Catalog Database Logon Credentials" 
Add-Content -Path $File  "//----------------------------------------------------------------------------------------------"  
Add-Content -Path $File  "//Sql Database Server Name = $ServerName"
Add-Content -Path $File  "//Sql Database Login = $userId"
Add-Content -Path $File  "//Database Password = $password" 
Add-Content -Path $File  " " 

Add-Content -Path $File  "//----------------------------------------------------------------------------------------------"  
Add-Content -Path $File  "//Catalog Database connection string" 
Add-Content -Path $File  "//----------------------------------------------------------------------------------------------"  

Add-Content -Path $File  $concatenatedCatalogString 
Add-Content -Path $File  " " 

if ($WriteToScreen -eq "True") 
{
    Write-Host "//Catalog Database connection string" 
    Write-Host $concatenatedCatalogString 
    Write-Host 
}
# Add database credentials for troubleshooting
dotnet user-secrets set "CatalogConnectionString" $catalogDBString --project $finalDirectory

# Get the Topic connection string
$topicPrefix = "ServiceBusPublisherConnectionString"
$topic = (Get-AzureRmServiceBusKey -ResourceGroupName $ResourceGroupName -Namespace $YourInitials-ActivateAzure -Name RootManageSharedAccessKey).PrimaryConnectionString
$topicSuffix = ";EntityPath=eventbustopic"
$topicString="$quote$topicPrefix$quote$colon$quote$topic$topicSuffix$quote$comma"
$topicStringwoprefix="$quote$topic$topicSuffix$quote"

Add-Content -Path $File  "//-----------------------------------------------------------"  
Add-Content -Path $File  "//Azure Service Bus Topic Connection String" 
Add-Content -Path $File  "//-----------------------------------------------------------"  

Add-Content -Path $File  $topicString 
Add-Content -Path $File  " " 
dotnet user-secrets set "ServiceBusPublisherConnectionString" $topicStringwoprefix --project $finalDirectory

if ($WriteToScreen -eq "True") 
{
    Write-Host "//Azure Service Bus Topic Connection String"
    Write-Host $topicString
    Write-Host 
}

#Get Cosmos connection information
# Get the list of keys for the CosmosDB database
$myKeys = Invoke-AzureRmResourceAction -Action listKeys `
    -ResourceType "Microsoft.DocumentDb/databaseAccounts" `
    -ApiVersion "2016-03-31" `
    -ResourceGroupName $ResourceGroupName `
    -Name $DBName -Force
  
# pull out the primary key
$primaryKey = $myKeys.primaryMasterKey;

# This method 'should' get the connection string but does not return anything 
#Invoke-AzureRmResourceAction -Action listConnectionStrings `
#    -ResourceType "Microsoft.DocumentDb/databaseAccounts" `
#    -ApiVersion "2016-03-31" `
#    -ResourceGroupName $ResourceGroupName `
#    -Name $DBName

# Get the CosmosDB connection URI
$cosmosUriHeader="CosmosEndpoint"
$cosmosUriString="$https$YourInitials-activateazure.documents.azure.com:443"
$cosmosUri="$quote$cosmosUriHeader$quote$colon$quote$cosmosUriString$quote$comma"
$cosmosUriWithHttps="$https$YourInitials-cosmosdb.documents.azure.com:443"


Add-Content -Path $File  "//-----------------------------------------------------------"  
Add-Content -Path $File  "//Azure Cosmos DB Connection URI" 
Add-Content -Path $File  "//-----------------------------------------------------------"  

Add-Content -Path $File  $cosmosUri 
Add-Content -Path $File  " " 

if ($WriteToScreen -eq "True") 
{
    Write-Host "//Azure Cosmos DB Connection URI"
    Write-Host $cosmosUri 
    Write-Host 
}

# Get the CosmosDB Primay Key
$cosmosKeyHeader="CosmosPrimaryKey"
$cosmosKeywHeader="$quote$cosmosKeyHeader$quote$colon$quote$primaryKey$quote$comma"
$cosmosKey="$quote$primaryKey$quote"

Add-Content -Path $File  "//-----------------------------------------------------------"  
Add-Content -Path $File  "//Azure Cosmos DB Primary Key" 
Add-Content -Path $File  "//-----------------------------------------------------------"  

Add-Content -Path $File  $cosmosKeywHeader 
Add-Content -Path $File  " " 
dotnet user-secrets set $quote$cosmosUriHeader$quote $cosmosUriString --project $finalDirectory
dotnet user-secrets set $quote$cosmosKeyHeader$quote $cosmosKey --project $finalDirectory

if ($WriteToScreen -eq "True") 
{
    Write-Host "//Azure Cosmos DB Primary Key"
    Write-Host $cosmosUricosmosKey 
    Write-Host 
}


#Get ACR Resgisty Name
$acrRegistryPrefix="ContainerRegistryName"
$acrRegistryName=$YourInitials+"acractivateazure"
$acrRegistry="$quote$acrRegistryPrefix$quote$colon$quote$acrRegistryName$quote$comma"

Add-Content -Path $File  "//-----------------------------------------------------------"  
Add-Content -Path $File  "//Azure Container Resgistry Name" 
Add-Content -Path $File  "//-----------------------------------------------------------"  

Add-Content -Path $File  $acrRegistry 
Add-Content -Path $File  " " 

if ($WriteToScreen -eq "True") 
{
    Write-Host "//Azure Container Resgistry Name"
    Write-Host $acrRegistry 
    Write-Host 
}

#Get ACR Resgisty Login Server
$acrServerPrefix="ContainerLoginServer"
$acrServerName=$YourInitials+"acractivateazure.azurecr.io"
$acrServer="$quote$acrServerPrefix$quote$colon$quote$acrServerName$quote"

Add-Content -Path $File  "//-----------------------------------------------------------"  
Add-Content -Path $File  "//Azure Container Registry Login Server" 
Add-Content -Path $File  "//-----------------------------------------------------------"  

Add-Content -Path $File  $acrServer 
dotnet user-secrets set $quote$acrRegistryPrefix$quote $quote$acrRegistryName$quote --project $finalDirectory
dotnet user-secrets set $quote$acrServerPrefix$quote $quote$acrServerName$quote --project $finalDirectory

if ($WriteToScreen -eq "True") 
{
    Write-Host "//Azure Container Registry Login Server"
    Write-Host $acrServer 
}

Write-Host "**********************************************************************************************" 
Write-Host "*                                                                                            *" 
Write-Host "* Done                                                                                       *" 
Write-Host "*                                                                                            *" 
Write-Host "* Open '$File' and copy to 'UserSecrets' in reference application  *" 
Write-Host "*                                                                                            *" 
Write-Host "**********************************************************************************************" 

