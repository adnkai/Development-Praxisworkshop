#Based on SO 61021658
#https://stackoverflow.com/questions/61021658/using-microsoft-graph-sdk-for-powershell-to-assign-role-to-app-service-principal
$GraphConnection = Connect-MgGraph -Scopes "User.ReadWrite.All", "Directory.ReadWrite.All", "Group.Read.All", "Directory.AccessAsUser.All"
#https://docs.microsoft.com/de-de/graph/permissions-reference#all-permissions-and-ids
# Microsoft.Graph ResourceAccess scopes and roles #Scope => Delegated; 
$mgOfflineAccessScope = @{
    "Id" = "7427e0e9-2fba-42fe-b0c0-848c9e6a8182"
    "Type" = "Scope"
}
$mgOpenidScope = @{
    "Id" = "37f7f235-527c-4136-accd-4a02d197296e"
    "Type" = "Scope"
}
$mgFilesReadWriteAllScope = @{
    "Id" = "863451e7-0667-486c-a5d6-d135439485f0"
    "Type" = "Scope"
}
$mgResourceAccess = $mgOfflineAccessScope, $mgOpenidScope, $mgFilesReadWriteAllScope
#00000003-0000-0000-c000-000000000000 => Graph
$requiredResourceAccess = @{
    "ResourceAppId" = "00000003-0000-0000-c000-000000000000"
    "ResourceAccess" = $mgResourceAccess
}
$web = @{
    RedirectUriSettings = @(
        @{
            Uri = "https://localhost:8363/Login" # Change to you applications endpoint(s)
        }
    )
    LogoutUrl = "https://localhost:5050/Logout" # Change to you applications endpoint
}
$mgApplicationParams = @{
    DisplayName = "AppRegAuthWRKSHP" # Resource name
    RequiredResourceAccess = $requiredResourceAccess
    Web = $web
}
# We need to create our application before we can add permissions to our custom scope
$mgApplication = New-MgApplication @mgApplicationParams
# Azure doesn't always update immediately, make sure app exists before we try to update its config
$appExists = $false
while (!$appExists) {
    Start-Sleep -Seconds 2
    $appExists = Get-MgApplication -ApplicationId $mgApplication.Id
}
$PWInfos = @{
    PasswordCredential = @{
        DisplayName = "PWD"
    }
}
# https://docs.microsoft.com/en-us/graph/api/application-addpassword?view=graph-rest-1.0&tabs=http
Add-MgApplicationPassword -ApplicationId $mgApplication.Id -BodyParameter $PWInfos
$newApplicationRole = @{
    "appRoles" = @(
        @{
            allowedMemberTypes = @("User", "Application")
            description = "Rollenbeschreibung"
            displayName = "EditorRolle"
            value = "Access.ReadWrite"
            id = New-Guid
        },
        @{
            allowedMemberTypes = @("User", "Application")
            description = "Rollenbeschreibung"
            displayName = "UserRolle"
            value = "Access.Read"
            id = New-Guid
        }
    )
}
# https://docs.microsoft.com/en-us/graph/api/application-update?view=graph-rest-1.0&tabs=http
Update-MgApplication -ApplicationId $mgApplication.Id -BodyParameter $newApplicationRole
# Force creation of Enterprise Application
$appServicePrincipal = New-MgServicePrincipal -AppId $mgApplication.AppId -Tags @("WindowsAzureActiveDirectoryIntegratedApp")
#$result = New-MgServicePrincipalAppRoleAssignment -ServicePrincipalId $appServicePrincipal.Id `
#-AppRoleId <RoleID> `
#-ResourceId <USER/GROUP> `
#-PrincipalType "ServicePrincipal"
Disconnect-MgGraph