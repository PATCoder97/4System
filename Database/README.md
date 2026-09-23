# Database foundation

Run scripts in numeric order against a new SQL Server database:

1. `001_core_identity_and_authorization.sql`
2. `002_seed_core_authorization.sql`

The SQL files are UTF-8. If a PowerShell runner is used, read them explicitly with `Get-Content -Encoding UTF8` so Traditional Chinese seed values are not corrupted.

The scripts create the foundation only; no real employee or administrator account is seeded.

## Authorization model

```text
User account
  -> security group membership
  -> roles assigned to the group
  -> permissions assigned to each role
  -> application function and action
```

Do not assign individual permissions directly to a user. When an exception is needed, create a clearly named security group and add that user to it. Effective permissions are the union of permissions inherited from all active groups.

The initial implementation deliberately does not support `DENY` rules or nested groups. Both features make permission troubleshooting substantially harder. A missing permission means denied.

Use `dbo.vw_auth_UserEffectivePermission` to inspect where a user receives a permission. Use `dbo.usp_auth_UserHasPermission` for a single permission check. Membership changes go through `dbo.usp_auth_AddUserToGroup` and `dbo.usp_auth_RemoveUserFromGroup`; both operations write an audit record.

`hr_EmployeeProfile` contains operational employee information. Personal identifiers, home address and other HR-sensitive fields are isolated in the one-to-one table `hr_EmployeePrivate`, so ordinary user-management screens do not need access to them.

## Bootstrap example

After creating the first department and employee, create the first Windows-authenticated user and place it in the administrator group:

```sql
INSERT dbo.dm_Department(DepartmentCode, DepartmentName)
VALUES ('IT', N'Phòng hệ thống');

INSERT dbo.hr_EmployeeProfile(EmployeeCode, FullName, DepartmentId)
SELECT 'EMP001', N'Quản trị viên đầu tiên', DepartmentId
FROM dbo.dm_Department
WHERE DepartmentCode = 'IT';

INSERT dbo.auth_UserAccount(EmployeeProfileId, LoginName, DomainAccount, AuthenticationType)
SELECT EmployeeProfileId, 'EMP001', 'DOMAIN\\EMP001', 'WINDOWS'
FROM dbo.hr_EmployeeProfile
WHERE EmployeeCode = 'EMP001';

DECLARE @UserId bigint =
(
    SELECT UserId FROM dbo.auth_UserAccount WHERE LoginName = 'EMP001'
);

EXEC dbo.usp_auth_AddUserToGroup
    @UserId = @UserId,
    @GroupCode = 'SYSTEM_ADMINISTRATORS';
```

Replace the sample domain and identity values before execution.

## Local connection string

The application reads `Winform4System/connectionStrings.local.config`. This file is ignored by Git because it contains machine-specific credentials. Copy `connectionStrings.example.config` to that filename and fill in the real SQL Server values. Never commit the local file.

## EF6 Database First

After these scripts have been applied to the real database, generate the EDMX inside `Winform4System.DataAccess`. Import only the tables, view and stored procedures needed by the current module. Generated entity files must not contain handwritten business logic; extend them with partial classes or services.
