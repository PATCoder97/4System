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
    public sealed partial class JobTitleManagementView : XtraUserControl
    {
        private readonly JobTitleManagementService _service = new JobTitleManagementService(new ConnectionStringProvider());
        private readonly DXMenuItem _editItem;
        private readonly DXMenuItem _statusItem;
        public JobTitleManagementView()
        {
            InitializeComponent(); _editItem = CreateMenuItem("編輯職稱", EditSelected, SvgIconCatalog.Edit); _statusItem = CreateMenuItem("停用職稱", ChangeStatus, SvgIconCatalog.Disabled); btnAdd.Visibility = CurrentAuthorization.HasPermission("SYSTEM.JOB_TITLE.ADMIN") ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
        }
        private static DXMenuItem CreateMenuItem(string caption, EventHandler handler, DevExpress.Utils.Svg.SvgImage icon) { var item = new DXMenuItem(caption, handler, icon, DXMenuItemPriority.Normal); item.ImageOptions.SvgImageSize = new Size(24, 24); return item; }
        private void JobTitleManagementView_Load(object sender, EventArgs e) { LoadData(); }
        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) { LoadData(GetSelected()?.JobTitleId); }
        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) { OpenEditor(null); }
        private void gridViewJobTitles_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.Menu == null || e.MenuType != DevExpress.XtraGrid.Views.Grid.GridMenuType.Row || e.HitInfo.RowHandle < 0 || !CurrentAuthorization.HasPermission("SYSTEM.JOB_TITLE.ADMIN")) return;
            gridViewJobTitles.FocusedRowHandle = e.HitInfo.RowHandle; JobTitleListItem selected = GetSelected(); if (selected == null) return; _editItem.BeginGroup = e.Menu.Items.Count > 0; _statusItem.BeginGroup = true; _statusItem.Caption = selected.IsActive ? "停用職稱" : "啟用職稱"; _statusItem.ImageOptions.SvgImage = selected.IsActive ? SvgIconCatalog.Disabled : SvgIconCatalog.Confirm; e.Menu.Items.Add(_editItem); e.Menu.Items.Add(_statusItem);
        }
        private JobTitleListItem GetSelected() { return gridViewJobTitles.GetFocusedRow() as JobTitleListItem; }
        private void EditSelected(object sender, EventArgs e) { JobTitleListItem item = GetSelected(); if (item != null) OpenEditor(item); }
        private void OpenEditor(JobTitleListItem item)
        {
            using (var form = new JobTitleEditForm(item)) { if (form.ShowDialog(FindForm()) != DialogResult.OK) return; try { int id = _service.Save(form.Value); LoadData(id); } catch (Exception ex) { ShowError(ex.Message, "儲存職稱失敗"); } }
        }
        private void ChangeStatus(object sender, EventArgs e)
        {
            JobTitleListItem item = GetSelected(); if (item == null) return; bool enable = !item.IsActive; string action = enable ? "啟用" : "停用"; string detail = enable ? string.Empty : "\n若仍有使用此職稱的有效人員，系統將拒絕停用。"; if (XtraMessageBox.Show($"確定要{action}職稱「{item.JobTitleName}」嗎？{detail}", action + "職稱確認", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return; try { _service.SetActive(item.JobTitleId, enable, item.RowVersion); LoadData(item.JobTitleId); } catch (Exception ex) { ShowError(ex.Message, action + "職稱失敗"); }
        }
        private void LoadData(int? selectedId = null)
        {
            try { gridJobTitles.DataSource = _service.GetJobTitles().ToList(); if (selectedId.HasValue) { int handle = gridViewJobTitles.LocateByValue("JobTitleId", selectedId.Value); if (handle >= 0) gridViewJobTitles.FocusedRowHandle = handle; } } catch (Exception ex) { ShowError(ex.Message, "載入職稱失敗"); }
        }
        private static void ShowError(string message, string title) { XtraMessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }
}
