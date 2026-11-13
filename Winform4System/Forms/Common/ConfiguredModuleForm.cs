using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraBars.Navigation;
using Winform4System.Business.Services;
using Winform4System.DataAccess.Configuration;
using Winform4System.DataAccess.Entities;
using Winform4System.Forms.SpareParts;
using Winform4System.Forms.SystemManagement;
using Winform4System.Helpers;

namespace Winform4System.Forms.Common
{
    public abstract class ConfiguredModuleForm : FluentModuleForm
    {
        private readonly ModuleNavigationService service = new ModuleNavigationService(new ConnectionStringProvider());
        private readonly string moduleCode;
        private Action openInitial;

        protected ConfiguredModuleForm(string code, string componentName) : base("功能模組", componentName)
        {
            moduleCode = code;
            var functions = service.GetFunctions(code);
            var root = functions.SingleOrDefault(x => x.FunctionCode == code);
            if (root != null)
            {
                Text = root.DisplayName;
                AddChildren(root, functions, null);
            }
            Shown += (sender, args) => openInitial?.Invoke();
        }

        private void AddChildren(ApplicationFunction parent, IReadOnlyList<ApplicationFunction> functions, AccordionControlElement group)
        {
            AccordionControlElement defaultGroup = group;
            foreach (var function in functions.Where(x => x.ParentFunctionId == parent.FunctionId))
            {
                if (string.IsNullOrWhiteSpace(function.NavigationTarget))
                {
                    var childGroup = AddNavigationGroup(function.DisplayName, parent: group);
                    childGroup.ImageOptions.SvgImage = SvgIconCatalog.Resolve(function.IconKey, SvgIconCatalog.Department);
                    AddChildren(function, functions, childGroup);
                    continue;
                }
                Func<Control> factory = GetFactory(function.NavigationTarget);
                if (factory == null) continue;
                if (defaultGroup == null)
                {
                    defaultGroup = AddNavigationGroup(parent.DisplayName);
                    defaultGroup.ImageOptions.SvgImage = SvgIconCatalog.Resolve(parent.IconKey, SvgIconCatalog.Department);
                }
                Func<Control> create = () =>
                {
                    service.DemandFunction(moduleCode, function.FunctionCode);
                    return factory();
                };
                var target = Winform4System.Core.Models.FunctionCatalog.Find(function.NavigationTarget);
                AddNavigationItem(defaultGroup, function.FunctionCode, function.DisplayName,
                    SvgIconCatalog.Resolve(function.IconKey, SvgIconCatalog.Resolve(target?.IconKey)), create);
                if (openInitial == null) openInitial = () => OpenTab(function.FunctionCode, function.DisplayName, create);
            }
        }

        private static Func<Control> GetFactory(string target)
        {
            switch (target)
            {
                case "SparePartMaterialView": return () => new SparePartMaterialView();
                case "SparePartMachineView": return () => new SparePartMachineView();
                case "SparePartTransactionView": return () => new SparePartTransactionView();
                case "SparePartRecheckView": return () => new SparePartRecheckView();
                case "SparePartRecoveryTaskView": return () => new SparePartRecoveryTaskView();
                case "SparePartInspectionView": return () => new SparePartInspectionView();
                case "SparePartCostView": return () => new SparePartCostView();
                case "SparePartRecoveryView": return () => new SparePartRecoveryView();
                case "EmployeeManagementView": return () => new EmployeeManagementView();
                case "DepartmentManagementView": return () => new DepartmentManagementView();
                case "JobTitleManagementView": return () => new JobTitleManagementView();
                case "EmployeePermissionView": return () => new EmployeePermissionView();
                case "SecurityGroupManagementView": return () => new SecurityGroupManagementView();
                case "RolePermissionManagementView": return () => new RolePermissionManagementView();
                case "AuditLogView": return () => new AuditLogView();
                case "FunctionManagementView": return () => new FunctionManagementView();
                case "SystemSettingManagementView": return () => new SystemSettingManagementView();
                case "SystemHealthView": return () => new SystemHealthView();
                default: return null;
            }
        }
    }
}
