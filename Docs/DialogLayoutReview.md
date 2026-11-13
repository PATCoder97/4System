# Rà soát bố cục dialog — 2026-11-13

Áp dụng playbook `layout`, `operate`, `craft-floor` và `polish` của [Impeccable chính thức](https://github.com/pbakaus/impeccable/blob/main/.agents/skills/impeccable/SKILL.md), kết hợp quy định DevExpress/zh-Hant trong AGENTS.md. Giữ skin WXI và font Microsoft JhengHei UI của dự án.

Launcher Impeccable không tải được engine trong phiên này. Phần kiểm tra cơ học thực hiện bằng đo bounds/minimum size của LayoutControl khi chạy WinForms, không phải kết quả từ detector CLI.

## Phạm vi và bằng chứng

- Render toàn bộ 20 dialog nghiệp vụ trong SystemManagement và SpareParts bằng dữ liệu mẫu trên Test.
- Lưu 32 ảnh/trạng thái: mặc định, ba kiểu tham số, các chế độ kho, kích thước tối thiểu của ba dialog co giãn và hai trường hợp tên dài.
- Kiểm tra độc lập source và ảnh render trước/sau. Không coi vùng grid/list đang rỗng là khoảng trống thừa.
- Kết quả ở DPI 96: không có control vượt vùng layout hiển thị; không có layout hiển thị nhỏ hơn MinSize; không phát sinh modal lỗi trong bộ capture.
- Đã build Debug và Release thành công. Dữ liệu mẫu tạm đã được dọn.
- Chưa thay đổi thiết lập DPI Windows để chạy thử ở 125%/150%; không xem kiểm tra bounds ở DPI 96 là bằng chứng cho các mức DPI đó.

## Các sửa chính

Kích thước dưới đây là **ClientSize đo khi chạy**, không phải kích thước ngoài cửa sổ hay giá trị chưa scale trong Designer.

| Dialog | Trước | Sau | Thay đổi |
|---|---:|---:|---|
| AuthenticationPolicyForm | 620 × 300 | 620 × 184 | Bỏ vùng trắng dưới giải thích |
| DepartmentEditForm | 620 × 354 | 620 × 262 | Thu khoảng đáy |
| JobTitleEditForm | 620 × 300 | 620 × 227 | Thu khoảng đáy |
| SystemSettingEditForm | 760 × 429 | 760 × 274 | Thu khoảng đáy ở STRING/INTEGER/BOOLEAN |
| FunctionEditForm | 720 × 612 | 720 × 559 | Giữ đủ memo và ba checkbox, giảm tail |
| EmployeeEditForm | 700 × 500 | 700 × 448 | Căn thứ tự trường và Tab, bỏ placeholder tiếng Anh |
| RolePermissionEditForm | 1200 × 857 | 1100 × 686 | Metadata trên cùng, tab dành cho dữ liệu; đủ tám cột quyền |
| SecurityGroupEditForm | 960 × 677 | 920 × 540 | Metadata trên cùng, tab role/thành viên co giãn |
| AuditLogDetailForm | 980 × 771 | 980 × 664 | Thông tin định danh hai cột, memo nội dung co giãn |
| SparePartRecoveryScheduleForm | 560 × 169 | 560 × 145 | Hai hàng editor có chiều cao nhất quán |
| SparePartMachineForm | 567 × 151 | 640 × 151 | Tăng chỗ cho tên/vị trí/số lượng |
| SparePartMaterialForm | 431 × 597 | 640 × 597 | Tăng chiều rộng, giữ vùng nhập tên dài/chọn thiết bị |

`SparePartTransactionForm` dùng client width 640, thay vì 398/428. Chiều cao được tính từ layout đang hiển thị sau khi BarManager hoàn tất dock, gồm phần đệm đáy và giới hạn WorkingArea. Đã kiểm tra 收貨, 調撥, 盤點 và 領用 ở ba lựa chọn xử lý hàng cũ. Không còn scrollbar dư ở kích thước mặc định. Cột số lượng cố định chiều rộng phù hợp; phần còn lại dành cho tên kho.

Các control, popup view/cột và layout cố định của phần thu hồi trong dialog kho đã chuyển về Designer. Code-behind chỉ giữ nạp dữ liệu, điều khiển trạng thái và tính kích thước theo các trường đang hiển thị.

## Nguyên tắc giữ lại

- Giữ nhịp hàng editor khoảng 36 px, caption/editor 14.25F và khoảng đáy nhỏ; không giảm font để nhét nội dung.
- Thứ tự nhân viên: mã → tên → tổ chức → liên hệ → tình trạng làm việc/tài khoản. Thứ tự Tab tương ứng.
- Audit ưu tiên thời gian/người thao tác/hành động trước thông tin máy/IP/correlation.
- Bảng quyền, nhóm, thành viên, kiểm kê, tài liệu và chuỗi thay thế tiếp tục được co giãn. Khoảng trắng bên trong danh sách rỗng dành cho dữ liệu thực tế.
- Dialog quyền/nhóm/audit được giới hạn theo WorkingArea khi mở để phù hợp màn hình nhỏ; bảng vẫn hỗ trợ cuộn khi cần.
- Không thay đổi quy tắc nghiệp vụ, quyền, dữ liệu Prod hoặc schema database trong đợt chỉnh layout này.
