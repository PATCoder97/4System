using DevExpress.Utils;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Winform4System.DataAccess.Entities;
using Winform4System.Core.Models;
using Winform4System.Helpers;

namespace Winform4System.Forms.SystemManagement
{
    public sealed partial class FunctionEditForm : XtraForm
    {
        private const string NotStartedText = "尚未開始";
        private const string InProgressText = "開發中";
        private const string CompletedText = "已完成";

        private readonly ApplicationFunction _source;
        private readonly Func<ApplicationFunction, int> save;
        private List<ApplicationFunction> functions;
        public int SavedId { get; private set; }

        public ApplicationFunction Value { get; private set; }

        public FunctionEditForm(ApplicationFunction source, IEnumerable<ApplicationFunction> allFunctions, Func<ApplicationFunction, int> saveAction = null, int? parentId = null)
        {
            _source = source ?? new ApplicationFunction { ParentFunctionId = parentId, DevelopmentStatus = "NOT_STARTED", IsVisible = true, IsActive = true };
            save = saveAction;
            if (allFunctions == null)
                throw new ArgumentNullException(nameof(allFunctions));

            InitializeComponent();
            functions = allFunctions.ToList();
            if (_source.FunctionId == 0)
                _source.SortOrder = functions.Where(x => x.ParentFunctionId == parentId).Select(x => x.SortOrder).DefaultIfEmpty(0).Max() + 10;
            Text = _source.FunctionId == 0 ? "新增功能" : "編輯功能";
            txbCode.Properties.ReadOnly = _source.FunctionId > 0;
            ConfigureEditors(functions);
            LoadValues();
        }

        private void ConfigureEditors(List<ApplicationFunction> allFunctions)
        {
            var excludedIds = GetDescendantIds(_source.FunctionId, allFunctions);
            excludedIds.Add(_source.FunctionId);
            var parentOptions = allFunctions
                .Where(item => !excludedIds.Contains(item.FunctionId) && (string.IsNullOrWhiteSpace(item.NavigationTarget) || FunctionCatalog.Find(item.NavigationTarget)?.IsModule == true))
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
            cbbParent.Properties.Columns.Clear();
            cbbParent.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("DisplayName", "所屬群組"));
            txbNavigationTarget.Properties.DisplayMember = "Caption";
            txbNavigationTarget.Properties.ValueMember = "Key";
            txbNavigationTarget.Properties.Columns.Clear();
            txbNavigationTarget.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Caption", "導覽目標"));
            var icons = new List<DevExpress.XtraEditors.Controls.ImageComboBoxItem>
            { new DevExpress.XtraEditors.Controls.ImageComboBoxItem("預設圖示", "", -1) };
            foreach (var icon in FunctionCatalog.Icons)
            {
                int index = iconImages.Count;
                iconImages.Add(icon.Key, SvgIconCatalog.Resolve(icon.Key));
                icons.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(icon.Value, icon.Key, index));
            }
            cbbIcon.Properties.Items.AddRange(icons.ToArray());
            cbbParent.EditValueChanged += (sender, args) => UpdateTargets();
        }

        private void LoadValues()
        {
            txbCode.Text = _source.FunctionCode;
            txbDisplayName.Text = _source.DisplayName;
            memoDescription.Text = _source.Description;
            cbbParent.EditValue = _source.ParentFunctionId;
            UpdateTargets();
            txbNavigationTarget.EditValue = _source.NavigationTarget ?? "";
            cbbIcon.EditValue = _source.IconKey ?? "";
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
                FunctionCode = txbCode.Text.Trim().ToUpperInvariant(),
                ParentFunctionId = cbbParent.EditValue == null
                    ? (int?)null
                    : Convert.ToInt32(cbbParent.EditValue),
                DisplayName = txbDisplayName.Text.Trim(),
                Description = memoDescription.Text,
                NavigationTarget = Convert.ToString(txbNavigationTarget.EditValue),
                IconKey = Convert.ToString(cbbIcon.EditValue),
                SortOrder = Convert.ToInt32(speSortOrder.Value),
                DevelopmentStatus = GetStatusCode(cbbDevelopmentStatus.Text),
                IsWide = chkWide.Checked,
                IsVisible = chkVisible.Checked,
                IsActive = chkActive.Checked,
                RowVersion = _source.RowVersion
            };

            try
            {
                SavedId = save == null ? Value.FunctionId : save(Value);
            }
            catch (Exception exception)
            {
                XtraMessageBox.Show(exception.Message, "儲存功能失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cbbParent_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Clear) cbbParent.EditValue = null;
        }

        private void UpdateTargets()
        {
            string previous = Convert.ToString(txbNavigationTarget.EditValue);
            int? parentId = cbbParent.EditValue == null ? (int?)null : Convert.ToInt32(cbbParent.EditValue);
            FunctionTarget host = null;
            var seen = new HashSet<int>();
            while (parentId.HasValue && seen.Add(parentId.Value))
            {
                var parent = functions.FirstOrDefault(x => x.FunctionId == parentId.Value);
                if (parent == null) break;
                var candidate = FunctionCatalog.Find(parent.NavigationTarget);
                if (candidate?.IsModule == true) { host = candidate; break; }
                parentId = parent.ParentFunctionId;
            }
            var options = FunctionCatalog.Targets.Where(x => host == null ? x.IsModule : x.Host == host.Key)
                .Select(x => new TargetOption { Key = x.Key, Caption = x.Caption }).ToList();
            options.Insert(0, new TargetOption { Key = "", Caption = "群組（不開啟畫面）" });
            if (!string.IsNullOrWhiteSpace(_source.NavigationTarget) && FunctionCatalog.Find(_source.NavigationTarget) == null)
                options.Add(new TargetOption { Key = _source.NavigationTarget, Caption = "既有待實作功能" });
            txbNavigationTarget.Properties.DataSource = options;
            txbNavigationTarget.EditValue = options.Any(x => x.Key == previous) ? previous : "";
        }

        private sealed class TargetOption
        {
            public string Key { get; set; }
            public string Caption { get; set; }
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
