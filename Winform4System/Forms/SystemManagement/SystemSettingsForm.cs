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
            bool canViewEmployees = CurrentAuthorization.HasPermission("SYSTEM.USER.VIEW");
            bool canViewEmployeePermissions = CurrentAuthorization.HasPermission("SYSTEM.USER.PERMISSION.VIEW");
            bool canViewDepartments = CurrentAuthorization.HasPermission("SYSTEM.DEPARTMENT.VIEW");
            bool canViewJobTitles = CurrentAuthorization.HasPermission("SYSTEM.JOB_TITLE.VIEW");
            bool canViewGroups = CurrentAuthorization.HasPermission("SYSTEM.GROUP.VIEW");
            bool canViewRoles = CurrentAuthorization.HasPermission("SYSTEM.ROLE.VIEW");
            bool canViewAuditLogs = CurrentAuthorization.HasPermission("SYSTEM.AUDIT.VIEW");
            bool canViewFunctions = CurrentAuthorization.HasPermission("SYSTEM.FUNCTION.VIEW");
            bool canViewSettings = CurrentAuthorization.HasPermission("SYSTEM.SETTING.VIEW");
            bool canViewHealth = CurrentAuthorization.HasPermission("SYSTEM.HEALTH.VIEW");

            var peopleGroup = canViewEmployees || canViewDepartments || canViewJobTitles
                ? AddNavigationGroup("人員與組織", "人員、部門與職稱")
                : null;
            var authorizationGroup = canViewEmployeePermissions || canViewGroups || canViewRoles
                ? AddNavigationGroup("權限與安全", "人員權限、群組與角色")
                : null;
            var governanceGroup = canViewAuditLogs || canViewFunctions || canViewSettings || canViewHealth
                ? AddNavigationGroup("稽核與設定", "稽核記錄與功能設定")
                : null;
            bool initialTabRegistered = false;

            if (canViewEmployees)
            {
                AddNavigationItem(peopleGroup, "employees", "人員管理", SvgIconCatalog.ChangeUser, () => new EmployeeManagementView());
                Shown += (sender, args) => OpenTab("employees", "人員管理", () => new EmployeeManagementView());
                initialTabRegistered = true;
            }

            if (canViewDepartments)
            {
                AddNavigationItem(peopleGroup, "departments", "部門管理", SvgIconCatalog.Department, () => new DepartmentManagementView());
                if (!initialTabRegistered)
                {
                    Shown += (sender, args) => OpenTab("departments", "部門管理", () => new DepartmentManagementView());
                    initialTabRegistered = true;
                }
            }

            if (canViewJobTitles)
            {
                AddNavigationItem(peopleGroup, "job-titles", "職稱管理", SvgIconCatalog.Promote, () => new JobTitleManagementView());
                if (!initialTabRegistered)
                {
                    Shown += (sender, args) => OpenTab("job-titles", "職稱管理", () => new JobTitleManagementView());
                    initialTabRegistered = true;
                }
            }

            if (canViewEmployeePermissions)
            {
                AddNavigationItem(authorizationGroup, "employee-permissions", "人員權限", SvgIconCatalog.AddUserGroup, () => new EmployeePermissionView());
                if (!initialTabRegistered)
                {
                    Shown += (sender, args) => OpenTab("employee-permissions", "人員權限", () => new EmployeePermissionView());
                    initialTabRegistered = true;
                }
            }

            if (canViewGroups)
            {
                AddNavigationItem(authorizationGroup, "security-groups", "安全性群組管理", SvgIconCatalog.AddUserGroup, () => new SecurityGroupManagementView());
                if (!initialTabRegistered)
                {
                    Shown += (sender, args) => OpenTab("security-groups", "安全性群組管理", () => new SecurityGroupManagementView());
                    initialTabRegistered = true;
                }
            }

            if (canViewRoles)
            {
                AddNavigationItem(authorizationGroup, "roles-permissions", "角色與權限管理", SvgIconCatalog.SelectionChecked, () => new RolePermissionManagementView());
                if (!initialTabRegistered)
                {
                    Shown += (sender, args) => OpenTab("roles-permissions", "角色與權限管理", () => new RolePermissionManagementView());
                    initialTabRegistered = true;
                }
            }

            if (canViewAuditLogs)
            {
                AddNavigationItem(governanceGroup, "audit-logs", "稽核記錄", SvgIconCatalog.View, () => new AuditLogView());
                if (!initialTabRegistered)
                {
                    Shown += (sender, args) => OpenTab("audit-logs", "稽核記錄", () => new AuditLogView());
                    initialTabRegistered = true;
                }
            }

            if (canViewFunctions)
            {
                AddNavigationItem(
                    governanceGroup,
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

            if (canViewSettings)
            {
                AddNavigationItem(governanceGroup, "system-settings", "系統參數", SvgIconCatalog.Edit, () => new SystemSettingManagementView());
                if (!initialTabRegistered)
                {
                    Shown += (sender, args) => OpenTab("system-settings", "系統參數", () => new SystemSettingManagementView());
                    initialTabRegistered = true;
                }
            }

            if (canViewHealth)
            {
                AddNavigationItem(governanceGroup, "system-health", "系統狀態", SvgIconCatalog.Info, () => new SystemHealthView());
                if (!initialTabRegistered)
                {
                    Shown += (sender, args) => OpenTab("system-health", "系統狀態", () => new SystemHealthView());
                }
            }
        }
    }
}
