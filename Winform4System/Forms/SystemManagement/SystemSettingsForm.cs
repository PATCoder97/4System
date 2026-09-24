using Winform4System.Core.Security;
using Winform4System.Forms.Common;
using Winform4System.Helpers;

namespace Winform4System.Forms.SystemManagement
{
    public sealed class SystemSettingsForm : FluentModuleForm
    {
        public SystemSettingsForm()
            : base("系統設定", "systemSettings")
        {
            var settingsGroup = AddNavigationGroup("系統設定");

            if (CurrentAuthorization.HasPermission("SYSTEM.FUNCTION.VIEW"))
            {
                AddNavigationItem(
                    settingsGroup,
                    "function-cards",
                    "功能卡管理",
                    SvgIconCatalog.Equipment,
                    () => new FunctionManagementView());

                Shown += (sender, args) => OpenTab(
                    "function-cards",
                    "功能卡管理",
                    () => new FunctionManagementView());
            }
        }
    }
}
