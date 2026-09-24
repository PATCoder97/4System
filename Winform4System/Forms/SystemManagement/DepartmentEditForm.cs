using DevExpress.Utils;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Winform4System.Business.Services;

namespace Winform4System.Forms.SystemManagement
{
    public sealed partial class DepartmentEditForm : XtraForm
    {
        private readonly DepartmentListItem _source;

        public DepartmentEditModel Value { get; private set; }

        public DepartmentEditForm(DepartmentListItem source, IEnumerable<DepartmentListItem> departments)
        {
            _source = source;
            InitializeComponent();
            ConfigureParents((departments ?? Enumerable.Empty<DepartmentListItem>()).ToList());
            LoadValues();
        }

        private void ConfigureParents(List<DepartmentListItem> departments)
        {
            var excluded = new HashSet<int>();
            if (_source != null)
            {
                excluded.Add(_source.DepartmentId);
                var pending = new Queue<int>();
                pending.Enqueue(_source.DepartmentId);
                while (pending.Count > 0)
                {
                    int parentId = pending.Dequeue();
                    foreach (int childId in departments.Where(x => x.ParentDepartmentId == parentId).Select(x => x.DepartmentId))
                        if (excluded.Add(childId)) pending.Enqueue(childId);
                }
            }
            cbbParent.Properties.DataSource = departments.Where(x => x.IsActive && !excluded.Contains(x.DepartmentId)).OrderBy(x => x.SortOrder).ThenBy(x => x.DepartmentCode).ToList();
            cbbParent.Properties.DisplayMember = nameof(DepartmentListItem.DisplayName);
            cbbParent.Properties.ValueMember = nameof(DepartmentListItem.DepartmentId);
            cbbParent.Properties.AllowNullInput = DefaultBoolean.True;
        }

        private void LoadValues()
        {
            if (_source == null)
            {
                chkActive.Checked = true;
                Text = "新增部門";
                return;
            }
            txbCode.Text = _source.DepartmentCode;
            txbCode.ReadOnly = true;
            txbName.Text = _source.DepartmentName;
            cbbParent.EditValue = _source.ParentDepartmentId;
            spinSortOrder.Value = _source.SortOrder;
            chkActive.Checked = _source.IsActive;
            Text = "編輯部門";
        }

        private void btnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Value = new DepartmentEditModel
            {
                DepartmentId = _source?.DepartmentId,
                DepartmentCode = txbCode.Text,
                DepartmentName = txbName.Text,
                ParentDepartmentId = cbbParent.EditValue == null ? (int?)null : Convert.ToInt32(cbbParent.EditValue),
                SortOrder = Convert.ToInt32(spinSortOrder.Value),
                IsActive = chkActive.Checked,
                RowVersion = _source?.RowVersion
            };
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
