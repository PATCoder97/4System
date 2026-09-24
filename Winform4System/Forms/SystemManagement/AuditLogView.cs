using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Winform4System.Business.Services;
using Winform4System.Core.Security;
using Winform4System.DataAccess.Configuration;
using Winform4System.Helpers;

namespace Winform4System.Forms.SystemManagement
{
    public sealed partial class AuditLogView : XtraUserControl
    {
        private readonly AuditLogService _service = new AuditLogService(new ConnectionStringProvider());
        private readonly DXMenuItem _viewDetailItem;

        public AuditLogView()
        {
            InitializeComponent();
            _viewDetailItem = new DXMenuItem("檢視詳細資料", ViewSelected, SvgIconCatalog.View, DXMenuItemPriority.Normal);
            _viewDetailItem.ImageOptions.SvgImageSize = new Size(24, 24);
            btnExport.Visibility = CurrentAuthorization.HasPermission("SYSTEM.AUDIT.EXPORT")
                ? DevExpress.XtraBars.BarItemVisibility.Always
                : DevExpress.XtraBars.BarItemVisibility.Never;
        }

        private void AuditLogView_Load(object sender, EventArgs e)
        {
            dateFrom.EditValue = DateTime.Today.AddDays(-6);
            dateTo.EditValue = DateTime.Today;
            try
            {
                var options = _service.GetFilterOptions();
                repositoryAction.Items.AddRange(options.ActionCodes.Cast<object>().ToArray());
                repositoryEntity.Items.AddRange(options.EntityNames.Cast<object>().ToArray());
                LoadData();
            }
            catch (Exception ex) { ShowError(ex.Message, "載入稽核記錄失敗"); }
        }

        private void btnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) { LoadData(); }
        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) { ClearFilters(); LoadData(); }

        private void ClearFilters()
        {
            dateFrom.EditValue = DateTime.Today.AddDays(-6);
            dateTo.EditValue = DateTime.Today;
            userKeyword.EditValue = null;
            actionCode.EditValue = null;
            entityName.EditValue = null;
            keyword.EditValue = null;
        }

        private void LoadData()
        {
            try
            {
                DateTime from = GetDate(dateFrom, "請選擇開始日期。").Date;
                DateTime to = GetDate(dateTo, "請選擇結束日期。").Date;
                var result = _service.Search(new AuditLogFilter
                {
                    FromUtc = from.ToUniversalTime(),
                    ToUtc = to.AddDays(1).ToUniversalTime(),
                    UserKeyword = Convert.ToString(userKeyword.EditValue),
                    ActionCode = Convert.ToString(actionCode.EditValue),
                    EntityName = Convert.ToString(entityName.EditValue),
                    Keyword = Convert.ToString(keyword.EditValue)
                });
                gridAudit.DataSource = result.Items.ToList();
                lblResult.Text = result.IsTruncated
                    ? $"顯示前 {AuditLogService.MaximumRows:N0} 筆記錄，請縮小查詢範圍。"
                    : $"共 {result.Items.Count:N0} 筆記錄";
                lblResult.Appearance.ForeColor = result.IsTruncated ? Color.Firebrick : Color.Black;
            }
            catch (Exception ex) { ShowError(ex.Message, "查詢稽核記錄失敗"); }
        }

        private static DateTime GetDate(DevExpress.XtraBars.BarEditItem item, string message)
        {
            if (item.EditValue == null || item.EditValue == DBNull.Value) throw new InvalidOperationException(message);
            return Convert.ToDateTime(item.EditValue);
        }

        private AuditLogListItem GetSelected() => gridViewAudit.GetFocusedRow() as AuditLogListItem;

        private void gridViewAudit_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.Menu == null || e.MenuType != DevExpress.XtraGrid.Views.Grid.GridMenuType.Row || e.HitInfo.RowHandle < 0) return;
            gridViewAudit.FocusedRowHandle = e.HitInfo.RowHandle;
            _viewDetailItem.BeginGroup = e.Menu.Items.Count > 0;
            e.Menu.Items.Add(_viewDetailItem);
        }

        private void gridViewAudit_DoubleClick(object sender, EventArgs e) { ViewSelected(sender, e); }

        private void ViewSelected(object sender, EventArgs e)
        {
            var item = GetSelected();
            if (item == null) return;
            using (var form = new AuditLogDetailForm(item)) form.ShowDialog(FindForm());
        }

        private void btnExport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (gridViewAudit.RowCount == 0)
            {
                XtraMessageBox.Show("目前沒有可匯出的記錄。", "匯出稽核記錄", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var dialog = new SaveFileDialog
            {
                Filter = "Excel 活頁簿 (*.xlsx)|*.xlsx",
                FileName = "稽核記錄_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xlsx",
                AddExtension = true,
                DefaultExt = "xlsx"
            })
            {
                if (dialog.ShowDialog(FindForm()) != DialogResult.OK) return;
                try
                {
                    gridAudit.ExportToXlsx(dialog.FileName);
                    _service.RecordExport(gridViewAudit.RowCount);
                    if (XtraMessageBox.Show("匯出完成，是否立即開啟檔案？", "匯出稽核記錄", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                        Process.Start(dialog.FileName);
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message, "匯出稽核記錄失敗");
                }
            }
        }

        private static void ShowError(string message, string title) { XtraMessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }
}
