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
- Các module cấp cao có thanh điều hướng và nhiều chức năng con phải kế thừa khung `FluentModuleForm` dùng chung; không sao chép lại cấu hình `FluentDesignForm`, `AccordionControl` và tab trong từng module.
- Mỗi chức năng con trong module phải được triển khai dưới dạng `XtraUserControl` và mở trong tab của khung chung. Chỉ dùng `XtraForm` riêng cho dialog nhập liệu, xác nhận hoặc tác vụ cần cửa sổ độc lập.
- `XtraUserControl` nghiệp vụ và dialog nhập liệu phải dùng `LayoutControl` làm layout gốc để quản lý căn lề, co giãn và khoảng cách. Không bố trí giao diện chính bằng tọa độ tuyệt đối hoặc ghép nhiều panel thủ công khi `LayoutControl` đáp ứng được.
- Tất cả control cố định của form/UserControl phải được khai báo và cấu hình trong file `.Designer.cs` thông qua `InitializeComponent`, bao gồm layout, bar, nút, editor, grid/tree, cột cố định, repository editor và các layout item. Không sinh các control cố định trong constructor hoặc code-behind.
- Code-behind chỉ dùng để nạp dữ liệu, gắn hành vi nghiệp vụ, cập nhật trạng thái và tạo những control/menu thực sự thay đổi theo dữ liệu lúc chạy. Nếu cấu trúc giao diện không thay đổi theo dữ liệu thì phải đưa về Designer.
- Mỗi form/UserControl có giao diện phải dùng class `partial`, ghép đúng `.cs`, `.Designer.cs` và `.resx` khi có tài nguyên; file project phải khai báo `DependentUpon` đúng để Visual Studio Designer nhận diện.
- Nút thao tác của màn hình và dialog phải nằm trong vùng layout riêng, có thứ tự nhất quán; nội dung chính phải co giãn theo kích thước cửa sổ.
- Thanh lệnh chính của `XtraUserControl` và `XtraForm` phải ưu tiên `BarManager` kết hợp `Bar`, `BarButtonItem` và `StandaloneBarDockControl`/`BarDockControl`; không tự dựng toolbar bằng nhiều `SimpleButton` đặt theo tọa độ khi Bar đáp ứng được.
- Bộ lọc hoặc lựa chọn nằm trên thanh lệnh phải dùng `BarEditItem` cùng repository editor DevExpress phù hợp, ví dụ `RepositoryItemComboBox`, `RepositoryItemLookUpEdit` hoặc `RepositoryItemSearchLookUpEdit`.
- Mọi hành động áp dụng cho một dòng, bản ghi hoặc node cụ thể như xem, sửa, xóa và đổi trạng thái phải được đưa vào menu chuột phải của `GridView`/`TreeList`; không đặt các hành động theo dòng trên thanh lệnh chính. Khi người dùng nhấp chuột phải, phải chọn đúng dòng/node dưới con trỏ trước khi mở menu. Thanh lệnh chính chỉ dành cho thao tác toàn màn hình hoặc toàn danh sách như thêm mới, tải lại, lọc, xuất và in.
- Menu chuột phải trên `GridView`/`TreeList` phải dùng menu DevExpress như `PopupMenu`, `DXPopupMenu`, `DXMenuItem` và `DXSubMenuItem`. Ưu tiên xử lý sự kiện `PopupMenuShowing`, giữ nguyên menu mặc định do DevExpress cung cấp và thêm các hành động nghiệp vụ vào `e.Menu`; không thay menu gốc bằng một popup riêng nếu không có yêu cầu đặc biệt. Dùng `BeginGroup` để phân tách hành động nghiệp vụ với menu gốc và giữa các nhóm hành động khác mục đích.
- Menu mặc định do DevExpress sinh ra cũng phải được localize sang tiếng Trung phồn thể bằng localizer dùng chung; không để lẫn caption tiếng Anh hoặc tiếng Trung giản thể. Font menu mặc định là `Microsoft JhengHei UI` cỡ `12F`, bao gồm cả mục mặc định và mục nghiệp vụ được thêm lúc chạy.
- Các hành động chuẩn như thêm, sửa, xóa, xem, tải lại, xác nhận, hủy, xuất và in phải dùng icon từ catalog SVG dùng chung; kích thước icon và kiểu hiển thị caption phải đồng nhất trong cùng một thanh lệnh.
- Thanh lệnh chuẩn theo giao diện `7system`/module Spare phải dùng icon SVG kích thước `32x32`, hiển thị `CaptionGlyph`; font caption ở trạng thái Normal, Hovered và Pressed là `Microsoft JhengHei UI` cỡ `14.25F`, trạng thái Disabled là `12F`, màu chữ khả dụng là đen. Không để `BarManager` dùng font hoặc kích thước icon mặc định nhỏ.
- Editor và caption của `LayoutControlItem` trong form/dialog nhập liệu mặc định dùng `Microsoft JhengHei UI` cỡ `14.25F`; nội dung danh sách thả xuống cũng dùng cùng font. Chỉ giảm xuống `12F` khi không gian thực sự hạn chế và phải áp dụng đồng nhất trong toàn dialog.
- Nút hoặc menu không có quyền thực hiện phải được ẩn hoặc vô hiệu hóa ngay khi khởi tạo giao diện và cập nhật lại theo trạng thái bản ghi đang chọn.
- Dữ liệu phân cấp phải ưu tiên `TreeList`; dữ liệu phẳng ưu tiên `GridControl`/`GridView`. Cả hai phải dùng font giao diện chuẩn, tiêu đề rõ ràng và hỗ trợ lọc khi danh sách có thể dài.
- Tiêu đề cột của `GridView`/`TreeList` mặc định dùng `Microsoft JhengHei UI` cỡ `14.25F`; dòng dữ liệu mặc định dùng cỡ `12F`. Chỉ giảm cỡ chữ khi không gian hiển thị thực sự hạn chế và phải giữ đồng nhất trong toàn màn hình.
- Danh sách nghiệp vụ mặc định bật dòng lọc nhanh, ẩn group panel khi không có nhu cầu nhóm, tắt sửa trực tiếp nếu việc sửa phải qua dialog, và tránh focus từng ô khi người dùng thao tác theo dòng.
- Cột phải có caption tiếng Trung phồn thể, định dạng ngày/số rõ ràng, căn lề theo loại dữ liệu và chiều rộng hợp lý; không phụ thuộc hoàn toàn vào `BestFitColumns` nếu có cột mô tả dài hoặc cột nghiệp vụ quan trọng.
- Hành động áp dụng cho bản ghi đang chọn phải đặt trong menu chuột phải; trạng thái `Enabled`/`Visible` phải cập nhật theo quyền, loại bản ghi và trạng thái nghiệp vụ hiện tại.
- Sau khi dialog thêm/sửa/xóa hoàn tất thành công, màn hình gọi phải tải lại dữ liệu và cố gắng giữ lại dòng đang chọn khi điều đó giúp người dùng tiếp tục công việc.
- Dialog nhập liệu phải phân biệt rõ chế độ xem, thêm, sửa và xóa; thanh lệnh chỉ hiển thị các hành động hợp lệ cho chế độ hiện tại, đồng thời phải xác nhận trước thao tác phá hủy hoặc thay đổi trạng thái quan trọng.
- Menu, tab và nút thao tác phải được ẩn hoặc vô hiệu hóa theo permission trước khi người dùng thao tác; tầng Business vẫn phải kiểm tra permission lại trước khi thay đổi dữ liệu.

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
