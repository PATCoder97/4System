# PostgreSQL 17 và Share Data API

Ứng dụng .NET Framework 4.8 giữ EF6 6.5.2, dùng EntityFramework6.Npgsql 6.4.3 và Npgsql 4.1.14. UI gọi service/repository; không truy cập DbContext trực tiếp. Database là nguồn schema, ứng dụng không tự tạo/migrate bảng khi khởi động.

## Khởi tạo database trống

Chạy `001_schema.sql` rồi `002_seed.sql` bằng chủ sở hữu database. Schema chỉ chạy một lần; seed có thể chạy lại mà không ghi đè các thiết lập đã sửa. Tên bảng/cột giữ nguyên chữ hoa/thường và phải đặt trong dấu nháy kép khi viết SQL quản trị. Không chạy script SQL Server trong `Database/Scripts`.

Database đã khởi tạo chạy thêm `003_spare_part_card_order.sql` để chỉ hiển thị card 機邊庫, mở module SparePart hiện có. Mã `ASSET.SPARE_PART` và các quyền được giữ nguyên; card placeholder `ASSET.MACHINE_WAREHOUSE` bị ẩn. Seed mới đã dùng cấu hình này.

Chạy `004_pending_file_deletion.sql` trên cả database mới và database đã khởi tạo để thêm hàng đợi xóa file của API 0.1.2. Cập nhật schema trước khi chạy phiên bản ứng dụng mới.

- Prod: `10.198.182.123:5432/winform4system_prod`, user `w4s_prod`.
- Test: `10.198.182.123:5432/winform4system_test`, user `w4s_test`.
- `RowVersion` là `bytea` 8 byte được cập nhật bằng trigger và sequence; EF6 vẫn phát hiện cập nhật xung đột.
- Hai view `vw_sparepart_User` và `vw_sparepart_Department` đọc dữ liệu từ hồ sơ/người dùng/bộ phận chung.
- Không seed tài khoản nhân viên hay mật khẩu domain.

## Chọn môi trường và build

1. Copy `Winform4System/connectionStrings.example.config` thành `connectionStrings.local.config`, điền hai mật khẩu.
2. Copy `Winform4System/appSettings.example.config` thành `appSettings.local.config`, điền API key.
3. Đặt `AppEnvironment` là `Test` hoặc `Prod`. Giá trị này chọn đồng thời connection string và thư mục file. Mặc định dùng Test.
4. Restore NuGet, build solution trong Visual Studio 2022. Hai file cấu hình local được copy cạnh EXE và bị loại khỏi Git.

Server giữ mật khẩu DB tại `/home/fhs/.config/winform4system/database-credentials.json` (quyền 600). Không đưa file cấu hình local vào commit hoặc log.

## Tài khoản domain ban đầu

Sau khi build Debug, chạy từ thư mục repo (thay mã ví dụ bằng mã nhân viên thật):

```powershell
.\Database\Tools\New-DomainUser.ps1 -UserId VNW0014732 -SystemAdministrator -Environment Test
```

Dùng `-Environment Prod` để cấp riêng trên Prod. Bỏ `-SystemAdministrator` để tạo người dùng thông thường. Công cụ từ chối tài khoản đã tồn tại; không tự nâng quyền tài khoản cũ. Đăng nhập phần mềm bằng mật khẩu domain hiện có; công cụ không đặt mật khẩu domain và không tạo tài khoản trong Active Directory.

Máy WORKGROUP không cần join domain để đăng nhập: ứng dụng xác thực trực tiếp với `vn.fpg.com` bằng LDAP Negotiate có signing/sealing khi có mạng công ty/VPN. Mật khẩu cache chỉ dùng khi domain không khả dụng, sau ít nhất một lần xác thực domain thành công. Kiểm tra kết nối và phân loại lỗi bằng `Tests/Test-DomainCredentialValidator.ps1 -TestCorporateNetwork`; phép thử mạng dùng một tên tài khoản ngẫu nhiên để kiểm tra domain từ chối thông tin không hợp lệ.

## Kho file

- API: `http://10.198.182.123:8000/`, header `X-API-Key`.
- Prod: `winform4system/prod/`; Test: `winform4system/test/`.
- Upload binary bằng PUT, `overwrite=false`; tên file lưu trữ được sinh riêng. Đường dẫn qua API được mã hóa theo từng đoạn.
- File tải về nằm trong `%LOCALAPPDATA%/Winform4System/FileCache/<prod|test>/`; đây chỉ là cache, không phải kho dùng chung. Mỗi lần mở file lưu trữ sẽ tải lại từ API.
- Template nhãn cần được tải lên `winform4system/<môi trường>/00/ReportFormat/` với tên template mà màn hình sử dụng. Database/kho file mới chưa có template hay dữ liệu nghiệp vụ cũ.
- API 0.1.2 hỗ trợ DELETE có `X-API-Key`. HTTP 404 khi xóa được coi là đã hoàn tất; các lỗi khác giữ yêu cầu để thử lại.
- Xóa ảnh kiểm kê, xóa hướng dẫn hoặc xóa rõ ràng ảnh/bằng chứng sẽ ghi yêu cầu vào `sys_PendingFileDeletion` cùng transaction EF cập nhật bản ghi. Chỉ sau khi commit, tiến trình nền mới xóa trên server và cache. Tiến trình chạy mỗi phút khi ứng dụng hoạt động; yêu cầu lỗi được thử lại sau 5 phút, có lưu số lần thử và lỗi gần nhất. Màn hình trạng thái hệ thống hiển thị số yêu cầu chưa xử lý.
- Thay ảnh vật liệu chỉ ngừng sử dụng ảnh trước đó, vẫn giữ file để xem lịch sử. Không tự động quét/xóa file cũ đã mồ côi từ các phiên bản trước.
- Hai thư mục môi trường cùng dùng một API key, không phải ranh giới phân quyền trên server.

## Kiểm tra

`Database/Tools/VerifyPostgres.cs` là công cụ kiểm tra tích hợp dùng cấu hình Test: đọc seed, thêm/sửa/xóa bộ phận tạm, kiểm tra xung đột RowVersion, default audit và rollback, đọc view Spare, upload/download/delete binary và DELETE lặp lại. Tự xóa file thử sau khi kiểm tra. Chỉ chạy trên Test.

`Database/Tools/VerifyFileDeletion.cs` kiểm tra rollback yêu cầu xóa, xóa hướng dẫn qua service, xử lý hàng đợi, chặn đường dẫn ngoài kho và lên lịch thử lại khi lỗi. Chỉ chạy trên Test, tự dọn bản ghi/file thử.

`ExportPostgresSchema.cs` xuất metadata EDMX của hai context để đối chiếu mapping; không tự áp dụng DDL.
