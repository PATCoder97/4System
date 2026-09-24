# Kế hoạch hoàn thiện quản trị hệ thống

Tài liệu này dùng làm backlog triển khai dần cho module `系統管理`. Các chức năng giao diện phải tuân thủ `AGENTS.md`: tiếng Trung phồn thể, DevExpress, `FluentModuleForm`, `XtraUserControl`, `LayoutControl`, EF6 và kiểm tra permission tại cả UI lẫn Business.

## Hiện trạng

- [x] Khung module `系統管理` và điều hướng theo permission.
- [x] `人員管理`: quản lý hồ sơ nhân viên và tài khoản đăng nhập.
- [x] `人員權限`: gán nhân viên vào nhóm bảo mật và xem quyền hiệu lực.
- [x] `功能卡管理`: chỉnh cấu hình card chức năng.
- [x] `安全性群組管理`: quản lý nhóm, vai trò của nhóm và xem thành viên.
- [x] `角色與權限管理`: quản lý vai trò, ma trận quyền và phạm vi ảnh hưởng.
- [x] `稽核記錄`: tra cứu, xem chi tiết và xuất nhật ký theo quyền.
- [ ] Quản trị vòng đời và bảo mật tài khoản chưa đầy đủ.

## Nguyên tắc phân quyền

```text
Nhân viên/Tài khoản
  → Nhóm bảo mật
  → Vai trò
  → Quyền
  → Chức năng + hành động
```

- Không gán permission trực tiếp cho từng nhân viên.
- Trường hợp đặc biệt phải tạo nhóm bảo mật có tên và mục đích rõ ràng.
- Business service luôn kiểm tra permission lại trước khi thay đổi dữ liệu.
- Mọi thay đổi nhân viên, nhóm, vai trò và quyền phải ghi audit.
- Thay đổi quyền phải có cách buộc phiên đăng nhập cập nhật hoặc đăng nhập lại.

## Giai đoạn 0 — Sửa lỗi bảo mật nền tảng

Mức ưu tiên: **Khẩn cấp**

- [x] Sửa `CurrentAuthorization.HasPermission` để `ASSET.SPARE_PART.ADMIN` chỉ có hiệu lực với `ASSET.SPARE_PART.*`, không trở thành quyền admin toàn hệ thống.
- [x] Thêm kiểm thử phạm vi permission, bao gồm quyền hệ thống, phụ tùng và quyền không liên quan.
- [x] Chống quản trị viên tự vô hiệu hóa tài khoản đang đăng nhập.
- [x] Chống gỡ người cuối cùng khỏi nhóm quản trị hệ thống.
- [x] Bổ sung kiểm soát concurrency bằng `RowVersion` cho các bảng quản trị quan trọng.

Tiêu chí hoàn thành:

- Tài khoản quản lý phụ tùng không thể truy cập chức năng quản trị hệ thống nếu không được cấp quyền tương ứng.
- Hệ thống luôn còn ít nhất một quản trị viên đang hoạt động.
- Xung đột cập nhật được cảnh báo bằng tiếng Trung phồn thể thay vì ghi đè âm thầm.

## Giai đoạn 1 — Quản lý nhóm bảo mật

Tên giao diện: `安全性群組管理`

Permission:

- `SYSTEM.GROUP.VIEW`
- `SYSTEM.GROUP.ADMIN`

Phạm vi:

- [x] Danh sách nhóm: mã, tên, mô tả, nhóm hệ thống, trạng thái.
- [x] Thêm, sửa và ngừng sử dụng nhóm.
- [x] Gán hoặc gỡ vai trò khỏi nhóm.
- [x] Xem danh sách nhân viên thuộc nhóm.
- [x] Hiển thị tổng số vai trò và nhân viên của nhóm.
- [x] Không cho sửa mã và không cho ngừng nhóm hệ thống khi còn thành viên.
- [x] Cảnh báo ảnh hưởng trước khi ngừng nhóm hoặc thay đổi vai trò.
- [x] Ghi audit cho mọi thay đổi.

