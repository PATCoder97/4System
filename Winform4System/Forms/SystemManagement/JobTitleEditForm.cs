using DevExpress.XtraEditors;
using System;
using System.Windows.Forms;
using Winform4System.Business.Services;

namespace Winform4System.Forms.SystemManagement
{
    public sealed partial class JobTitleEditForm : XtraForm
    {
        private readonly JobTitleListItem _source;
        public JobTitleEditModel Value { get; private set; }

        public JobTitleEditForm(JobTitleListItem source)
        {
            _source = source;
            InitializeComponent();
            if (source == null) { chkActive.Checked = true; Text = "新增職稱"; return; }
            txbCode.Text = source.JobTitleCode; txbCode.ReadOnly = true; txbName.Text = source.JobTitleName; spinSortOrder.Value = source.SortOrder; chkActive.Checked = source.IsActive; Text = "編輯職稱";
        }

        private void btnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Value = new JobTitleEditModel { JobTitleId = _source?.JobTitleId, JobTitleCode = txbCode.Text, JobTitleName = txbName.Text, SortOrder = Convert.ToInt32(spinSortOrder.Value), IsActive = chkActive.Checked, RowVersion = _source?.RowVersion };
            DialogResult = DialogResult.OK; Close();
        }

        private void btnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) { DialogResult = DialogResult.Cancel; Close(); }
    }
}
