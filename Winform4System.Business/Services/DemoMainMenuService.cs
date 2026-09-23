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
                new MenuItemDefinition("SALES_CERT", "生產與銷售", "證書管理", "追蹤文件與證書", Color.FromArgb(17, 112, 186)),
                new MenuItemDefinition("SALES_ORDER", "生產與銷售", "生產與訂單", "生產計畫與訂單進度", Color.FromArgb(0, 137, 123)),
                new MenuItemDefinition("CONTRACT", "生產與銷售", "合同管理", "合同及相關文件", Color.FromArgb(94, 53, 177)),

                new MenuItemDefinition("ISO", "標準與技術", "ISO文件", "標準及內部文件", Color.FromArgb(21, 101, 192)),
                new MenuItemDefinition("PROJECT", "標準與技術", "技術專案", "技術專案與IE改善", Color.FromArgb(2, 119, 189)),
                new MenuItemDefinition("KNOWLEDGE", "標準與技術", "知識庫管理", "搜尋與分享知識", Color.FromArgb(0, 121, 107)),
                new MenuItemDefinition("STANDARD", "標準與技術", "國際規範", "適用國際規範清單", Color.FromArgb(69, 90, 100)),

                new MenuItemDefinition("WORK", "部門管理", "工作管理", "部門工作與執行進度", Color.FromArgb(245, 124, 0)),
                new MenuItemDefinition("ASSET", "部門管理", "機邊庫及固定資產", "設備、物料與固定資產", Color.FromArgb(239, 108, 0)),
                new MenuItemDefinition("SAFETY", "部門管理", "安全衛生與環境管理", "EHS相關業務", Color.FromArgb(198, 40, 40)),
                new MenuItemDefinition("SIGN", "部門管理", "電子核簽", "簽核流程與電子簽名", Color.FromArgb(173, 20, 87)),

                new MenuItemDefinition("USERS", "系統管理", "使用者管理", "帳號與部門管理", Color.FromArgb(55, 71, 79)),
                new MenuItemDefinition("ROLES", "系統管理", "角色與權限", "功能存取權限管理", Color.FromArgb(62, 39, 35)),
                new MenuItemDefinition("SETTINGS", "系統管理", "系統設定", "系統參數與一般設定", Color.FromArgb(66, 66, 66)),
                new MenuItemDefinition("MANUAL", "系統管理", "操作手冊", "使用者操作說明", Color.FromArgb(84, 110, 122))
            };
        }
    }
}
