using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Winform4System.Business.Services;
using Winform4System.Core.Security;
using Winform4System.DataAccess.Configuration;
using Winform4System.Helpers;

namespace Winform4System.Forms.SystemManagement
{
    public sealed partial class EmployeeManagementView : XtraUserControl
    {
        private readonly EmployeeManagementService _service = new EmployeeManagementService(new ConnectionStringProvider());
        private readonly DXMenuItem _editItem;
        private readonly DXMenuItem _deactivateItem;

        public EmployeeManagementView()
        {
            InitializeComponent();
            _editItem = CreateMenuItem("編輯人員", EditSelected, SvgIconCatalog.Edit);
            _deactivateItem = CreateMenuItem("停用人員", DeactivateSelected, SvgIconCatalog.SuspendUser);
            btnAdd.Visibility = CurrentAuthorization.HasPermission("SYSTEM.USER.ADMIN") ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
        }

        private static DXMenuItem CreateMenuItem(string caption, EventHandler handler, DevExpress.Utils.Svg.SvgImage image)
        {
            var item = new DXMenuItem(caption, handler, image, DXMenuItemPriority.Normal);
            item.ImageOptions.SvgImageSize = new Size(24, 24);
            return item;
        }

        private void EmployeeManagementView_Load(object sender, EventArgs e) { LoadData(); }
        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) { LoadData(); }
        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) { OpenEditor(null); }

        private void gridViewEmployees_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.Menu == null || e.MenuType != DevExpress.XtraGrid.Views.Grid.GridMenuType.Row || e.HitInfo.RowHandle < 0 || !CurrentAuthorization.HasPermission("SYSTEM.USER.ADMIN")) return;
            gridViewEmployees.FocusedRowHandle = e.HitInfo.RowHandle;
            _editItem.BeginGroup = e.Menu.Items.Count > 0;
            _deactivateItem.BeginGroup = true;
            _deactivateItem.Enabled = GetSelected()?.IsAccountActive == true;
            e.Menu.Items.Add(_editItem);
            e.Menu.Items.Add(_deactivateItem);
        }

        private EmployeeListItem GetSelected() => gridViewEmployees.GetFocusedRow() as EmployeeListItem;
        private void EditSelected(object sender, EventArgs e) { var item = GetSelected(); if (item != null) OpenEditor(item); }

        private void OpenEditor(EmployeeListItem item)
        {
            using (var form = new EmployeeEditForm(item, _service.GetDepartments()))
            {
                if (form.ShowDialog(FindForm()) != DialogResult.OK) return;
                try { _service.Save(form.Value); LoadData(form.Value.UserId); }
                catch (Exception ex) { ShowError(ex.Message, "儲存人員失敗"); }
            }
        }

        private void DeactivateSelected(object sender, EventArgs e)
        {
            var item = GetSelected();
            if (item == null) return;
            if (XtraMessageBox.Show($"確定要停用人員「{GetName(item)}」及其登入帳號嗎？", "停用確認", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            try { _service.Deactivate(item.UserId); LoadData(item.UserId); }
            catch (Exception ex) { ShowError(ex.Message, "停用人員失敗"); }
        }

        private void LoadData(string selectedUserId = null)
        {
            try
            {
                gridEmployees.DataSource = _service.GetEmployees().ToList();
                if (!string.IsNullOrWhiteSpace(selectedUserId))
                {
                    int handle = gridViewEmployees.LocateByValue("UserId", selectedUserId);
                    if (handle >= 0) gridViewEmployees.FocusedRowHandle = handle;
                }
            }
            catch (Exception ex) { ShowError(ex.Message, "載入人員失敗"); }
        }

        private static string GetName(EmployeeListItem item) => !string.IsNullOrWhiteSpace(item.DisplayNameTW) ? item.DisplayNameTW : !string.IsNullOrWhiteSpace(item.DisplayNameVN) ? item.DisplayNameVN : item.UserId;
        private static void ShowError(string message, string title) { XtraMessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }
}
