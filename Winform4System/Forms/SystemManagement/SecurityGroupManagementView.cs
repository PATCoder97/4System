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
    public sealed partial class SecurityGroupManagementView : XtraUserControl
    {
        private readonly SecurityGroupManagementService _service = new SecurityGroupManagementService(new ConnectionStringProvider());
        private readonly DXMenuItem _editItem;
        private readonly DXMenuItem _deactivateItem;

        public SecurityGroupManagementView()
        {
            InitializeComponent();
            _editItem = CreateMenuItem("編輯安全性群組", EditSelected, SvgIconCatalog.Edit);
            _deactivateItem = CreateMenuItem("停用安全性群組", DeactivateSelected, SvgIconCatalog.Disabled);
            btnAdd.Visibility = CurrentAuthorization.HasPermission("SYSTEM.GROUP.ADMIN") ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
        }

        private static DXMenuItem CreateMenuItem(string caption, EventHandler handler, DevExpress.Utils.Svg.SvgImage icon)
        {
            var item = new DXMenuItem(caption, handler, icon, DXMenuItemPriority.Normal);
            item.ImageOptions.SvgImageSize = new Size(24, 24);
            return item;
        }

        private void SecurityGroupManagementView_Load(object sender, EventArgs e) { LoadData(); }
        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) { LoadData(); }
        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) { OpenEditor(null); }
        private SecurityGroupListItem GetSelected() => gridViewGroups.GetFocusedRow() as SecurityGroupListItem;

        private void gridViewGroups_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.Menu == null || e.MenuType != DevExpress.XtraGrid.Views.Grid.GridMenuType.Row || e.HitInfo.RowHandle < 0) return;
            gridViewGroups.FocusedRowHandle = e.HitInfo.RowHandle;
            bool canAdmin = CurrentAuthorization.HasPermission("SYSTEM.GROUP.ADMIN");
            _editItem.BeginGroup = e.Menu.Items.Count > 0;
            _editItem.Enabled = canAdmin;
            _deactivateItem.BeginGroup = true;
            _deactivateItem.Enabled = canAdmin && GetSelected()?.IsActive == true;
            e.Menu.Items.Add(_editItem);
            e.Menu.Items.Add(_deactivateItem);
        }

        private void EditSelected(object sender, EventArgs e) { var item = GetSelected(); if (item != null) OpenEditor(item); }
        private void OpenEditor(SecurityGroupListItem item)
        {
            try
            {
                using (var form = new SecurityGroupEditForm(item, _service.GetDetail(item?.GroupId)))
                {
                    if (form.ShowDialog(FindForm()) != DialogResult.OK) return;
                    int groupId = _service.Save(form.Value);
                    LoadData(groupId);
                }
            }
            catch (Exception ex) { ShowError(ex.Message, "儲存安全性群組失敗"); }
        }

        private void DeactivateSelected(object sender, EventArgs e)
        {
            var item = GetSelected(); if (item == null) return;
            if (XtraMessageBox.Show($"確定要停用安全性群組「{item.GroupName}」嗎？\n群組成員將失去由此群組取得的權限。", "停用確認", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            try { _service.Deactivate(item.GroupId, item.RowVersion); LoadData(item.GroupId); }
            catch (Exception ex) { ShowError(ex.Message, "停用安全性群組失敗"); }
        }

        private void LoadData(int? selectedId = null)
        {
            try
            {
                gridGroups.DataSource = _service.GetGroups().ToList();
                if (selectedId.HasValue) { int handle = gridViewGroups.LocateByValue("GroupId", selectedId.Value); if (handle >= 0) gridViewGroups.FocusedRowHandle = handle; }
            }
            catch (Exception ex) { ShowError(ex.Message, "載入安全性群組失敗"); }
        }

        private static void ShowError(string message, string title) { XtraMessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }
}
