using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Winform4System.Business.Services;
using Winform4System.DataAccess.Configuration;

namespace Winform4System.Forms.SystemManagement
{
    public sealed partial class SystemHealthView : XtraUserControl
    {
        private readonly SystemHealthService _service = new SystemHealthService(new ConnectionStringProvider());
        public SystemHealthView() { InitializeComponent(); }
        private void SystemHealthView_Load(object sender, EventArgs e) { LoadData(); }
        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) { LoadData(); }
        private void LoadData()
        {
            try
            {
                Version version = Assembly.GetEntryAssembly()?.GetName().Version;
                gridHealth.DataSource = _service.Check(version?.ToString() ?? string.Empty).ToList();
            }
            catch (Exception ex) { XtraMessageBox.Show(ex.Message, "檢查系統狀態失敗", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
        private void gridViewHealth_RowStyle(object sender, RowStyleEventArgs e)
        {
            if (e.RowHandle < 0) return;
            var item = gridViewHealth.GetRow(e.RowHandle) as SystemHealthItem;
            if (item == null) return;
            if (item.State == HealthState.Error) e.Appearance.ForeColor = Color.Firebrick;
            else if (item.State == HealthState.Warning) e.Appearance.ForeColor = Color.DarkOrange;
            else if (item.State == HealthState.Healthy) e.Appearance.ForeColor = Color.DarkGreen;
        }
    }
}
