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
using Winform4System.Helpers;

namespace Winform4System.Forms.SystemManagement
{
    public sealed partial class DepartmentManagementView : XtraUserControl
    {
        private readonly DepartmentManagementService _service = new DepartmentManagementService(new ConnectionStringProvider());
        private readonly DXMenuItem _editItem;
        private readonly DXMenuItem _transferItem;
        private readonly DXMenuItem _statusItem;
        private List<DepartmentListItem> _departments = new List<DepartmentListItem>();

        public DepartmentManagementView()
        {
            InitializeComponent();
            _editItem = CreateMenuItem("編輯部門", EditSelected, SvgIconCatalog.Edit);
            _transferItem = CreateMenuItem("轉移有效人員", TransferEmployees, SvgIconCatalog.UserTransfer);
            _statusItem = CreateMenuItem("停用部門", ChangeStatus, SvgIconCatalog.Disabled);
            btnAdd.Visibility = CurrentAuthorization.HasPermission("SYSTEM.DEPARTMENT.ADMIN") ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
        }

        private static DXMenuItem CreateMenuItem(string caption, EventHandler handler, DevExpress.Utils.Svg.SvgImage icon)
        {
            var item = new DXMenuItem(caption, handler, icon, DXMenuItemPriority.Normal);
            item.ImageOptions.SvgImageSize = new Size(24, 24);
            return item;
        }

        private void DepartmentManagementView_Load(object sender, EventArgs e) { LoadData(); }
        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) { LoadData(GetSelected()?.DepartmentId); }
        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) { OpenEditor(null); }

        private void treeDepartments_PopupMenuShowing(object sender, DevExpress.XtraTreeList.PopupMenuShowingEventArgs e)
        {
            if (e.Menu == null || e.MenuType != DevExpress.XtraTreeList.Menu.TreeListMenuType.Node || e.HitInfo.Node == null || !CurrentAuthorization.HasPermission("SYSTEM.DEPARTMENT.ADMIN")) return;
            treeDepartments.FocusedNode = e.HitInfo.Node;
            DepartmentListItem selected = GetSelected();
            if (selected == null) return;
            _editItem.BeginGroup = e.Menu.Items.Count > 0;
            _transferItem.BeginGroup = true;
            _transferItem.Enabled = selected.EmployeeCount > 0 && _departments.Any(x => x.IsActive && x.DepartmentId != selected.DepartmentId);
            _statusItem.BeginGroup = true;
            _statusItem.Caption = selected.IsActive ? "停用部門" : "啟用部門";
            _statusItem.ImageOptions.SvgImage = selected.IsActive ? SvgIconCatalog.Disabled : SvgIconCatalog.Confirm;
            e.Menu.Items.Add(_editItem);
            e.Menu.Items.Add(_transferItem);
            e.Menu.Items.Add(_statusItem);
        }

        private DepartmentListItem GetSelected()
        {
            return treeDepartments.FocusedNode == null ? null : treeDepartments.GetDataRecordByNode(treeDepartments.FocusedNode) as DepartmentListItem;
        }

        private void EditSelected(object sender, EventArgs e)
        {
            DepartmentListItem selected = GetSelected();
            if (selected != null) OpenEditor(selected);
        }

        private void OpenEditor(DepartmentListItem selected)
        {
            using (var form = new DepartmentEditForm(selected, _departments))
            {
                if (form.ShowDialog(FindForm()) != DialogResult.OK) return;
                try
                {
                    int savedId = _service.Save(form.Value);
                    LoadData(savedId);
                }
                catch (Exception ex) { ShowError(ex.Message, "儲存部門失敗"); }
            }
        }

        private void ChangeStatus(object sender, EventArgs e)
        {
            DepartmentListItem selected = GetSelected();
            if (selected == null) return;
            bool enable = !selected.IsActive;
            string action = enable ? "啟用" : "停用";
            string detail = enable ? string.Empty : "\n若仍有在職人員或啟用中的下層部門，系統將拒絕停用。";
            if (XtraMessageBox.Show($"確定要{action}部門「{selected.DepartmentName}」嗎？{detail}", action + "部門確認", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            try { _service.SetActive(selected.DepartmentId, enable, selected.RowVersion); LoadData(selected.DepartmentId); }
            catch (Exception ex) { ShowError(ex.Message, action + "部門失敗"); }
        }

        private void TransferEmployees(object sender, EventArgs e)
        {
            DepartmentListItem source = GetSelected();
            if (source == null) return;
            try
            {
                var candidates = _service.GetTransferCandidates(source.DepartmentId);
                using (var form = new EmployeeDepartmentTransferForm(source, _departments, candidates))
                {
                    if (form.ShowDialog(FindForm()) != DialogResult.OK) return;
                    int transferred = _service.TransferEmployees(source.DepartmentId, form.TargetDepartmentId, candidates);
                    XtraMessageBox.Show($"已成功轉移 {transferred} 位人員。", "轉移完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData(source.DepartmentId);
                }
            }
            catch (Exception ex) { ShowError(ex.Message, "轉移人員失敗"); }
        }

        private void LoadData(int? selectedId = null)
        {
            try
            {
                _departments = _service.GetDepartments().ToList();
                treeDepartments.DataSource = _departments;
                treeDepartments.ExpandAll();
                if (selectedId.HasValue)
                {
                    DevExpress.XtraTreeList.Nodes.TreeListNode node = treeDepartments.FindNodeByKeyID(selectedId.Value);
                    if (node != null) treeDepartments.FocusedNode = node;
                }
            }
            catch (Exception ex) { ShowError(ex.Message, "載入部門失敗"); }
        }

        private static void ShowError(string message, string title) { XtraMessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }
}
