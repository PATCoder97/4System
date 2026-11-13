using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Winform4System.Business.Services;
using Winform4System.Core.Security;
using Winform4System.DataAccess.Configuration;
using Winform4System.DataAccess.Entities;
using Winform4System.Helpers;
using Winform4System.Core.Models;

namespace Winform4System.Forms.SystemManagement
{
    public sealed partial class FunctionManagementView : XtraUserControl
    {
        private readonly ApplicationFunctionService _service;
        private readonly DXMenuItem _itemEdit;
        private List<ApplicationFunction> _functions = new List<ApplicationFunction>();

        public FunctionManagementView()
        {
            InitializeComponent();
            _service = new ApplicationFunctionService(new ConnectionStringProvider());
            btnAdd.Visibility = CurrentAuthorization.HasPermission("SYSTEM.FUNCTION.ADMIN")
                ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
            _itemEdit = new DXMenuItem(
                "編輯功能卡",
                ItemEdit_Click,
                SvgIconCatalog.Edit,
                DXMenuItemPriority.Normal);
            _itemEdit.ImageOptions.SvgImageSize = new Size(24, 24);
            _itemEdit.AppearanceHovered.ForeColor = Color.Blue;
        }

        private void FunctionManagementView_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void ItemEdit_Click(object sender, EventArgs e)
        {
            EditSelected();
        }

        private void treeFunctions_PopupMenuShowing(
            object sender,
            DevExpress.XtraTreeList.PopupMenuShowingEventArgs e)
        {
            if (e.Menu == null ||
                e.MenuType != DevExpress.XtraTreeList.Menu.TreeListMenuType.Node ||
                e.HitInfo.Node == null ||
                !CurrentAuthorization.HasPermission("SYSTEM.FUNCTION.ADMIN"))
                return;

            treeFunctions.FocusedNode = e.HitInfo.Node;
            _itemEdit.BeginGroup = e.Menu.Items.Count > 0;
            e.Menu.Items.Add(_itemEdit);
        }

        private void LoadData(int? focusId = null)
        {
            try
            {
                if (!focusId.HasValue && treeFunctions.FocusedNode != null)
                    focusId = (treeFunctions.GetDataRecordByNode(treeFunctions.FocusedNode) as FunctionTreeRow)?.FunctionId;
                _functions = _service.GetList().ToList();
                treeFunctions.DataSource = _functions.Select(item => new FunctionTreeRow
                {
                    FunctionId = item.FunctionId,
                    ParentFunctionId = item.ParentFunctionId,
                    FunctionCode = item.FunctionCode,
                    DisplayName = item.DisplayName,
                    Description = item.Description,
                    IconCaption = !string.IsNullOrWhiteSpace(item.IconKey) && FunctionCatalog.Icons.ContainsKey(item.IconKey) ? FunctionCatalog.Icons[item.IconKey] : "預設圖示",
                    NavigationTarget = item.NavigationTarget,
                    SortOrder = item.SortOrder,
                    DevelopmentStatus = GetDevelopmentStatusDisplay(item.DevelopmentStatus),
                    IsWide = item.IsWide,
                    IsVisible = item.IsVisible,
                    IsActive = item.IsActive
                }).ToList();
                treeFunctions.ExpandAll();
                if (focusId.HasValue) treeFunctions.FocusedNode = treeFunctions.FindNodeByKeyID(focusId.Value);
                treeFunctions.BestFitColumns();
            }
            catch (Exception exception)
            {
                XtraMessageBox.Show(
                    exception.Message,
                    "載入功能卡失敗",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void EditSelected()
        {
            if (!CurrentAuthorization.HasPermission("SYSTEM.FUNCTION.ADMIN"))
            {
                XtraMessageBox.Show(
                    "您沒有管理功能卡的權限。",
                    "權限不足",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            var row = treeFunctions.FocusedNode == null
                ? null
                : treeFunctions.GetDataRecordByNode(treeFunctions.FocusedNode) as FunctionTreeRow;
            var selected = row == null
                ? null
                : _functions.FirstOrDefault(item => item.FunctionId == row.FunctionId);
            if (selected == null)
            {
                XtraMessageBox.Show(
                    "請先選擇需要編輯的功能卡。",
                    "功能卡管理",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            using (var form = new FunctionEditForm(selected, _functions, item => { _service.Update(item); return item.FunctionId; }))
            {
                if (form.ShowDialog(FindForm()) == DialogResult.OK) LoadData(form.SavedId);
            }
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!CurrentAuthorization.HasPermission("SYSTEM.FUNCTION.ADMIN")) return;
            var row = treeFunctions.FocusedNode == null ? null : treeFunctions.GetDataRecordByNode(treeFunctions.FocusedNode) as FunctionTreeRow;
            var selected = row == null ? null : _functions.FirstOrDefault(x => x.FunctionId == row.FunctionId);
            int? parentId = selected == null ? null : (string.IsNullOrWhiteSpace(selected.NavigationTarget)
                || FunctionCatalog.Find(selected.NavigationTarget)?.IsModule == true ? (int?)selected.FunctionId : selected.ParentFunctionId);
            using (var form = new FunctionEditForm(null, _functions, item => _service.Create(item), parentId))
            {
                if (form.ShowDialog(FindForm()) == DialogResult.OK)
                {
                    LoadData(form.SavedId);
                    XtraMessageBox.Show("功能已新增。請至角色與權限管理設定存取權限，並重新登入以更新權限。", "新增完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private static string GetDevelopmentStatusDisplay(string status)
        {
            switch ((status ?? string.Empty).Trim().ToUpperInvariant())
            {
                case "COMPLETED":
                    return "已完成";
                case "IN_PROGRESS":
                    return "開發中";
                default:
                    return "尚未開始";
            }
        }

        private sealed class FunctionTreeRow
        {
            public int FunctionId { get; set; }
            public int? ParentFunctionId { get; set; }
            public string FunctionCode { get; set; }
            public string DisplayName { get; set; }
            public string IconCaption { get; set; }
            public string Description { get; set; }
            public string NavigationTarget { get; set; }
            public int SortOrder { get; set; }
            public string DevelopmentStatus { get; set; }
            public bool IsWide { get; set; }
            public bool IsVisible { get; set; }
            public bool IsActive { get; set; }
        }
    }
}