Tiêu chí hoàn thành:

- Quản trị viên có thể hoàn chỉnh quan hệ `Nhóm → Vai trò` mà không dùng SQL.
- Sau khi lưu, màn hình gọi tải lại và giữ dòng đang chọn.

## Giai đoạn 2 — Quản lý vai trò và quyền

Tên giao diện: `角色與權限管理`

Permission:

- `SYSTEM.ROLE.VIEW`
- `SYSTEM.ROLE.ADMIN`

Phạm vi:

- [x] Danh sách vai trò: mã, tên, mô tả, vai trò hệ thống, trạng thái.
- [x] Thêm, sửa và ngừng sử dụng vai trò.
- [x] Ma trận chức năng × hành động.
- [x] Hỗ trợ các hành động `ACCESS`, `VIEW`, `CREATE`, `UPDATE`, `DELETE`, `APPROVE`, `EXPORT`, `ADMIN`.
- [x] Gán hoặc gỡ permission khỏi vai trò.
- [x] Xem các nhóm và nhân viên bị ảnh hưởng.
- [x] Hiển thị số nhóm/người bị ảnh hưởng trước khi lưu.
- [x] Bảo vệ vai trò hệ thống khi vẫn còn gán cho nhóm.
- [x] Ghi audit đầy đủ.

Tiêu chí hoàn thành:

- Quản trị viên có thể hoàn chỉnh quan hệ `Vai trò → Quyền` mà không dùng SQL.
- Có thể truy ngược một quyền đến vai trò, nhóm và nhân viên nhận quyền đó.

## Giai đoạn 3 — Nhật ký kiểm toán

Tên giao diện: `稽核記錄`

Permission đề xuất:

- `SYSTEM.AUDIT.VIEW`
- `SYSTEM.AUDIT.EXPORT`

Phạm vi:

- [x] Ghi audit khi thêm/sửa/ngừng nhân viên và tài khoản.
- [x] Ghi audit khi thay đổi nhóm của nhân viên.
- [x] Ghi audit khi thay đổi nhóm, vai trò hoặc permission.
- [x] Ghi đăng nhập thành công, thất bại, khóa và tự động mở khóa tài khoản.
- [x] Danh sách lọc theo thời gian, người thao tác, hành động và đối tượng.
- [x] Xem chi tiết mô tả, dữ liệu JSON và mã liên kết của bản ghi.
- [x] Xuất Excel theo permission và ghi audit cho thao tác xuất.
- [x] Nhật ký chỉ đọc, không cho sửa hoặc xóa trên giao diện.

Tiêu chí hoàn thành:

- Có thể trả lời được ai đã thay đổi quyền gì, cho ai, vào thời điểm nào.

## Giai đoạn 4 — Bảo mật và vòng đời tài khoản

Tên giao diện có thể tích hợp vào `人員管理` và menu chuột phải.

- [x] Mở khóa tài khoản và đặt lại `FailedLoginCount`.
- [x] Chỉ sử dụng xác thực domain `vn.fpg.com` theo `7system`, không phân loại LOCAL/Windows trên giao diện.
- [x] Cho phép xác thực bằng hash của lần đăng nhập domain thành công gần nhất khi domain controller không khả dụng.
- [x] Bật/tắt tài khoản độc lập với trạng thái hồ sơ nhân viên khi nghiệp vụ yêu cầu.
- [x] Thu hồi phiên đăng nhập bằng `SecurityStamp` hoặc cơ chế phiên tương đương.
- [x] Không quản lý hoặc buộc đổi mật khẩu domain trong ứng dụng; chỉ lưu hash để kiểm tra ngoại tuyến.
- [ ] Chính sách số lần đăng nhập sai và thời hạn khóa từ tham số hệ thống.
- [x] Hiển thị lần đăng nhập cuối và trạng thái khóa.
- [x] Xác nhận và audit mọi thao tác bảo mật.

Tiêu chí hoàn thành:

