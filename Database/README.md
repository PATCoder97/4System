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

## Bootstrap tài khoản đăng nhập

Ứng dụng hiện xác thực tài khoản `LOCAL` bằng PBKDF2-SHA256. Không seed tài khoản hoặc mật khẩu mặc định vào source. Sau khi chạy hai script schema, tạo quản trị viên đầu tiên bằng PowerShell; công cụ sẽ hỏi mật khẩu hai lần và không ghi mật khẩu ra console:

```powershell
.\Database\Tools\New-LocalUser.ps1 -LoginName ADMIN -SystemAdministrator
```

Bỏ `-SystemAdministrator` để tạo tài khoản thuộc nhóm `STANDARD_USERS`. Mật khẩu phải có ít nhất 12 ký tự. Sau khi có module quản lý người dùng, việc tạo tài khoản và đổi mật khẩu nên được thực hiện trên giao diện đó thay vì dùng công cụ bootstrap.

## Local connection string

The application reads `Winform4System/connectionStrings.local.config`. This file is ignored by Git because it contains machine-specific credentials. Copy `connectionStrings.example.config` to that filename and fill in the real SQL Server values. Never commit the local file.

## EF6 với database hiện hữu

Runtime truy cập database bằng EF6 `DbContext` và LINQ trong `Winform4System.DataAccess`; không dùng SQL thuần trong code C#. Database là nguồn cấu trúc chính và EF không được tự tạo hay tự migrate database (`Database.SetInitializer(null)`).

Chỉ map các bảng/cột mà module hiện tại thực sự sử dụng. Khi schema thay đổi, cập nhật entity/mapping EF tương ứng hoặc refresh EDMX nếu module đã chuyển sang model designer. Không đặt business logic trong entity sinh tự động; mở rộng qua repository, service hoặc partial class.
