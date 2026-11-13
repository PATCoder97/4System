[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [ValidatePattern('^VNW[0-9]{7}$')]
    [string]$UserId,
    [switch]$SystemAdministrator,
    [ValidateSet('Test', 'Prod')]
    [string]$Environment = 'Test'
)
$ErrorActionPreference = 'Stop'
$projectRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$outputFolder = Join-Path $projectRoot 'Winform4System\bin\Debug'
$compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
$tool = Join-Path $outputFolder 'ProvisionDomainUser.exe'
& $compiler /nologo "/out:$tool" "/r:$outputFolder\Winform4System.DataAccess.dll" "/r:$outputFolder\EntityFramework.dll" /r:System.Data.dll (Join-Path $PSScriptRoot 'ProvisionDomainUser.cs')
if ($LASTEXITCODE -ne 0) { throw 'Biên dịch công cụ thất bại. Build Debug solution trước khi chạy.' }
Copy-Item -LiteralPath (Join-Path $outputFolder 'Winform4System.exe.config') -Destination ($tool + '.config') -Force
$groupCode = if ($SystemAdministrator) { 'SYSTEM_ADMINISTRATORS' } else { 'STANDARD_USERS' }
& $tool $UserId.ToUpperInvariant() $Environment $groupCode
if ($LASTEXITCODE -ne 0) { throw 'Tạo tài khoản thất bại.' }
