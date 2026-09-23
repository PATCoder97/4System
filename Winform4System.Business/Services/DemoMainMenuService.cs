using System.Collections.Generic;
using System.Drawing;
using Winform4System.Core.Models;

namespace Winform4System.Business.Services
{
    public sealed class DemoMainMenuService : IMainMenuService
    {
        public IReadOnlyList<MenuItemDefinition> GetMenuItems()
        {
            return new List<MenuItemDefinition>
            {
                new MenuItemDefinition("SALES_CERT", "Sản xuất & kinh doanh", "Quản lý chứng chỉ", "Theo dõi hồ sơ và chứng nhận", Color.FromArgb(17, 112, 186)),
                new MenuItemDefinition("SALES_ORDER", "Sản xuất & kinh doanh", "Sản xuất & đơn hàng", "Kế hoạch và tiến độ thực hiện", Color.FromArgb(0, 137, 123)),
                new MenuItemDefinition("CONTRACT", "Sản xuất & kinh doanh", "Quản lý hợp đồng", "Hợp đồng và tài liệu liên quan", Color.FromArgb(94, 53, 177)),

                new MenuItemDefinition("ISO", "Tiêu chuẩn & kỹ thuật", "Tài liệu ISO", "Tiêu chuẩn và tài liệu nội bộ", Color.FromArgb(21, 101, 192)),
                new MenuItemDefinition("PROJECT", "Tiêu chuẩn & kỹ thuật", "Dự án kỹ thuật", "Cải tiến kỹ thuật và IE", Color.FromArgb(2, 119, 189)),
                new MenuItemDefinition("KNOWLEDGE", "Tiêu chuẩn & kỹ thuật", "Kho tri thức", "Tìm kiếm và chia sẻ kiến thức", Color.FromArgb(0, 121, 107)),
                new MenuItemDefinition("STANDARD", "Tiêu chuẩn & kỹ thuật", "Tiêu chuẩn quốc tế", "Danh mục tiêu chuẩn áp dụng", Color.FromArgb(69, 90, 100)),

                new MenuItemDefinition("WORK", "Quản lý bộ phận", "Quản lý công việc", "Công việc và tiến độ bộ phận", Color.FromArgb(245, 124, 0)),
                new MenuItemDefinition("ASSET", "Quản lý bộ phận", "Kho & tài sản", "Thiết bị, vật tư và tài sản", Color.FromArgb(239, 108, 0)),
                new MenuItemDefinition("SAFETY", "Quản lý bộ phận", "An toàn & môi trường", "Nghiệp vụ EHS", Color.FromArgb(198, 40, 40)),
                new MenuItemDefinition("SIGN", "Quản lý bộ phận", "Ký duyệt điện tử", "Luồng duyệt và chữ ký", Color.FromArgb(173, 20, 87)),

                new MenuItemDefinition("USERS", "Quản trị hệ thống", "Quản lý người dùng", "Tài khoản và đơn vị", Color.FromArgb(55, 71, 79)),
                new MenuItemDefinition("ROLES", "Quản trị hệ thống", "Vai trò & phân quyền", "Quyền truy cập chức năng", Color.FromArgb(62, 39, 35)),
                new MenuItemDefinition("SETTINGS", "Quản trị hệ thống", "Cấu hình hệ thống", "Tham số và thiết lập chung", Color.FromArgb(66, 66, 66)),
                new MenuItemDefinition("MANUAL", "Quản trị hệ thống", "Hướng dẫn sử dụng", "Tài liệu hỗ trợ người dùng", Color.FromArgb(84, 110, 122))
            };
        }
    }
}
