using DevExpress.XtraEditors;
using System;
using System.Linq;
using System.Windows.Forms;
using Winform4System.Business.Services;

namespace Winform4System.Forms.SystemManagement
{
    public sealed partial class SecurityGroupEditForm : XtraForm
    {
        private readonly SecurityGroupListItem _source;
        public SecurityGroupEditModel Value { get; private set; }

        public SecurityGroupEditForm(SecurityGroupListItem source, SecurityGroupDetail detail)
        {
            _source = source;
            InitializeComponent();
            checkedRoles.DataSource = detail.Roles.ToList(); checkedRoles.DisplayMember = "RoleName"; checkedRoles.ValueMember = "RoleId";
            for (int i = 0; i < detail.Roles.Count; i++) checkedRoles.SetItemChecked(i, detail.Roles[i].IsAssigned);
            gridMembers.DataSource = detail.Members.ToList();
            if (source == null) { Text = "新增安全性群組"; return; }
            Text = "編輯安全性群組"; txbCode.Text = source.GroupCode; txbCode.ReadOnly = true; txbName.Text = source.GroupName; memoDescription.Text = source.Description; lblSystemGroup.Text = source.IsSystemGroup ? "系統群組：是" : "系統群組：否";
        }

        private void btnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txbCode.Text) || string.IsNullOrWhiteSpace(txbName.Text)) { XtraMessageBox.Show("請輸入群組代碼及群組名稱。", "資料不完整", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (XtraMessageBox.Show("確定要儲存安全性群組及其角色設定嗎？", "儲存確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            Value = new SecurityGroupEditModel { GroupId = _source?.GroupId, GroupCode = txbCode.Text, GroupName = txbName.Text, Description = memoDescription.Text, RoleIds = checkedRoles.CheckedItems.Cast<SecurityGroupRoleOption>().Select(x => x.RoleId).ToList(), RowVersion = _source?.RowVersion };
            DialogResult = DialogResult.OK; Close();
        }
        private void btnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) { DialogResult = DialogResult.Cancel; Close(); }
    }
}
