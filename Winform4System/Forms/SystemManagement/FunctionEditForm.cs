using DevExpress.Utils;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Winform4System.DataAccess.Entities;
using Winform4System.Core.Models;
using Winform4System.Helpers;
using Winform4System.Business.Services;
using Winform4System.Core.Security;

namespace Winform4System.Forms.SystemManagement
{
    public enum FunctionEditMode { Create, View, Edit, Delete }

    public sealed partial class FunctionEditForm : XtraForm
    {
        private const string NotStartedText = "尚未開始";
        private const string InProgressText = "開發中";
        private const string CompletedText = "已完成";

        private readonly ApplicationFunction _source;
        private readonly Func<ApplicationFunction, int> save;
        private readonly Action<ApplicationFunction> delete;
        private FunctionEditMode mode;
        private string deletionBlockReason;
        private List<ApplicationFunction> functions;
        public int SavedId { get; private set; }

        public ApplicationFunction Value { get; private set; }

        public FunctionEditForm(ApplicationFunction source, IEnumerable<ApplicationFunction> allFunctions, Func<ApplicationFunction, int> saveAction = null, int? parentId = null,
            FunctionEditMode mode = FunctionEditMode.Edit, Action<ApplicationFunction> deleteAction = null)
        {
            _source = source ?? new ApplicationFunction { ParentFunctionId = parentId, DevelopmentStatus = "NOT_STARTED", IsVisible = true, IsActive = true };
            save = saveAction;
            delete = deleteAction;
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
            deletionBlockReason = ApplicationFunctionService.GetDeletionBlockReason(_source, functions.Any(x => x.ParentFunctionId == _source.FunctionId));
            SetMode(_source.FunctionId == 0 ? FunctionEditMode.Create : mode);
        }

        private void SetMode(FunctionEditMode value)
        {
            mode = value;
            bool canManage = CurrentAuthorization.HasPermission("SYSTEM.FUNCTION.ADMIN");
            bool editing = canManage && (mode == FunctionEditMode.Create || mode == FunctionEditMode.Edit);
            foreach (BaseEdit editor in new BaseEdit[] { txbCode, txbDisplayName, cbbParent, txbNavigationTarget, cbbIcon,
                speSortOrder, cbbDevelopmentStatus, memoDescription, chkWide, chkVisible, chkActive })
                editor.Properties.ReadOnly = !editing;
            txbCode.Properties.ReadOnly = !editing || _source.FunctionId > 0;
            btnEdit.Visibility = mode == FunctionEditMode.View && canManage
                ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
            btnDelete.Visibility = mode == FunctionEditMode.View && canManage && delete != null
                ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
            btnDelete.Enabled = deletionBlockReason == null;
            btnDelete.Hint = deletionBlockReason ?? "刪除此功能及其權限設定";
            btnSave.Visibility = mode == FunctionEditMode.View ? DevExpress.XtraBars.BarItemVisibility.Never : DevExpress.XtraBars.BarItemVisibility.Always;
            btnSave.Enabled = canManage && (mode != FunctionEditMode.Delete || (delete != null && deletionBlockReason == null));
            btnSave.Caption = mode == FunctionEditMode.Delete ? "確認刪除" : "儲存";
            btnSave.ImageOptions.SvgImage = mode == FunctionEditMode.Delete ? SvgIconCatalog.Delete : SvgIconCatalog.Confirm;
            btnCancel.Caption = mode == FunctionEditMode.View ? "關閉" : "取消";
            Text = mode == FunctionEditMode.Create ? "新增功能" : mode == FunctionEditMode.View ? "檢視功能" : mode == FunctionEditMode.Delete ? "刪除功能" : "編輯功能";
        }

        private void btnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (CurrentAuthorization.HasPermission("SYSTEM.FUNCTION.ADMIN")) SetMode(FunctionEditMode.Edit);
        }

        private void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (CurrentAuthorization.HasPermission("SYSTEM.FUNCTION.ADMIN") && delete != null && deletionBlockReason == null)
                SetMode(FunctionEditMode.Delete);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            layoutControl1.PerformLayout();
            int spacing = (int)Math.Ceiling(8 * DeviceDpi / 96F);
            ClientSize = new System.Drawing.Size(
                Math.Max(ClientSize.Width, Root.MinSize.Width + ClientSize.Width - layoutControl1.ClientSize.Width + spacing),
                Math.Max(ClientSize.Height, Root.MinSize.Height + ClientSize.Height - layoutControl1.ClientSize.Height + spacing));
            DialogLayoutHelper.FitToWorkingArea(this);
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
            if (!CurrentAuthorization.HasPermission("SYSTEM.FUNCTION.ADMIN") || mode == FunctionEditMode.View) return;
            if (mode == FunctionEditMode.Delete)
            {
                if (delete == null || deletionBlockReason != null) return;
                if (XtraMessageBox.Show(this, $"確定永久刪除「{_source.DisplayName}」嗎？\n功能代碼：{_source.FunctionCode}\n\n相關權限及角色授權將一併移除，受影響使用者須重新登入。\n業務資料不會刪除。此操作無法復原。",
                    "確認刪除功能", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.Yes) return;
                try { delete(_source); }
                catch (Exception exception)
                {
                    XtraMessageBox.Show(this, exception.Message, "刪除功能失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                SavedId = _source.ParentFunctionId ?? 0;
                DialogResult = DialogResult.OK;
                Close();
                return;
            }
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
            if (!cbbParent.Properties.ReadOnly && e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Clear) cbbParent.EditValue = null;
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
            var options = FunctionCatalog.Targets.Where(x => !x.IsModule || host == null)
                .Select(x => new TargetOption { Key = x.Key, Caption = x.Caption }).ToList();
            options.Insert(0, new TargetOption { Key = "", Caption = "功能群組（顯示子功能）" });
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
