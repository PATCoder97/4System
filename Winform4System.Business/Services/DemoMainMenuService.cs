using System.Collections.Generic;
using Winform4System.Core.Models;

namespace Winform4System.Business.Services
{
    public sealed class DemoMainMenuService : IMainMenuService
    {
        public IReadOnlyList<MenuItemDefinition> GetMenuItems()
        {
            return new List<MenuItemDefinition>
            {
                new MenuItemDefinition(
                    "ASSET",
                    "部門管理",
                    "機邊庫",
                    "設備、物料與固定資產",
                    MenuDevelopmentStatus.InProgress),

                new MenuItemDefinition("USERS", "系統管理", "使用者管理", "帳號與部門管理", MenuDevelopmentStatus.NotStarted),
                new MenuItemDefinition("ROLES", "系統管理", "角色與權限", "功能存取權限管理", MenuDevelopmentStatus.NotStarted),
                new MenuItemDefinition("SETTINGS", "系統管理", "系統設定", "系統參數與一般設定", MenuDevelopmentStatus.NotStarted),
                new MenuItemDefinition("MANUAL", "系統管理", "操作手冊", "使用者操作說明", MenuDevelopmentStatus.NotStarted)
            };
        }
    }
}
