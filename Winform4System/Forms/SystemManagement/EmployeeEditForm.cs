using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Winform4System.Business.Services;

namespace Winform4System.Forms.SystemManagement
{
    public sealed partial class EmployeeEditForm : XtraForm
    {
        private readonly EmployeeListItem _source;
        public EmployeeEditModel Value { get; private set; }

        public EmployeeEditForm(EmployeeListItem source, IEnumerable<DepartmentOption> departments, IEnumerable<JobTitleOption> jobTitles)
        {
            _source = source;
            InitializeComponent();
            cbbDepartment.Properties.DataSource = departments.ToList(); cbbDepartment.Properties.DisplayMember = "DisplayName"; cbbDepartment.Properties.ValueMember = "DepartmentId";
            cbbJobTitle.Properties.DataSource = jobTitles.ToList(); cbbJobTitle.Properties.DisplayMember = "DisplayName"; cbbJobTitle.Properties.ValueMember = "JobTitleId";
            cbbEmploymentStatus.Properties.Items.AddRange(new object[] { "停職", "在職", "留職停薪", "離職" });
            LoadValues();
        }

        private void LoadValues()
        {
            if (_source == null) { cbbEmploymentStatus.SelectedIndex = 1; chkAccountActive.Checked = true; Text = "新增人員"; return; }
            txbUserId.Text = _source.UserId; txbUserId.ReadOnly = true; txbDisplayNameTW.Text = _source.DisplayNameTW; txbDisplayNameVN.Text = _source.DisplayNameVN; cbbDepartment.EditValue = _source.DepartmentId; cbbJobTitle.EditValue = _source.JobTitleId; txbEmail.Text = _source.WorkEmail; txbPhone.Text = _source.WorkPhone; dateHire.EditValue = _source.HireDate; cbbEmploymentStatus.SelectedIndex = _source.EmploymentStatus; chkAccountActive.Checked = _source.IsAccountActive; Text = "編輯人員";
        }

        private void btnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) { DialogResult = DialogResult.Cancel; Close(); }
        private void btnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                Value = new EmployeeEditModel { EmployeeProfileId = _source?.EmployeeProfileId, UserId = txbUserId.Text, DisplayNameTW = txbDisplayNameTW.Text, DisplayNameVN = txbDisplayNameVN.Text, DepartmentId = cbbDepartment.EditValue == null ? 0 : Convert.ToInt32(cbbDepartment.EditValue), JobTitleId = cbbJobTitle.EditValue == null ? (int?)null : Convert.ToInt32(cbbJobTitle.EditValue), WorkEmail = txbEmail.Text, WorkPhone = txbPhone.Text, HireDate = dateHire.EditValue as DateTime?, EmploymentStatus = (byte)Math.Max(0, cbbEmploymentStatus.SelectedIndex), IsAccountActive = chkAccountActive.Checked, EmployeeRowVersion = _source?.EmployeeRowVersion, AccountRowVersion = _source?.AccountRowVersion };
                if (string.IsNullOrWhiteSpace(Value.UserId) || string.IsNullOrWhiteSpace(Value.DisplayNameVN) || Value.DepartmentId <= 0) throw new InvalidOperationException("請填寫人員編號、越文姓名及部門。");
                DialogResult = DialogResult.OK; Close();
            }
            catch (Exception ex) { XtraMessageBox.Show(ex.Message, "資料不完整", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }
    }
}
