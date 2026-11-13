using Winform4System.Forms.Common;
namespace Winform4System.Forms.SystemManagement
{
    public sealed class SystemSettingsForm : ConfiguredModuleForm
    {
        public SystemSettingsForm(string moduleCode = "SYSTEM.SETTINGS") : base(moduleCode, "systemSettings") { }
    }
}
