# Database foundation

Run scripts in numeric order against a new SQL Server database:

1. `001_core_identity_and_authorization.sql`
2. `002_seed_core_authorization.sql`
3. `003_main_menu_cards.sql`
4. `004_standardize_user_id.sql`
5. `005_standardize_employee_display_names.sql`
6. `006_spare_part.sql`
7. `007_function_card_management.sql`
8. `008_employee_management.sql`
9. `009_security_group_management.sql`
10. `010_role_permission_management.sql`
11. `011_audit_log_management.sql`
12. `012_domain_authentication.sql`
13. `013_simplify_domain_account.sql`
14. `014_authentication_policy_settings.sql`
15. `015_department_management.sql`
16. `016_job_title_management.sql`
17. `017_restore_user_account_created_at_default.sql`

Script `006` creates the spare-parts schema, registers the function and its permissions, and creates the `SPARE_PART_VIEWER`, `SPARE_PART_OPERATOR`, and `SPARE_PART_MANAGER` roles. Assign those roles through security groups; the script intentionally does not grant normal users access automatically.

Set `SparePartDataPath` in `Winform4System/App.config` to the shared 7system data root if the existing spare-parts photos, recovery evidence, and report templates must remain available. When left blank, the application uses the current user's Documents folder.

The SQL files are UTF-8. If a PowerShell runner is used, read them explicitly with `Get-Content -Encoding UTF8` so Traditional Chinese seed values are not corrupted.

The scripts create the foundation only; no real employee or administrator account is seeded.

## Định danh người dùng

`UserId` là mã nhân viên duy nhất và cũng là tài khoản đăng nhập. Mã này có kiểu `varchar(10)` và định dạng `VNW` + 7 chữ số, ví dụ `VNW0014732`. Các bảng phân quyền, audit và bảng nghiệp vụ tham chiếu trực tiếp khóa này; không duy trì thêm `LoginName` hoặc khóa người dùng dạng số song song.

Tên nhân viên được lưu riêng bằng `DisplayNameTW` (tên Trung phồn thể) và `DisplayNameVN` (tên Việt). Giao diện zh-Hant ưu tiên `DisplayNameTW`, sau đó mới dùng `DisplayNameVN` và cuối cùng là `UserId` khi chưa có hồ sơ nhân viên.

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

Ứng dụng chỉ xác thực `UserId` thuộc domain `vn.fpg.com`, giống `7system`, và không lưu mật khẩu dạng rõ trong database. Trên máy đã join đúng domain này, mật khẩu được xác thực bằng domain controller; sau mỗi lần thành công, ứng dụng cập nhật PBKDF2 hash của mật khẩu vừa dùng. Khi máy không join `vn.fpg.com` hoặc domain controller tạm thời không khả dụng, ứng dụng so sánh mật khẩu với hash của lần xác thực domain thành công gần nhất.

```powershell
.\Database\Tools\New-DomainUser.ps1 -UserId VNW0014732 -SystemAdministrator
```

Bỏ `-SystemAdministrator` để tạo tài khoản thuộc nhóm `STANDARD_USERS`. Tài khoản mới phải đăng nhập thành công ít nhất một lần trên máy thuộc `vn.fpg.com` trước khi có thể dùng mật khẩu dự phòng trên máy ngoài domain.

## Local connection string

The application reads `Winform4System/connectionStrings.local.config`. This file is ignored by Git because it contains machine-specific credentials. Copy `connectionStrings.example.config` to that filename and fill in the real SQL Server values. Never commit the local file.

## EF6 với database hiện hữu

Runtime truy cập database bằng EF6 `DbContext` và LINQ trong `Winform4System.DataAccess`; không dùng SQL thuần trong code C#. Database là nguồn cấu trúc chính và EF không được tự tạo hay tự migrate database (`Database.SetInitializer(null)`).

Chỉ map các bảng/cột mà module hiện tại thực sự sử dụng. Khi schema thay đổi, cập nhật entity/mapping EF tương ứng hoặc refresh EDMX nếu module đã chuyển sang model designer. Không đặt business logic trong entity sinh tự động; mở rộng qua repository, service hoặc partial class.
