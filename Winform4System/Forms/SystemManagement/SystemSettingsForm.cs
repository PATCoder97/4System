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

            if (CurrentAuthorization.HasPermission("SYSTEM.FUNCTION.VIEW"))
            {
                AddNavigationItem(
                    settingsGroup,
                    "function-cards",
                    "功能卡管理",
                    SvgIconCatalog.Equipment,
                    () => new FunctionManagementView());

                if (!initialTabRegistered)
                    Shown += (sender, args) => OpenTab("function-cards", "功能卡管理", () => new FunctionManagementView());
            }
        }
    }
}