- Quản trị viên xử lý đầy đủ tài khoản bị khóa hoặc cần thu hồi quyền mà không thao tác database.

## Giai đoạn 5 — Danh mục tổ chức

Tên giao diện:

- `部門管理`
- `職稱管理`

Permission đề xuất:

- `SYSTEM.DEPARTMENT.VIEW/ADMIN`
- `SYSTEM.JOB_TITLE.VIEW/ADMIN`

Phạm vi:

- [ ] Cây phòng ban bằng `TreeList`.
- [ ] Thêm, sửa, sắp xếp và ngừng sử dụng phòng ban.
- [ ] Quản lý chức danh.
- [ ] Không cho ngừng danh mục đang được nhân viên hoạt động sử dụng nếu chưa chuyển dữ liệu.
- [ ] Dialog chuyển nhân viên khi tái cấu trúc phòng ban.

Tiêu chí hoàn thành:

- Có thể tạo nhân viên mới và duy trì cơ cấu tổ chức hoàn toàn từ giao diện.

## Giai đoạn 6 — Tham số và trạng thái hệ thống

Tên giao diện:

- `系統參數`
- `系統狀態`

Permission đề xuất:

- `SYSTEM.SETTING.VIEW/ADMIN`
- `SYSTEM.HEALTH.VIEW`

Phạm vi:

- [ ] Quản lý tham số không nhạy cảm: giới hạn upload, thời gian khóa, đường dẫn nghiệp vụ và cấu hình cảnh báo.
- [ ] Không hiển thị hoặc lưu bí mật ở dạng rõ trên UI.
- [ ] Kiểm tra kết nối database và thư mục chia sẻ.
- [ ] Hiển thị phiên bản ứng dụng, phiên bản schema và môi trường.
- [ ] Hiển thị tình trạng lưu trữ và các lỗi cấu hình quan trọng.
- [ ] Ghi audit khi thay đổi tham số.

## Giai đoạn 7 — Khôi phục và vận hành

- [ ] Quy trình bootstrap quản trị viên đầu tiên được tài liệu hóa và kiểm thử.
- [ ] Quy trình khôi phục khi không còn tài khoản quản trị sử dụng được.
- [ ] Kiểm tra backup database gần nhất; không tự chạy backup nếu chưa có cơ chế vận hành được phê duyệt.
- [ ] Trang hướng dẫn xử lý sự cố đăng nhập, quyền và kết nối dữ liệu.
- [ ] Checklist triển khai script database theo đúng thứ tự.

## Thứ tự triển khai đề xuất

1. Giai đoạn 0 — sửa phạm vi admin và bảo vệ chống tự khóa.
2. Giai đoạn 1 — quản lý nhóm bảo mật.
3. Giai đoạn 2 — quản lý vai trò và ma trận quyền.
4. Giai đoạn 3 — audit đầy đủ.
5. Giai đoạn 4 — vòng đời tài khoản.
6. Giai đoạn 5 — phòng ban và chức danh.
7. Giai đoạn 6 — tham số và trạng thái hệ thống.
8. Giai đoạn 7 — khôi phục và vận hành.

## Checklist chung cho từng chức năng

- [ ] Script database idempotent và được thêm vào `Database/README.md`.
- [ ] Entity/mapping EF6 được cập nhật theo database.
- [ ] UI không truy cập `DbContext` trực tiếp.
- [ ] Business service kiểm tra permission trước khi đọc hoặc ghi.
- [ ] UI tiếng Trung phồn thể và font đúng chuẩn.
- [ ] Control cố định được khai báo trong `.Designer.cs`.
- [ ] Toolbar, icon, grid và dialog đồng nhất với module Spare.
- [ ] Thao tác theo bản ghi nằm trong menu chuột phải.
- [ ] Thao tác quan trọng có xác nhận và audit.
- [ ] Sau khi lưu, danh sách tải lại và giữ bản ghi đang chọn.
- [ ] Build Debug thành công.
- [ ] Kiểm thử tối thiểu cho quyền được phép, bị từ chối và xung đột dữ liệu.
