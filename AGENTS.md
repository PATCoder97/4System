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

## Trạng thái card chức năng

- Card chức năng phải lấy màu theo trạng thái phát triển, không gán màu tùy ý theo từng chức năng.
- Chưa triển khai (`NotStarted`): tím `RGB(128, 57, 123)`.
- Đang phát triển (`InProgress`): nâu đỏ `RGB(183, 71, 42)`.
- Đã hoàn thành (`Completed`): xanh `RGB(16, 110, 190)`.
- Tiêu đề card trên Main dùng font `DFKai-SB` cỡ `26F`, theo giao diện của `7system`.
