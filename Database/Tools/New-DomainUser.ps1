[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [ValidatePattern('^VNW[0-9]{7}$')]
    [string]$UserId,

    [switch]$SystemAdministrator,

    [string]$ConnectionConfig
)

$ErrorActionPreference = 'Stop'

if ([string]::IsNullOrWhiteSpace($ConnectionConfig)) {
    $ConnectionConfig = Join-Path $PSScriptRoot '..\..\Winform4System\connectionStrings.local.config'
}

$resolvedConfig = (Resolve-Path -LiteralPath $ConnectionConfig).Path
[xml]$config = Get-Content -LiteralPath $resolvedConfig -Encoding UTF8
$connectionSetting = $config.connectionStrings.add | Where-Object { $_.name -eq 'MainDatabase' } | Select-Object -First 1
if ($null -eq $connectionSetting -or [string]::IsNullOrWhiteSpace($connectionSetting.connectionString)) {
    throw 'Không tìm thấy connection string MainDatabase.'
}

$normalizedUserId = $UserId.Trim().ToUpperInvariant()
$groupCode = if ($SystemAdministrator) { 'SYSTEM_ADMINISTRATORS' } else { 'STANDARD_USERS' }

$connection = New-Object Data.SqlClient.SqlConnection($connectionSetting.connectionString)
$connection.Open()
try {
    $transaction = $connection.BeginTransaction()
    try {
        $insert = $connection.CreateCommand()
        $insert.Transaction = $transaction
        $insert.CommandText = @'
IF EXISTS (SELECT 1 FROM dbo.auth_UserAccount WHERE UserId = @UserId)
    THROW 51010, N'Tài khoản đã tồn tại.', 1;

INSERT dbo.auth_UserAccount(UserId)
VALUES (@UserId);
'@
        [void]$insert.Parameters.Add('@UserId', [Data.SqlDbType]::VarChar, 10)
        $insert.Parameters['@UserId'].Value = $normalizedUserId
        [void]$insert.ExecuteNonQuery()

        $assign = $connection.CreateCommand()
        $assign.Transaction = $transaction
        $assign.CommandText = 'EXEC dbo.usp_auth_AddUserToGroup @UserId, @GroupCode, @AssignedByUserId;'
        [void]$assign.Parameters.Add('@UserId', [Data.SqlDbType]::VarChar, 10)
        [void]$assign.Parameters.Add('@GroupCode', [Data.SqlDbType]::VarChar, 80)
        [void]$assign.Parameters.Add('@AssignedByUserId', [Data.SqlDbType]::VarChar, 10)
        $assign.Parameters['@UserId'].Value = $normalizedUserId
        $assign.Parameters['@GroupCode'].Value = $groupCode
        $assign.Parameters['@AssignedByUserId'].Value = $normalizedUserId
        [void]$assign.ExecuteNonQuery()

        $transaction.Commit()
        Write-Output "Đã tạo tài khoản domain '$normalizedUserId' trong nhóm '$groupCode'."
    }
    catch {
        $transaction.Rollback()
        throw
    }
}
finally {
    $connection.Dispose()
}
