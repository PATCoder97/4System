param(
    [string]$CoreAssemblyPath = (Join-Path $PSScriptRoot '..\Winform4System.Core\bin\Debug\Winform4System.Core.dll')
)

$ErrorActionPreference = 'Stop'
$resolvedAssemblyPath = (Resolve-Path -LiteralPath $CoreAssemblyPath).Path
[void][Reflection.Assembly]::LoadFrom($resolvedAssemblyPath)

function Assert-Permission {
    param(
        [string]$PermissionCode,
        [bool]$Expected,
        [string]$Scenario
    )

    $actual = [Winform4System.Core.Security.CurrentAuthorization]::HasPermission($PermissionCode)
    if ($actual -ne $Expected) {
        throw "FAILED: $Scenario. Permission=$PermissionCode, expected=$Expected, actual=$actual"
    }
}

try {
    [Winform4System.Core.Security.CurrentAuthorization]::SetIdentityAndPermissions(
        'VNW0000001',
        [string[]]@('ASSET.SPARE_PART.ADMIN'))
    Assert-Permission 'ASSET.SPARE_PART.VIEW' $true 'Spare-part admin can use permissions in its scope'
    Assert-Permission 'ASSET.SPARE_PART.UPDATE' $true 'Spare-part admin can use permissions in its scope'
    Assert-Permission 'SYSTEM.USER.ADMIN' $false 'Spare-part admin is not a system admin'
    Assert-Permission 'SYSTEM.ROLE.VIEW' $false 'Spare-part admin cannot view system roles'

    [Winform4System.Core.Security.CurrentAuthorization]::SetIdentityAndPermissions(
        'VNW0000001',
        [string[]]@('SYSTEM.USER.ADMIN'))
    Assert-Permission 'SYSTEM.USER.ADMIN' $true 'Exact system permission is accepted'
    Assert-Permission 'SYSTEM.ROLE.ADMIN' $false 'System permission does not extend to another function'
    Assert-Permission 'ASSET.SPARE_PART.VIEW' $false 'System permission does not extend to spare parts'

    [Winform4System.Core.Security.CurrentAuthorization]::SetIdentityAndPermissions(
        'VNW0000001',
        [string[]]@())
    Assert-Permission 'SYSTEM.USER.VIEW' $false 'Account without system permission is denied'
    Assert-Permission 'ASSET.SPARE_PART.VIEW' $false 'Account without spare-part permission is denied'

    Write-Output 'PASS: Authorization scope tests succeeded.'
}
finally {
    [Winform4System.Core.Security.CurrentAuthorization]::SetIdentityAndPermissions($null, [string[]]@())
}
