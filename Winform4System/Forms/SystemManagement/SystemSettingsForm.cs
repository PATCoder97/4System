using Winform4System.Core.Security;
using Winform4System.Forms.Common;
using Winform4System.Helpers;

namespace Winform4System.Forms.SystemManagement
{
    public sealed class SystemSettingsForm : FluentModuleForm
    {
        public SystemSettingsForm()
            : base("系統管理", "systemSettings")
        {
            var settingsGroup = AddNavigationGroup("系統管理");
            bool initialTabRegistered = false;

            if (CurrentAuthorization.HasPermission("SYSTEM.USER.VIEW"))
            {
                AddNavigationItem(settingsGroup, "employees", "人員管理", SvgIconCatalog.ChangeUser, () => new EmployeeManagementView());
                Shown += (sender, args) => OpenTab("employees", "人員管理", () => new EmployeeManagementView());
                initialTabRegistered = true;
            }

            if (CurrentAuthorization.HasPermission("SYSTEM.USER.PERMISSION.VIEW"))
            {
                AddNavigationItem(settingsGroup, "employee-permissions", "人員權限", SvgIconCatalog.AddUserGroup, () => new EmployeePermissionView());
                if (!initialTabRegistered)
                {
                    Shown += (sender, args) => OpenTab("employee-permissions", "人員權限", () => new EmployeePermissionView());
                    initialTabRegistered = true;
                }
            }

            if (CurrentAuthorization.HasPermission("SYSTEM.GROUP.VIEW"))
            {
                AddNavigationItem(settingsGroup, "security-groups", "安全性群組管理", SvgIconCatalog.AddUserGroup, () => new SecurityGroupManagementView());
                if (!initialTabRegistered)
                {
                    Shown += (sender, args) => OpenTab("security-groups", "安全性群組管理", () => new SecurityGroupManagementView());
                    initialTabRegistered = true;
                }
            }

            if (CurrentAuthorization.HasPermission("SYSTEM.ROLE.VIEW"))
            {
                AddNavigationItem(settingsGroup, "roles-permissions", "角色與權限管理", SvgIconCatalog.SelectionChecked, () => new RolePermissionManagementView());
                if (!initialTabRegistered)
                {
                    Shown += (sender, args) => OpenTab("roles-permissions", "角色與權限管理", () => new RolePermissionManagementView());
                    initialTabRegistered = true;
                }
            }

            if (CurrentAuthorization.HasPermission("SYSTEM.AUDIT.VIEW"))
            {
                AddNavigationItem(settingsGroup, "audit-logs", "稽核記錄", SvgIconCatalog.View, () => new AuditLogView());
                if (!initialTabRegistered)
                {
                    Shown += (sender, args) => OpenTab("audit-logs", "稽核記錄", () => new AuditLogView());
                    initialTabRegistered = true;
                }
            }

            if (CurrentAuthorization.HasPermission("SYSTEM.FUNCTION.VIEW"))
            {
                AddNavigationItem(
                    settingsGroup,
                    "function-cards",
                    "功能卡管理",
                    SvgIconCatalog.Equipment,
                    () => new FunctionManagementView());

                if (!initialTabRegistered)
                {
                    Shown += (sender, args) => OpenTab("function-cards", "功能卡管理", () => new FunctionManagementView());
                    initialTabRegistered = true;
                }
            }
        }
    }
}
