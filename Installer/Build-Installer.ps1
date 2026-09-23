[CmdletBinding()]
param(
    [ValidateRange(0, 99)]
    [int]$BuildIndex = 0,

    [switch]$OpenOutput
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$installerRoot = $PSScriptRoot
$repositoryRoot = Split-Path -Parent $installerRoot
$solutionPath = Join-Path $repositoryRoot 'Winform4System.sln'
$applicationProjectRoot = Join-Path $repositoryRoot 'Winform4System'
$releaseOutput = Join-Path $applicationProjectRoot 'bin\Release'
$stagingDirectory = Join-Path $installerRoot 'Staging'
$installerProject = Join-Path $installerRoot 'Winform4System.Installer.aip'
$versionStatePath = Join-Path $installerRoot 'version.json'
$systemRoot = Split-Path -Parent (Split-Path -Parent $repositoryRoot)
$setupOutputRoot = Join-Path $systemRoot '04. Setup files'
$assemblyInfoPath = Join-Path $applicationProjectRoot 'Properties\AssemblyInfo.cs'

function Invoke-NativeTool
{
    param(
        [Parameter(Mandatory = $true)]
        [string]$FilePath,

        [Parameter(Mandatory = $true)]
        [string[]]$ToolArguments
    )

    Write-Host "`n> $FilePath $($ToolArguments -join ' ')" -ForegroundColor DarkGray
    & $FilePath @ToolArguments
    if ($LASTEXITCODE -ne 0)
    {
        throw "Lệnh thất bại với mã $LASTEXITCODE`: $FilePath"
    }
}

function Find-MSBuild
{
    $vsWhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
    if (Test-Path -LiteralPath $vsWhere)
    {
        $installationPath = & $vsWhere -latest -products * -requires Microsoft.Component.MSBuild -property installationPath
        if ($installationPath)
        {
            $candidate = Join-Path $installationPath 'MSBuild\Current\Bin\MSBuild.exe'
            if (Test-Path -LiteralPath $candidate)
            {
                return $candidate
            }
        }
    }

    $fallbacks = @(
        'C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe',
        'C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe',
        'C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe',
        'C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\MSBuild.exe'
    )

    $match = $fallbacks | Where-Object { Test-Path -LiteralPath $_ } | Select-Object -First 1
    if (-not $match)
    {
        throw 'Không tìm thấy MSBuild. Hãy cài Visual Studio hoặc Visual Studio Build Tools.'
    }

    return $match
}

function Find-AdvancedInstaller
{
    if ($env:ADVANCED_INSTALLER_PATH -and (Test-Path -LiteralPath $env:ADVANCED_INSTALLER_PATH))
    {
        return $env:ADVANCED_INSTALLER_PATH
    }

    $searchRoots = @(
        (Join-Path ${env:ProgramFiles(x86)} 'Caphyon'),
        (Join-Path $env:ProgramFiles 'Caphyon')
    ) | Where-Object { $_ -and (Test-Path -LiteralPath $_) }

    $match = $searchRoots |
        ForEach-Object { Get-ChildItem -LiteralPath $_ -Filter 'AdvancedInstaller.com' -File -Recurse -ErrorAction SilentlyContinue } |
        Sort-Object FullName -Descending |
        Select-Object -First 1

    if (-not $match)
    {
        throw 'Không tìm thấy Advanced Installer CLI. Hãy cài Advanced Installer hoặc đặt ADVANCED_INSTALLER_PATH.'
    }

    return $match.FullName
}

function Get-BuildVersion
{
    $datePart = Get-Date -Format 'yy.MM.dd'
    $nextIndex = $BuildIndex

    if ($nextIndex -eq 0)
    {
        $nextIndex = 1
        if (Test-Path -LiteralPath $versionStatePath)
        {
            $state = Get-Content -LiteralPath $versionStatePath -Raw | ConvertFrom-Json
            if (($state.date -eq $datePart) -and ([int]$state.index -ge 1))
            {
                $nextIndex = [int]$state.index + 1
            }
        }
    }

    return [pscustomobject]@{
        Date = $datePart
        Index = $nextIndex
        Version = "$datePart.$nextIndex"
        MsiVersion = ('{0}.{1}.{2}' -f
            [int]$datePart.Substring(0, 2),
            [int]$datePart.Substring(3, 2),
            (([int]$datePart.Substring(6, 2) * 100) + $nextIndex))
    }
}

function Set-ApplicationVersion
{
    param([Parameter(Mandatory = $true)][string]$Version)

    $utf8WithBom = New-Object System.Text.UTF8Encoding($true)
    $content = [System.IO.File]::ReadAllText($assemblyInfoPath, [System.Text.Encoding]::UTF8)
    $content = [System.Text.RegularExpressions.Regex]::Replace(
        $content,
        '(?m)^\[assembly: AssemblyVersion\("[^"]+"\)\]',
        "[assembly: AssemblyVersion(`"$Version`")]"
    )
    $content = [System.Text.RegularExpressions.Regex]::Replace(
        $content,
        '(?m)^\[assembly: AssemblyFileVersion\("[^"]+"\)\]',
        "[assembly: AssemblyFileVersion(`"$Version`")]"
    )
    [System.IO.File]::WriteAllText($assemblyInfoPath, $content, $utf8WithBom)
}

