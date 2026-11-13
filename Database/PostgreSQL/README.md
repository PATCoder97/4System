# PostgreSQL 17 và Share Data API

Ứng dụng .NET Framework 4.8 giữ EF6 6.5.2, dùng EntityFramework6.Npgsql 6.4.3 và Npgsql 4.1.14. UI gọi service/repository; không truy cập DbContext trực tiếp. Database là nguồn schema, ứng dụng không tự tạo/migrate bảng khi khởi động.

## Khởi tạo database trống

Chạy `001_schema.sql` rồi `002_seed.sql` bằng chủ sở hữu database. Schema chỉ chạy một lần; seed có thể chạy lại mà không ghi đè các thiết lập đã sửa. Tên bảng/cột giữ nguyên chữ hoa/thường và phải đặt trong dấu nháy kép khi viết SQL quản trị. Không chạy script SQL Server trong `Database/Scripts`.

Database đã khởi tạo chạy thêm `003_spare_part_card_order.sql` để chỉ hiển thị card 機邊庫, mở module SparePart hiện có. Mã `ASSET.SPARE_PART` và các quyền được giữ nguyên; card placeholder `ASSET.MACHINE_WAREHOUSE` bị ẩn. Seed mới đã dùng cấu hình này.

Chạy `004_pending_file_deletion.sql` trên cả database mới và database đã khởi tạo để thêm hàng đợi xóa file của API 0.1.2. Cập nhật schema trước khi chạy phiên bản ứng dụng mới.

Chạy `005_spare_part_navigation.sql` để đăng ký 8 chức năng con và nhóm 機邊庫【經理室】 dưới `ASSET.SPARE_PART`. Menu module đọc tên, thứ tự, trạng thái hiển thị/kích hoạt từ các bản ghi này; màn hình chính chỉ hiển thị card cấp module. Mỗi chức năng con có quyền `.VIEW` riêng trong ma trận phân quyền. Migration kế thừa phạm vi từ quyền module hiện có, chỉ cấp nhóm 經理室 cho role có APPROVE/ADMIN. Các quyền thao tác nghiệp vụ CREATE/UPDATE/DELETE/APPROVE/EXPORT hiện có vẫn được kiểm tra. Sau khi sửa cấu hình, mở lại module; sau khi sửa quyền role, đăng nhập lại để cập nhật phiên.

Chạy `006_manual_function_configuration.sql` để thêm `IconKey` và đưa các mục quản trị hệ thống về các nhóm bên trong module 系統管理. Sau migration, cả 機邊庫 và 系統管理 đọc menu từ DB. Không chạy lại các seed SQL Server cũ.

## Thêm chức năng thủ công

Trong 功能卡管理, tài khoản có `SYSTEM.FUNCTION.ADMIN` dùng nút **新增功能**. Chọn nhóm cha, nhập mã kỹ thuật duy nhất và tên zh-Hant, chọn màn hình đích hoặc để trống để tạo nhóm. Mã được chuẩn hóa chữ hoa và không thay đổi sau khi tạo.

Có thể chọn icon có xem trước từ catalog SVG dùng chung, thứ tự, trạng thái phát triển, hiển thị và kích hoạt. Icon được lưu bằng khóa catalog; không lưu đường dẫn tùy ý hay nạp class từ tên do người dùng nhập. Mỗi màn hình mới tự tạo permission `<FunctionCode>.VIEW` và gán cho role `SECURITY_ADMIN`. Các role khác cần được cấp quyền bằng ma trận quyền; đăng nhập lại để cập nhật phiên.

Có thể thêm một card module mới trỏ tới `SparePartForm` hoặc `SystemSettingsForm`. Di chuyển nhóm sẽ giữ nguyên liên kết các con; nhóm đưa ra ngay dưới danh mục chính sẽ có card riêng nếu còn màn hình con được phép truy cập. Chọn lại một màn hình có sẵn chỉ tạo thêm lối truy cập; quyền xem của mục menu, quyền module gốc và quyền nghiệp vụ gốc của màn hình vẫn bắt buộc. Màn hình nghiệp vụ mới vẫn phải được lập trình và đăng ký trong catalog. Tên hiển thị, icon và thứ tự có hiệu lực khi mở lại module.

Backend kiểm tra mã trùng, mã bất biến, vòng lặp nhóm cha, vị trí nhóm/màn hình và RowVersion. Tạo/sửa và cấp permission nằm trong một transaction EF6, có audit. Không tự cấp quyền màn hình gốc cho người dùng chỉ vì họ được cấp một mục menu mới.

## Xóa chức năng

Nhấp chuột phải → **檢視功能卡** để mở chế độ xem có **編輯／刪除／關閉**, hoặc chọn **刪除功能** để vào thẳng chế độ xóa. Nút **確認刪除** yêu cầu xác nhận, mặc định chọn không. Hủy không ghi dữ liệu. Quyền `SYSTEM.FUNCTION.ADMIN` được kiểm tra ở UI và Business.

Chặn xóa nhóm còn bất kỳ con nào (kể cả con ẩn/tắt), `MAIN` và các chức năng cung cấp quyền cố định mà catalog sử dụng. Các mục cốt lõi chỉ nên ẩn/tắt. Những mục khác như placeholder `ASSET.MACHINE_WAREHOUSE` có thể xóa khi không có con. Không tự xóa cả nhánh.

Service đọc lại bản ghi và RowVersion, rồi xóa liên kết role–permission, permission và chức năng theo thứ tự khóa ngoại trong một transaction EF6 mức Serializable. Audit `SYSTEM.FUNCTION.DELETE` giữ cấu hình trước xóa, mã permission, liên kết role và danh sách người dùng bị ảnh hưởng. Lỗi ở bất kỳ bước nào rollback toàn bộ. Dữ liệu nghiệp vụ, role, nhóm và tài khoản không bị xóa.

Đổi SecurityStamp của các tài khoản đang nhận các quyền bị xóa để cơ chế kiểm tra phiên yêu cầu đăng nhập lại; không thay đổi tài khoản không liên quan. Các tab đã mở vẫn kiểm tra quyền nghiệp vụ khi thao tác. Kiểm chứng bằng `Database/Tools/VerifyFunctionDeletion.cs` trên Test với fixture riêng, bao gồm lỗi giả lập ở bước xóa cuối để kiểm tra rollback; không dùng chức năng/tài khoản thật làm dữ liệu xóa.

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

`Database/Tools/VerifyModuleNavigation.cs` kiểm tra đủ 8 chức năng con, không tạo card con trên Main, thứ tự menu, quyền xem và trạng thái ẩn/tắt của mục hoặc nhóm cha. Chỉ chạy trên Test, tự dọn dữ liệu thử.

`Database/Tools/VerifyManualFunctions.cs` kiểm tra thêm/sửa nhóm và mục, icon, tạo permission/audit, mã trùng, mã icon/màn hình không hợp lệ, vòng lặp, chuyển sai module, RowVersion, card module mới và quyền nghiệp vụ gốc. Chỉ chạy trên Test, tự dọn dữ liệu thử và quyền/audit do phép thử tạo.

`ExportPostgresSchema.cs` xuất metadata EDMX của hai context để đối chiếu mapping; không tự áp dụng DDL.
