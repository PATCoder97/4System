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
    public sealed partial class EmployeePermissionView : XtraUserControl
    {
        private readonly EmployeePermissionService _service = new EmployeePermissionService(new ConnectionStringProvider());
        private readonly DXMenuItem _permissionItem;
        public EmployeePermissionView()
        {
            InitializeComponent();
            _permissionItem = new DXMenuItem("設定人員權限", OpenPermission, SvgIconCatalog.AddUserGroup, DXMenuItemPriority.Normal);
            _permissionItem.ImageOptions.SvgImageSize = new Size(24, 24);
        }
        private void EmployeePermissionView_Load(object sender, EventArgs e) { LoadData(); }
        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) { LoadData(); }
        private void gridViewEmployees_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.Menu == null || e.MenuType != DevExpress.XtraGrid.Views.Grid.GridMenuType.Row || e.HitInfo.RowHandle < 0) return;
            gridViewEmployees.FocusedRowHandle = e.HitInfo.RowHandle; _permissionItem.BeginGroup = e.Menu.Items.Count > 0; _permissionItem.Enabled = CurrentAuthorization.HasPermission("SYSTEM.USER.PERMISSION.ADMIN"); e.Menu.Items.Add(_permissionItem);
        }
        private void OpenPermission(object sender, EventArgs e)
        {
            var item = gridViewEmployees.GetFocusedRow() as EmployeeListItem; if (item == null) return;
            try
            {
                using (var form = new EmployeePermissionForm(item, _service.GetDetail(item.UserId)))
                {
                    if (form.ShowDialog(FindForm()) != DialogResult.OK) return;
                    _service.SaveGroups(item.UserId, form.SelectedGroupIds, form.OriginalGroupIds);
                    XtraMessageBox.Show("人員權限已更新。重新登入後將套用新權限。", "儲存成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex) { XtraMessageBox.Show(ex.Message, "設定人員權限失敗", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
        private void LoadData() { try { gridEmployees.DataSource = _service.GetEmployees().ToList(); } catch (Exception ex) { XtraMessageBox.Show(ex.Message, "載入人員權限失敗", MessageBoxButtons.OK, MessageBoxIcon.Error); } }
    }
}