function Reset-StagingDirectory
{
    $installerFullPath = [System.IO.Path]::GetFullPath($installerRoot).TrimEnd('\') + '\'
    $stagingFullPath = [System.IO.Path]::GetFullPath($stagingDirectory)
    if (-not $stagingFullPath.StartsWith($installerFullPath, [System.StringComparison]::OrdinalIgnoreCase))
    {
        throw "Thư mục staging không an toàn: $stagingFullPath"
    }

    if (Test-Path -LiteralPath $stagingFullPath)
    {
        Remove-Item -LiteralPath $stagingFullPath -Recurse -Force
    }
    New-Item -ItemType Directory -Path $stagingFullPath -Force | Out-Null
}

if (-not (Test-Path -LiteralPath $installerProject))
{
    throw "Không tìm thấy project Advanced Installer: $installerProject"
}

$localConnectionConfig = Join-Path $applicationProjectRoot 'connectionStrings.local.config'
if (-not (Test-Path -LiteralPath $localConnectionConfig))
{
    throw 'Thiếu Winform4System\connectionStrings.local.config. Hãy tạo file này từ connectionStrings.example.config trước khi đóng gói.'
}

$buildVersion = Get-BuildVersion
$version = $buildVersion.Version
$outputDirectory = $setupOutputRoot
$packageName = "Winform4System-$version.exe"
$packagePath = Join-Path $outputDirectory $packageName
$msBuild = Find-MSBuild
$advancedInstaller = Find-AdvancedInstaller

Write-Host "Đang tạo bộ cài 軋鋼部系統 phiên bản $version" -ForegroundColor Cyan
Set-ApplicationVersion -Version $version

Invoke-NativeTool -FilePath $msBuild -ToolArguments @(
    $solutionPath,
    '/t:Rebuild',
    '/p:Configuration=Release',
    '/p:Platform=Any CPU',
    '/p:DebugSymbols=false',
    '/p:DebugType=None',
    '/nologo',
    '/verbosity:minimal'
)

$applicationExe = Join-Path $releaseOutput 'Winform4System.exe'
if (-not (Test-Path -LiteralPath $applicationExe))
{
    throw "Không tìm thấy output Release: $applicationExe"
}

Reset-StagingDirectory
Copy-Item -Path (Join-Path $releaseOutput '*') -Destination $stagingDirectory -Recurse -Force

# Không đưa tài liệu IntelliSense và resource ngôn ngữ không sử dụng vào MSI.
Get-ChildItem -LiteralPath $stagingDirectory -Filter '*.xml' -File | Remove-Item -Force
foreach ($cultureName in @('de', 'es', 'ja'))
{
    $cultureDirectory = Join-Path $stagingDirectory $cultureName
    if (Test-Path -LiteralPath $cultureDirectory)
    {
        Remove-Item -LiteralPath $cultureDirectory -Recurse -Force
    }
}

$stagedConnectionConfig = Join-Path $stagingDirectory 'connectionStrings.local.config'
if (-not (Test-Path -LiteralPath $stagedConnectionConfig))
{
    throw 'Output Release không có connectionStrings.local.config; dừng để tránh tạo bộ cài không thể kết nối dữ liệu.'
}

New-Item -ItemType Directory -Path $outputDirectory -Force | Out-Null

Invoke-NativeTool -FilePath $advancedInstaller -ToolArguments @('/edit', $installerProject, '/SetVersion', $buildVersion.MsiVersion)
Invoke-NativeTool -FilePath $advancedInstaller -ToolArguments @('/edit', $installerProject, '/SetProperty', "FULLPRODUCTVERSION=$version")
Invoke-NativeTool -FilePath $advancedInstaller -ToolArguments @('/edit', $installerProject, '/SetOutputType', 'ExeInside', '-buildname', 'DefaultBuild')
Invoke-NativeTool -FilePath $advancedInstaller -ToolArguments @('/edit', $installerProject, '/SetPackageName', $packageName, '-buildname', 'DefaultBuild')
Invoke-NativeTool -FilePath $advancedInstaller -ToolArguments @('/edit', $installerProject, '/SetOutputLocation', '-buildname', 'DefaultBuild', '-path', $outputDirectory)
Invoke-NativeTool -FilePath $advancedInstaller -ToolArguments @('/edit', $installerProject, '/RefreshSync', 'APPDIR')
Invoke-NativeTool -FilePath $advancedInstaller -ToolArguments @('/rebuild', $installerProject, '-buildslist', 'DefaultBuild')

if (-not (Test-Path -LiteralPath $packagePath))
{
    $generatedPackage = Get-ChildItem -LiteralPath $outputDirectory -Filter "Winform4System-$version.*" -File | Select-Object -First 1
    if (-not $generatedPackage)
    {
        throw "Advanced Installer hoàn tất nhưng không tìm thấy file cài trong $outputDirectory"
    }
    $packagePath = $generatedPackage.FullName
}

$state = [ordered]@{
    date = $buildVersion.Date
    index = $buildVersion.Index
    version = $version
}
$state | ConvertTo-Json | Set-Content -LiteralPath $versionStatePath -Encoding UTF8

Write-Host "`nĐã tạo bộ cài:" -ForegroundColor Green
Write-Host $packagePath -ForegroundColor Green

if ($OpenOutput)
{
    Start-Process explorer.exe -ArgumentList @($outputDirectory)
}
