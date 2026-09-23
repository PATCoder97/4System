# Quy tắc phát triển Winform4System

## Ngôn ngữ giao diện

- Tất cả nội dung người dùng cuối nhìn thấy phải dùng tiếng Trung phồn thể (`zh-Hant`).
- Quy tắc này áp dụng cho tên chức năng, menu, tiêu đề form, nút bấm, nhãn, tooltip, thông báo, cảnh báo, nội dung xác nhận, báo cáo và dữ liệu seed dùng để hiển thị.
- Không sử dụng tiếng Trung giản thể trong giao diện.
- Font mặc định của toàn bộ giao diện là `Microsoft JhengHei UI`; chỉ dùng font khác khi người dùng yêu cầu rõ ràng cho một thành phần cụ thể.
- Khi sửa một màn hình cũ, phải chuyển các chuỗi tiếng Việt hoặc tiếng Anh mà người dùng nhìn thấy trên màn hình đó sang tiếng Trung phồn thể.
- Tên class, method, biến, bảng database, cột database, mã chức năng và mã permission tiếp tục dùng tiếng Anh để bảo đảm ổn định kỹ thuật.
- Tài liệu kỹ thuật, comment và commit message có thể dùng tiếng Việt có dấu, trừ khi người dùng yêu cầu khác.

## Control giao diện

- Ưu tiên sử dụng control của DevExpress cho form và các thành phần giao diện, ví dụ `XtraForm`, `SimpleButton`, `TextEdit`, `ButtonEdit`, `LabelControl`, `PanelControl`, `GridControl` và `LayoutControl`.
- Chỉ sử dụng control WinForms thuần khi DevExpress không có control tương đương, control DevExpress không đáp ứng được hành vi cần thiết, hoặc có lý do kỹ thuật rõ ràng.
- Khi chuyển giao diện từ `7system`, ưu tiên giữ cùng loại control DevExpress để giao diện, theme và hành vi được đồng nhất.

## Truy cập dữ liệu

- Tất cả truy cập SQL Server trong code ứng dụng phải sử dụng Entity Framework 6 và LINQ thông qua `DbContext`/entity của `Winform4System.DataAccess`.
- Ưu tiên quy trình Database First: database là nguồn cấu trúc chính; khi schema thay đổi phải cập nhật model/mapping EF tương ứng.
- Không viết câu SQL thuần trong code C# và không sử dụng `SqlCommand`, `ExecuteSqlCommand`, `SqlQuery` hoặc API tương đương để bỏ qua EF.
- Mọi thao tác đọc, thêm, sửa, xóa và transaction phải thực hiện qua EF6 repository/service. Không để UI truy cập `DbContext` trực tiếp.
- SQL thuần chỉ được phép trong các file migration, seed hoặc script quản trị nằm dưới thư mục `Database`; không nhúng các câu lệnh này vào application runtime.

## Định danh người dùng

- `UserId` là mã nhân viên duy nhất, đồng thời là tài khoản đăng nhập và khóa chính được các bảng nghiệp vụ tham chiếu.
- Định dạng chuẩn của `UserId` là `VNW` theo sau bởi đúng 7 chữ số, tổng cộng 10 ký tự, ví dụ `VNW0014732`.
- Lưu `UserId` bằng `varchar(10)`, chuẩn hóa chữ hoa trước khi tra cứu và không tạo thêm khóa người dùng dạng số hoặc cột `LoginName` song song.
- Mọi khóa ngoại tham chiếu người dùng phải dùng cùng kiểu `varchar(10)` và tên cột `UserId` hoặc tên vai trò rõ ràng như `CreatedByUserId`, `UpdatedByUserId`.

## Tên người dùng

- Hồ sơ nhân viên phải lưu riêng tên Trung phồn thể trong `DisplayNameTW` và tên Việt trong `DisplayNameVN`; không dùng các cột mơ hồ như `FullName` hoặc `PreferredName`.
- Giao diện zh-Hant ưu tiên hiển thị `DisplayNameTW`, sau đó fallback sang `DisplayNameVN` và cuối cùng là `UserId` nếu hồ sơ chưa có tên.
- Khi truyền thông tin người dùng qua repository, business service hoặc session, phải giữ riêng cả hai tên để các module sau có thể chọn đúng ngôn ngữ.

## Trạng thái card chức năng

- Card chức năng phải lấy màu theo trạng thái phát triển, không gán màu tùy ý theo từng chức năng.
- Chưa triển khai (`NotStarted`): tím `RGB(128, 57, 123)`.
- Đang phát triển (`InProgress`): nâu đỏ `RGB(183, 71, 42)`.
- Đã hoàn thành (`Completed`): xanh `RGB(16, 110, 190)`.
- Tiêu đề card trên Main dùng font `DFKai-SB` cỡ `26F`, theo giao diện của `7system`.
