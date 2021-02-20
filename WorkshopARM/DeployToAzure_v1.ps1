#####################################################################################
#
# What does this script do?
# * This script is used to kick off the process for creating the Azure resources that
#  you will need for the workshop. The script itself does not have individual commands
#  for each Azure resource, the script will get this information from the ARM template
#  microservices.json, located in the WorkshopARM folder. The student should not modify
#  the settings in the ARM template.
# 
# * If you are using Cloud Slice, the resource group that begins with 'rgServices-' will 
#  be used for the resources. If a student has their own subscription, the resource group
#  will be named 'rgServices-<your-initials>'
#
#*  Lastly, this script calls the WriteDataSettingsToFile.ps1 to fill in User Secrets information
#  If you would like to validate that your ARM template is formatted correctly without
#  actually doing the deployment, change the $ValidateOnly variable to $true
#
# This script requires that you provide 3 pieces of information:
# 1. Your initials, minimum 5 characters. No special characters like hypens
# 2. Your subscription ID. You can get this from the Azure portal
# 3. The Azure region that you want your resources deployed in
# 
# NOTE: This script is intended to be executed in a Learn-on-Demand environment where
# the path to the projects ARM template is known. If you are using your own hardware
# to perform the labs, you need to change the variable $rootARMPath value to the 
# path on your own machine. This variable is on line 66.
#
# SUGGESTION: If you make any changes to the ARM template, always run this script with
# $ValidateOnly set to $true first
#####################################################################################
# VARIABLES YOU NEED TO SET
#####################################################################################

# Your initials, use at least 5 characters
# No special characters like hypen or &
# Must start with a letter of the alphabet
$yourInitials = Read-Host "Enter your initials"

# Subscription ID 
$subscriptionId = Read-Host "Enter your your subscription id"

# Azure region where you want the resources deployed
# You can retrieve location abbreviations from https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.documents.locationnames.eastus?view=azure-dotnet
$location = "westeurope"

# **********************************************************************************************
# Should we bounce this script execution?
# **********************************************************************************************
if (($yourInitials -eq '') -or `
    ($yourInitials.Length -le 4) -or `
    ($subscriptionId -eq '') -or `
    ($location -eq ''))
{
	Write-Host 'You must provide your Initials, Subscription ID and Azure region/location before executing' -foregroundcolor Yellow
	exit
}


### BEGIN UPDATED VERSION
if (Get-Module -Name Az -ListAvailable) {
       Write-Warning -Message ('Az module not installed. Having both the AzureRM and ' +
      'Az modules installed at the same time is not supported.')
} else {
    Install-Module -Name Az -AllowClobber -Scope CurrentUser
}
### END UPDATED VERSION

#####################################################################################
# DO NOT CHANGE ANY OF THE VARIABLES BELOW
#####################################################################################
#####################################################################################
#  IF YOU ARE NOT USING LOD FOR THE WORKSHOP LABS, YOU WILL NEED TO CHANGE THE
#  $rootARMPath path to your own machines WorkshopARM path
#####################################################################################
$rootARMPath = Get-Location
$templatePath = Join-Path -Path $rootARMPath -ChildPath "microservices.json"

$ValidateOnly = $false

# For the resource group, it needs to have a prefix name of rgServices
$resourceGroupName = "rgServices-"

# Deployment name used by the ARM template
$deploymentName = $yourInitials + "deploy" + ((Get-Date).ToUniversalTime()).ToString('MMdd-HHmm')


# These parameters will be used with your ARM template
$paramObject = @{
    'yourInitials' = $yourInitials
    'location' = $location
}

#If user not logged into Azure account, redirect to login screen
if ([string]::IsNullOrEmpty($(Get-AzContext).Account)) 
{
    Connect-AzAccount 
    $VerbosePreference = "SilentlyContinue"
}

#If more than one under your account
Select-AzSubscription -SubscriptionId $subscriptionId

#Verify Current Subscription
Get-AzSubscription –SubscriptionId $subscriptionId

# Function for validating ARM template
function Format-ValidationOutput {
    param ($ValidationOutput, [int] $Depth = 0)
    Set-StrictMode -Off
    return @($ValidationOutput | Where-Object { $_ -ne $null } | ForEach-Object { @('  ' * $Depth + ': ' + $_.Message) + @(Format-ValidationOutput @($_.Details) ($Depth + 1)) })
}

#check to see if the resource group exists
# In Cloud Slice, the resource group name starts with rgServices.
# If a student is using their own subscription and do not have a resource group
# that starts with that name when the resource group is created it will be of the form
# rgServices-yourinitials
$checkforResourceGroup = Get-AzResourceGroup AzureRmResourceGroup -ErrorVariable notPresent -ErrorAction SilentlyContinue | Where ResourceGroupName -Like "$($resourceGroupName)*"

#Create or check for existing resource group
if(!$checkforResourceGroup)
{
    $resourceGroupName = $resourceGroupName + $yourInitials
    Write-Host "Resource group '$resourceGroupName' does not exist. Creating a new resource group.";
    Write-Host "Creating resource group '$resourceGroupName' in location '$location'";
    New-AzResourceGroup -Name $resourceGroupName -Location $location
}
else
{
    $resourceGroupName = $checkforResourceGroup.ResourceGroupName
    Write-Host "Using existing resource group" $resourceGroupName;
	
}

if ($ValidateOnly) 
{
    $ErrorMessages = Format-ValidationOutput (Test-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupName `
                                       -TemplateFile $templatePath `
                                       -TemplateParameterObject $paramObject)
    if ($ErrorMessages) {
        Write-Output '', 'Validation returned the following errors:', @($ErrorMessages), '', 'Template is invalid.'
    }
    else {
        Write-Output '', 'Template is valid.'
    }
}
else 
{

 
   # Deploy using the ARM template
    $myOutput =(New-AzResourceGroupDeployment -Name $deploymentName `
                                       -ResourceGroupName $resourceGroupName `
                                       -TemplateFile $templatePath `
                                       -TemplateParameterObject $paramObject `
                                       -Force -Verbose `
                                       -ErrorVariable ErrorMessages).Outputs

     #Write-Output $myOutput.Values.Value

    if ($ErrorMessages) {
        Write-Output '', 'Template deployment returned the following errors:', @(@($ErrorMessages) | ForEach-Object { $_.Exception.Message.TrimEnd("`r`n") })
   }

   # Call the PS script that scrapes the Azure resources information, puts secrets in key vault and writes to the app user secrets
   $settingsPath = Join-Path -Path $rootARMPath -ChildPath "WriteDataSettingsToFile.ps1"
   $command = "$($settingsPath) -YourInitials '$($yourInitials)' -ResourceGroupName '$($resourceGroupName)' -subscriptionId '$($subscriptionId)'"
   Write-Host "Executing Command: $($command)"
   Invoke-Expression $command
 }

