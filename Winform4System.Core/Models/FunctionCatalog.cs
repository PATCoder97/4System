using System;
using System.Collections.Generic;
using System.Linq;

namespace Winform4System.Core.Models
{
    public sealed class FunctionTarget
    {
        public FunctionTarget(string key, string caption, string host, string permission, string icon, bool managerOnly = false)
        { Key = key; Caption = caption; Host = host; RequiredPermission = permission; IconKey = icon; ManagerOnly = managerOnly; }
        public string Key { get; }
        public string Caption { get; }
        public string Host { get; }
        public string RequiredPermission { get; }
        public string IconKey { get; }
        public bool ManagerOnly { get; }
        public bool IsModule => Host == null;
    }

    public static class FunctionCatalog
    {
        public static readonly IReadOnlyList<FunctionTarget> Targets = Array.AsReadOnly(new[]
        {
            new FunctionTarget("SparePartForm", "機邊庫模組", null, "ASSET.SPARE_PART.VIEW", "Equipment"),
            new FunctionTarget("SystemSettingsForm", "系統管理模組", null, "SYSTEM.SETTINGS.VIEW", "Department"),
            new FunctionTarget("SparePartMaterialView", "機邊庫／備品資料", "SparePartForm", "ASSET.SPARE_PART.VIEW", "View"),
            new FunctionTarget("SparePartMachineView", "機邊庫／設備管理", "SparePartForm", "ASSET.SPARE_PART.VIEW", "Equipment"),
            new FunctionTarget("SparePartTransactionView", "機邊庫／進出庫管理", "SparePartForm", "ASSET.SPARE_PART.VIEW", "Transfer"),
            new FunctionTarget("SparePartRecheckView", "機邊庫／複盤作業", "SparePartForm", "ASSET.SPARE_PART.VIEW", "Refresh"),
            new FunctionTarget("SparePartRecoveryTaskView", "機邊庫／我的回收作業", "SparePartForm", "ASSET.SPARE_PART.VIEW", "UserTransfer"),
            new FunctionTarget("SparePartInspectionView", "機邊庫／盤點批次", "SparePartForm", "ASSET.SPARE_PART.APPROVE", "SelectionChecked", true),
            new FunctionTarget("SparePartCostView", "機邊庫／成本計算", "SparePartForm", "ASSET.SPARE_PART.APPROVE", "Cost", true),
            new FunctionTarget("SparePartRecoveryView", "機邊庫／回收管理", "SparePartForm", "ASSET.SPARE_PART.APPROVE", "Schedule", true),
            new FunctionTarget("EmployeeManagementView", "系統管理／人員管理", "SystemSettingsForm", "SYSTEM.USER.VIEW", "ChangeUser"),
            new FunctionTarget("DepartmentManagementView", "系統管理／部門管理", "SystemSettingsForm", "SYSTEM.DEPARTMENT.VIEW", "Department"),
            new FunctionTarget("JobTitleManagementView", "系統管理／職稱管理", "SystemSettingsForm", "SYSTEM.JOB_TITLE.VIEW", "Promote"),
            new FunctionTarget("EmployeePermissionView", "系統管理／人員權限", "SystemSettingsForm", "SYSTEM.USER.PERMISSION.VIEW", "AddUserGroup"),
            new FunctionTarget("SecurityGroupManagementView", "系統管理／安全性群組", "SystemSettingsForm", "SYSTEM.GROUP.VIEW", "AddUserGroup"),
            new FunctionTarget("RolePermissionManagementView", "系統管理／角色與權限", "SystemSettingsForm", "SYSTEM.ROLE.VIEW", "SelectionChecked"),
            new FunctionTarget("AuditLogView", "系統管理／稽核記錄", "SystemSettingsForm", "SYSTEM.AUDIT.VIEW", "View"),
            new FunctionTarget("FunctionManagementView", "系統管理／功能卡管理", "SystemSettingsForm", "SYSTEM.FUNCTION.VIEW", "Equipment"),
            new FunctionTarget("SystemSettingManagementView", "系統管理／系統參數", "SystemSettingsForm", "SYSTEM.SETTING.VIEW", "Edit"),
            new FunctionTarget("SystemHealthView", "系統管理／系統狀態", "SystemSettingsForm", "SYSTEM.HEALTH.VIEW", "Info")
        });

        public static readonly IReadOnlyDictionary<string, string> Icons = new System.Collections.ObjectModel.ReadOnlyDictionary<string, string>(
            new Dictionary<string, string>
            {
                { "View", "檢視" }, { "Equipment", "設備" }, { "Department", "部門" }, { "Transfer", "進出庫" },
                { "Refresh", "重新整理" }, { "UserTransfer", "人員作業" }, { "SelectionChecked", "核取" },
                { "Cost", "成本" }, { "Schedule", "排程" }, { "ChangeUser", "人員" }, { "Promote", "階層" },
                { "AddUserGroup", "權限群組" }, { "Edit", "編輯" }, { "Info", "資訊" }, { "Add", "新增" },
                { "Attachment", "附件" }, { "Search", "查詢" }, { "Print", "列印" }, { "ExportExcel", "匯出試算表" },
                { "Upload", "上傳" }, { "Learn", "說明" }, { "Finished", "完成" }, { "ApprovalStamp", "核准" },
                { "Filter", "篩選" }, { "Copy", "複製" }, { "Delete", "刪除" }, { "Confirm", "確認" }, { "Bot", "自動化" }
            });

        public static FunctionTarget Find(string key) => Targets.FirstOrDefault(x => x.Key == key);
    }
}
