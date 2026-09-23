# Winform4System

Khung ứng dụng WinForms .NET Framework 4.8, tham khảo bố cục Main của `7system` nhưng tách giao diện khỏi database.

## Cấu trúc

- `Winform4System`: giao diện DevExpress và điểm khởi động.
- `Winform4System.Core`: model dùng chung và thông tin phiên người dùng.
- `Winform4System.Business`: service nghiệp vụ; hiện cung cấp menu mẫu cho Main.
- `Winform4System.DataAccess`: lớp truy cập cấu hình kết nối; repository/EF sẽ bổ sung sau.
- `Winform4System.Logging`: logger file độc lập database.
- `Database`: script nền cho nhân sự, tài khoản, nhóm bảo mật, vai trò và phân quyền.

Luồng tham chiếu chính: `UI -> Business -> DataAccess`, các model chung nằm trong `Core`.

## Chạy ứng dụng

1. Mở `Winform4System.sln` bằng Visual Studio 2022.
2. Chọn `Winform4System` làm Startup Project.
3. Build cấu hình `Debug | Any CPU`.
4. Chạy ứng dụng. Main hiện sử dụng tài khoản và menu demo, không truy cập SQL.

Log được tạo trong thư mục `Logs` cạnh file `.exe`.

## Database

`Winform4System/App.config` loads the `MainDatabase` connection from `connectionStrings.local.config`. The local file is ignored by Git; use `connectionStrings.example.config` as the template and never commit real SQL credentials.

Chạy các script trong `Database/Scripts` theo thứ tự trước khi sinh EDMX bằng EF6 Database First. Mô hình phân quyền là `User -> SecurityGroup -> Role -> Permission -> Function`.
