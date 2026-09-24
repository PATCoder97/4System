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
    public sealed partial class RolePermissionManagementView : XtraUserControl
    {
        private readonly RolePermissionManagementService _service = new RolePermissionManagementService(new ConnectionStringProvider());
        private readonly DXMenuItem _editItem, _deactivateItem;
        public RolePermissionManagementView()
        {
            InitializeComponent(); _editItem = Menu("編輯角色與權限", EditSelected, SvgIconCatalog.Edit); _deactivateItem = Menu("停用角色", DeactivateSelected, SvgIconCatalog.Disabled); btnAdd.Visibility = CurrentAuthorization.HasPermission("SYSTEM.ROLE.ADMIN") ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
        }
        private static DXMenuItem Menu(string caption, EventHandler handler, DevExpress.Utils.Svg.SvgImage icon) { var item = new DXMenuItem(caption, handler, icon, DXMenuItemPriority.Normal); item.ImageOptions.SvgImageSize = new Size(24, 24); return item; }
        private void RolePermissionManagementView_Load(object sender, EventArgs e) { LoadData(); }
        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) { LoadData(); }
        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) { OpenEditor(null); }
        private RoleListItem Selected() => gridViewRoles.GetFocusedRow() as RoleListItem;
        private void gridViewRoles_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e) { if (e.Menu == null || e.MenuType != DevExpress.XtraGrid.Views.Grid.GridMenuType.Row || e.HitInfo.RowHandle < 0) return; gridViewRoles.FocusedRowHandle = e.HitInfo.RowHandle; bool admin = CurrentAuthorization.HasPermission("SYSTEM.ROLE.ADMIN"); _editItem.BeginGroup = e.Menu.Items.Count > 0; _editItem.Enabled = admin; _deactivateItem.BeginGroup = true; _deactivateItem.Enabled = admin && Selected()?.IsActive == true; e.Menu.Items.Add(_editItem); e.Menu.Items.Add(_deactivateItem); }
        private void EditSelected(object sender, EventArgs e) { var item = Selected(); if (item != null) OpenEditor(item); }
        private void OpenEditor(RoleListItem item) { try { using (var form = new RolePermissionEditForm(item, _service.GetDetail(item?.RoleId))) { if (form.ShowDialog(FindForm()) != DialogResult.OK) return; int id = _service.Save(form.Value); LoadData(id); } } catch (Exception ex) { Error(ex.Message, "儲存角色與權限失敗"); } }
        private void DeactivateSelected(object sender, EventArgs e) { var item = Selected(); if (item == null) return; if (XtraMessageBox.Show($"確定要停用角色「{item.RoleName}」嗎？\n受影響的群組與人員將失去此角色的權限。", "停用確認", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return; try { _service.Deactivate(item.RoleId, item.RowVersion); LoadData(item.RoleId); } catch (Exception ex) { Error(ex.Message, "停用角色失敗"); } }
        private void LoadData(int? id = null) { try { gridRoles.DataSource = _service.GetRoles().ToList(); if (id.HasValue) { int handle = gridViewRoles.LocateByValue("RoleId", id.Value); if (handle >= 0) gridViewRoles.FocusedRowHandle = handle; } } catch (Exception ex) { Error(ex.Message, "載入角色失敗"); } }
        private static void Error(string message, string title) { XtraMessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }
}
