using DevExpress.XtraEditors;
using System;
using System.Linq;
using System.Windows.Forms;
using Winform4System.Business.Services;

namespace Winform4System.Forms.SystemManagement
{
    public sealed partial class RolePermissionEditForm : XtraForm
    {
        private readonly RoleListItem _source;
        public RoleEditModel Value { get; private set; }
        public RolePermissionEditForm(RoleListItem source, RolePermissionDetail detail)
        {
            _source = source; InitializeComponent(); gridMatrix.DataSource = detail.Matrix.ToList(); gridGroups.DataSource = detail.Groups.ToList(); gridUsers.DataSource = detail.Users.ToList();
            tabMatrix.Text = "權限矩陣（已選 " + detail.Matrix.SelectMany(x => x.GetSelectedIds()).Distinct().Count() + "）";
            tabGroups.Text = "受影響群組（" + detail.Groups.Count + "）";
            tabUsers.Text = "受影響人員（" + detail.Users.Count + "）";
            if (source == null) { Text = "新增角色"; return; }
            Text = "編輯角色與權限"; txbCode.Text = source.RoleCode; txbCode.ReadOnly = true; txbName.Text = source.RoleName; memoDescription.Text = source.Description; lblSystemRole.Text = source.IsSystemRole ? "系統角色：是" : "系統角色：否";
        }
        private void gridViewMatrix_ShowingEditor(object sender, System.ComponentModel.CancelEventArgs e) { var row = gridViewMatrix.GetFocusedRow() as PermissionMatrixRow; if (row != null && !row.HasPermission(gridViewMatrix.FocusedColumn.FieldName)) e.Cancel = true; }
        private void btnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txbCode.Text) || string.IsNullOrWhiteSpace(txbName.Text)) { XtraMessageBox.Show("請輸入角色代碼及角色名稱。", "資料不完整", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            var rows = (gridMatrix.DataSource as System.Collections.Generic.IEnumerable<PermissionMatrixRow>)?.ToList() ?? new System.Collections.Generic.List<PermissionMatrixRow>();
            int affectedGroups = gridViewGroups.DataRowCount, affectedUsers = gridViewUsers.DataRowCount;
            string warning = $"確定要儲存角色及權限嗎？\n目前將影響 {affectedGroups} 個群組及 {affectedUsers} 位人員。";
            if (XtraMessageBox.Show(warning, "權限變更確認", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            Value = new RoleEditModel { RoleId = _source?.RoleId, RoleCode = txbCode.Text, RoleName = txbName.Text, Description = memoDescription.Text, PermissionIds = rows.SelectMany(x => x.GetSelectedIds()).Distinct().ToList(), RowVersion = _source?.RowVersion };
            DialogResult = DialogResult.OK; Close();
        }
        private void btnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) { DialogResult = DialogResult.Cancel; Close(); }
    }
}
