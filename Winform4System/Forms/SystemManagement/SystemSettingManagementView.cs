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
    public sealed partial class SystemSettingManagementView : XtraUserControl
    {
        private readonly SystemSettingService _service = new SystemSettingService(new ConnectionStringProvider());
        private readonly DXMenuItem _editItem;

        public SystemSettingManagementView()
        {
            InitializeComponent();
            _editItem = new DXMenuItem("編輯參數", EditSelected, SvgIconCatalog.Edit, DXMenuItemPriority.Normal);
            _editItem.ImageOptions.SvgImageSize = new Size(24, 24);
        }

        private void SystemSettingManagementView_Load(object sender, EventArgs e) { LoadData(); }
        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) { LoadData(GetSelected()?.SettingKey); }

        private void gridViewSettings_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.Menu == null || e.MenuType != DevExpress.XtraGrid.Views.Grid.GridMenuType.Row || e.HitInfo.RowHandle < 0
                || !CurrentAuthorization.HasPermission("SYSTEM.SETTING.ADMIN")) return;
            gridViewSettings.FocusedRowHandle = e.HitInfo.RowHandle;
            SystemSettingListItem selected = GetSelected();
            if (selected == null) return;
            _editItem.Enabled = selected.CanEdit;
            _editItem.BeginGroup = e.Menu.Items.Count > 0;
            e.Menu.Items.Add(_editItem);
        }

        private SystemSettingListItem GetSelected() { return gridViewSettings.GetFocusedRow() as SystemSettingListItem; }

        private void EditSelected(object sender, EventArgs e)
        {
            SystemSettingListItem selected = GetSelected();
            if (selected == null) return;
            using (var form = new SystemSettingEditForm(selected))
            {
                if (form.ShowDialog(FindForm()) != DialogResult.OK) return;
                try { _service.Save(form.Value); LoadData(selected.SettingKey); }
                catch (Exception ex) { ShowError(ex.Message, "儲存系統參數失敗"); }
            }
        }

        private void LoadData(string selectedKey = null)
        {
            try
            {
                gridSettings.DataSource = _service.GetSettings().ToList();
                if (!string.IsNullOrWhiteSpace(selectedKey))
                {
                    int handle = gridViewSettings.LocateByValue("SettingKey", selectedKey);
                    if (handle >= 0) gridViewSettings.FocusedRowHandle = handle;
                }
            }
            catch (Exception ex) { ShowError(ex.Message, "載入系統參數失敗"); }
        }

        private static void ShowError(string message, string title)
        {
            XtraMessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
