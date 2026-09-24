using DevExpress.Utils;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Winform4System.DataAccess.Entities;

namespace Winform4System.Forms.SystemManagement
{
    public sealed partial class FunctionEditForm : XtraForm
    {
        private const string NotStartedText = "尚未開始";
        private const string InProgressText = "開發中";
        private const string CompletedText = "已完成";

        private readonly ApplicationFunction _source;

        public ApplicationFunction Value { get; private set; }

        public FunctionEditForm(ApplicationFunction source, IEnumerable<ApplicationFunction> allFunctions)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            if (allFunctions == null)
                throw new ArgumentNullException(nameof(allFunctions));

            InitializeComponent();
            ConfigureEditors(allFunctions.ToList());
            LoadValues();
        }

        private void ConfigureEditors(List<ApplicationFunction> allFunctions)
        {
            var excludedIds = GetDescendantIds(_source.FunctionId, allFunctions);
            excludedIds.Add(_source.FunctionId);
            var parentOptions = allFunctions
                .Where(item => !excludedIds.Contains(item.FunctionId))
                .OrderBy(item => item.SortOrder)
                .ThenBy(item => item.DisplayName)
                .Select(item => new ParentOption
                {
                    FunctionId = item.FunctionId,
                    DisplayName = $"{item.DisplayName}  ({item.FunctionCode})"
                })
                .ToList();

            cbbParent.Properties.DataSource = parentOptions;
            cbbParent.Properties.DisplayMember = nameof(ParentOption.DisplayName);
            cbbParent.Properties.ValueMember = nameof(ParentOption.FunctionId);
            cbbParent.Properties.AllowNullInput = DefaultBoolean.True;
        }

        private void LoadValues()
        {
            txbCode.Text = _source.FunctionCode;
            txbDisplayName.Text = _source.DisplayName;
            memoDescription.Text = _source.Description;
            txbNavigationTarget.Text = _source.NavigationTarget;
            cbbParent.EditValue = _source.ParentFunctionId;
            speSortOrder.Value = _source.SortOrder;
            cbbDevelopmentStatus.SelectedItem = GetStatusDisplay(_source.DevelopmentStatus);
            chkWide.Checked = _source.IsWide;
            chkVisible.Checked = _source.IsVisible;
            chkActive.Checked = _source.IsActive;
        }

        private void btnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txbDisplayName.Text))
            {
                XtraMessageBox.Show(
                    "請輸入顯示名稱。",
                    "資料不完整",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txbDisplayName.Focus();
                return;
            }

            Value = new ApplicationFunction
            {
                FunctionId = _source.FunctionId,
                FunctionCode = _source.FunctionCode,
                ParentFunctionId = cbbParent.EditValue == null
                    ? (int?)null
                    : Convert.ToInt32(cbbParent.EditValue),
                DisplayName = txbDisplayName.Text.Trim(),
                Description = memoDescription.Text,
                NavigationTarget = txbNavigationTarget.Text,
                SortOrder = Convert.ToInt32(speSortOrder.Value),
                DevelopmentStatus = GetStatusCode(cbbDevelopmentStatus.Text),
                IsWide = chkWide.Checked,
                IsVisible = chkVisible.Checked,
                IsActive = chkActive.Checked
            };

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private static HashSet<int> GetDescendantIds(int functionId, List<ApplicationFunction> functions)
        {
            var result = new HashSet<int>();
            var pending = new Queue<int>();
            pending.Enqueue(functionId);

            while (pending.Count > 0)
            {
                int parentId = pending.Dequeue();
                foreach (int childId in functions
                    .Where(item => item.ParentFunctionId == parentId)
                    .Select(item => item.FunctionId))
                {
                    if (result.Add(childId))
                        pending.Enqueue(childId);
                }
            }

            return result;
        }

        private static string GetStatusDisplay(string status)
        {
            switch ((status ?? string.Empty).Trim().ToUpperInvariant())
            {
                case "COMPLETED":
                    return CompletedText;
                case "IN_PROGRESS":
                    return InProgressText;
                default:
                    return NotStartedText;
            }
        }

        private static string GetStatusCode(string display)
        {
            switch (display)
            {
                case CompletedText:
                    return "COMPLETED";
                case InProgressText:
                    return "IN_PROGRESS";
                default:
                    return "NOT_STARTED";
            }
        }

        private sealed class ParentOption
        {
            public int FunctionId { get; set; }
            public string DisplayName { get; set; }
        }
    }
}
