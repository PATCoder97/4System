using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Winform4System.Business.Services;

namespace Winform4System.Forms.SystemManagement
{
    public sealed partial class EmployeePermissionForm : XtraForm
    {
        public IReadOnlyList<int> SelectedGroupIds { get; private set; }
        public IReadOnlyList<int> OriginalGroupIds { get; private set; }
        public EmployeePermissionForm(EmployeeListItem employee, EmployeePermissionDetail detail)
        {
            InitializeComponent();
            Text = "人員權限 - " + (!string.IsNullOrWhiteSpace(employee.DisplayNameTW) ? employee.DisplayNameTW : employee.DisplayNameVN) + " (" + employee.UserId + ")";
            checkedGroups.DataSource = detail.Groups.ToList(); checkedGroups.DisplayMember = "GroupName"; checkedGroups.ValueMember = "GroupId";
            for (int index = 0; index < detail.Groups.Count; index++) checkedGroups.SetItemChecked(index, detail.Groups[index].IsAssigned);
            OriginalGroupIds = detail.Groups.Where(x => x.IsAssigned).Select(x => x.GroupId).ToList();
            memoRoles.Text = detail.Roles.Count == 0 ? "（無）" : string.Join(Environment.NewLine, detail.Roles);
            gridPermissions.DataSource = detail.Permissions.ToList();
        }
        private void btnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (XtraMessageBox.Show("確定要更新此人員的安全性群組嗎？", "權限變更確認", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            SelectedGroupIds = checkedGroups.CheckedItems.Cast<SecurityGroupOption>().Select(x => x.GroupId).ToList();
            DialogResult = DialogResult.OK;
            Close();
        }
        private void btnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) { DialogResult = DialogResult.Cancel; Close(); }
    }
}
