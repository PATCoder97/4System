param(
    [string]$BusinessAssemblyPath = (Join-Path $PSScriptRoot '..\Winform4System.Business\bin\Debug\Winform4System.Business.dll'),
    [switch]$TestCorporateNetwork
)
$ErrorActionPreference = 'Stop'
[void][Reflection.Assembly]::LoadFrom((Resolve-Path $BusinessAssemblyPath).Path)
$validatorType = [Winform4System.Business.Security.WindowsDomainCredentialValidator]
$validator = New-Object Winform4System.Business.Security.WindowsDomainCredentialValidator
if ($validator.Validate('', '') -ne 'Invalid') { throw 'Empty credentials must be rejected.' }
$flags = [Reflection.BindingFlags]'NonPublic,Static'
$classify = $validatorType.GetMethod('ClassifyLdapFailure', $flags)
foreach ($code in @(49, 8, 13, 50, 82)) {
    if ($classify.Invoke($null, @([int]$code)).ToString() -ne 'Invalid') {
        throw "LDAP error $code must not allow cached authentication."
    }
}
foreach ($code in @(51, 52, 81, 85, 91)) {
    if ($classify.Invoke($null, @([int]$code)).ToString() -ne 'Unavailable') {
        throw "LDAP connection failure $code must allow offline fallback."
    }
}
$normalize = $validatorType.GetMethod('NormalizeUserName', $flags)
foreach ($inputName in @(' VNW0014732 ', 'vnfpg\VNW0014732', 'VNW0014732@vn.fpg.com')) {
    if ($normalize.Invoke($null, @($inputName)) -ne 'VNW0014732') { throw 'Username normalization failed.' }
}
if ($TestCorporateNetwork) {
    # Một lần thử với tên ngẫu nhiên không thuộc tài khoản nhân viên thật.
    $probeUser = 'w4s_probe_' + [Guid]::NewGuid().ToString('N').Substring(0, 8)
    $result = $validator.Validate($probeUser, [Guid]::NewGuid().ToString('N'))
    if ($result -ne 'Invalid') { throw "Expected domain rejection for probe account; got $result." }
    Write-Output 'PASS: Domain controller rejected probe credentials from this machine.'
}
Write-Output 'PASS: Credential normalization and LDAP failure classification.'
