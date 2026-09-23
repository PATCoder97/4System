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

function ConvertFrom-SecureValue {
    param([Security.SecureString]$Value)

    $pointer = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($Value)
    try {
        return [Runtime.InteropServices.Marshal]::PtrToStringBSTR($pointer)
    }
    finally {
        [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($pointer)
    }
}

function New-PasswordHash {
    param([string]$Password)

    $iterations = 100000
    $salt = New-Object byte[] 16
    $random = [Security.Cryptography.RandomNumberGenerator]::Create()
    try {
        $random.GetBytes($salt)
    }
    finally {
        $random.Dispose()
    }

    $derive = New-Object Security.Cryptography.Rfc2898DeriveBytes(
        $Password,
        $salt,
        $iterations,
        [Security.Cryptography.HashAlgorithmName]::SHA256)
    try {
        $hash = $derive.GetBytes(32)
    }
    finally {
        $derive.Dispose()
    }

    return 'PBKDF2-SHA256${0}${1}${2}' -f `
        $iterations,
        [Convert]::ToBase64String($salt),
        [Convert]::ToBase64String($hash)
}

$resolvedConfig = (Resolve-Path -LiteralPath $ConnectionConfig).Path
[xml]$config = Get-Content -LiteralPath $resolvedConfig -Encoding UTF8
$connectionSetting = $config.connectionStrings.add | Where-Object { $_.name -eq 'MainDatabase' } | Select-Object -First 1
if ($null -eq $connectionSetting -or [string]::IsNullOrWhiteSpace($connectionSetting.connectionString)) {
    throw 'Không tìm thấy connection string MainDatabase.'
}

$securePassword = Read-Host 'Nhập mật khẩu' -AsSecureString
$secureConfirmation = Read-Host 'Nhập lại mật khẩu' -AsSecureString
$password = ConvertFrom-SecureValue $securePassword
$confirmation = ConvertFrom-SecureValue $secureConfirmation

try {
    if ($password.Length -lt 8) {
        throw 'Mật khẩu phải có ít nhất 8 ký tự.'
    }
    if ($password -cne $confirmation) {
        throw 'Hai lần nhập mật khẩu không trùng khớp.'
    }

    $encodedHash = New-PasswordHash $password
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

INSERT dbo.auth_UserAccount(UserId, AuthenticationType, PasswordHash)
VALUES (@UserId, 'LOCAL', @PasswordHash);
'@
            [void]$insert.Parameters.Add('@UserId', [Data.SqlDbType]::VarChar, 10)
            [void]$insert.Parameters.Add('@PasswordHash', [Data.SqlDbType]::VarChar, 500)
            $insert.Parameters['@UserId'].Value = $normalizedUserId
            $insert.Parameters['@PasswordHash'].Value = $encodedHash
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
            Write-Output "Đã tạo tài khoản '$normalizedUserId' trong nhóm '$groupCode'."
        }
        catch {
            $transaction.Rollback()
            throw
        }
    }
    finally {
        $connection.Dispose()
    }
}
finally {
    $password = $null
    $confirmation = $null
    $encodedHash = $null
}
