using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Winform4System.Business.Services;

namespace Winform4System.Forms.SystemManagement
{
    public sealed partial class EmployeeDepartmentTransferForm : XtraForm
    {
        private readonly DepartmentListItem _source;
        private readonly List<EmployeeDepartmentTransferItem> _employees;

        public int TargetDepartmentId { get; private set; }

        public EmployeeDepartmentTransferForm(
            DepartmentListItem source,
            IEnumerable<DepartmentListItem> departments,
            IEnumerable<EmployeeDepartmentTransferItem> employees)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _employees = (employees ?? Enumerable.Empty<EmployeeDepartmentTransferItem>()).ToList();
            InitializeComponent();

            List<DepartmentListItem> targets = departments.Where(x => x.IsActive && x.DepartmentId != source.DepartmentId)
                .OrderBy(x => x.SortOrder).ThenBy(x => x.DepartmentCode).ToList();
            cbbTargetDepartment.Properties.DataSource = targets;
            cbbTargetDepartment.Properties.DisplayMember = "DisplayName";
            cbbTargetDepartment.Properties.ValueMember = "DepartmentId";
            gridEmployees.DataSource = _employees;
            lblInformation.Text = $"來源部門：{source.DisplayName}\r\n下列 {_employees.Count} 位在職或留職停薪人員將一併轉移。確認前請檢查影響名單。";
            Text = "轉移部門人員";
        }

        private void btnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (_employees.Count == 0)
            {
                XtraMessageBox.Show("來源部門沒有可轉移的有效人員。", "無人員可轉移", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (cbbTargetDepartment.EditValue == null)
            {
                XtraMessageBox.Show("請選擇目標部門。", "資料不完整", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DepartmentListItem target = cbbTargetDepartment.GetSelectedDataRow() as DepartmentListItem;
            if (target == null) return;
            if (XtraMessageBox.Show(
                $"確定要將 {_employees.Count} 位人員從「{_source.DepartmentName}」轉移至「{target.DepartmentName}」嗎？",
                "轉移確認",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) != DialogResult.Yes) return;

            TargetDepartmentId = target.DepartmentId;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
