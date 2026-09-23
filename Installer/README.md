# Tạo bộ cài Winform4System

Bộ cài dùng Advanced Installer 22.2, giao diện tiếng Trung phồn thể và đóng gói output `Release` của ứng dụng thành một file EXE.

Project installer chỉ lấy file từ `Installer/Staging`. Không thêm `ProjectReference` để tránh Advanced Installer import output lần thứ hai và làm bộ cài tăng gần gấp đôi dung lượng.

## Chạy nhanh

Nhấp đúp `Build-Installer.bat` ở thư mục gốc.

Hoặc chạy PowerShell:

```powershell
.\Installer\Build-Installer.ps1
```

Mở thư mục kết quả sau khi build:

```powershell
.\Installer\Build-Installer.ps1 -OpenOutput
```

## Phiên bản

Phiên bản có định dạng `yy.MM.dd.index`, ví dụ `26.09.23.1`.

- Ngày mới bắt đầu lại từ index `1`.
- Build tiếp theo trong cùng ngày tự tăng index.
- Có thể chỉ định index từ `1` đến `99`: `.\Installer\Build-Installer.ps1 -BuildIndex 5`.
- Trạng thái phiên bản được lưu trong `Installer/version.json`.
- Script đồng bộ phiên bản vào `AssemblyInfo.cs` và project Advanced Installer.

Windows Installer chỉ so sánh ba phần của `ProductVersion`, nên MSI nội bộ mã hóa ngày và index thành `yy.MM.(dd*100+index)`. Tên file và phiên bản ứng dụng vẫn giữ đúng dạng `yy.MM.dd.index`; cách mã hóa này bảo đảm các bản build cùng ngày nâng cấp đúng bản cũ.

## Điều kiện

- Visual Studio hoặc Build Tools có MSBuild.
- Advanced Installer; có thể đặt đường dẫn CLI trong biến `ADVANCED_INSTALLER_PATH`.
- `Winform4System/connectionStrings.local.config` phải tồn tại. Script sẽ dừng thay vì tạo bộ cài thiếu cấu hình kết nối.

File cài được tạo theo cùng quy ước của `7system` tại:

`E:\01. DEV\02.4System\04. Setup files\Winform4System-<version>.exe`
