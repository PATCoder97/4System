using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Winform4System.DataAccess.Entities;
using Winform4System.Helpers;

namespace Winform4System.Forms.SystemManagement
{
    public sealed class FunctionEditForm : XtraForm
    {
        private const string NotStartedText = "尚未開始";
        private const string InProgressText = "開發中";
        private const string CompletedText = "已完成";

        private readonly ApplicationFunction _source;
        private readonly TextEdit _codeEdit;
        private readonly TextEdit _nameEdit;
        private readonly MemoEdit _descriptionEdit;
        private readonly TextEdit _navigationEdit;
        private readonly LookUpEdit _parentEdit;
        private readonly SpinEdit _sortOrderEdit;
        private readonly ComboBoxEdit _statusEdit;
        private readonly CheckEdit _wideEdit;
        private readonly CheckEdit _visibleEdit;
        private readonly CheckEdit _activeEdit;

        public ApplicationFunction Value { get; private set; }

        public FunctionEditForm(ApplicationFunction source, IEnumerable<ApplicationFunction> allFunctions)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            if (allFunctions == null)
                throw new ArgumentNullException(nameof(allFunctions));

            Text = "編輯功能卡";
            IconOptions.Icon = Properties.Resources.AppIcon;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(720, 535);

            _codeEdit = new TextEdit { ReadOnly = true };
            _nameEdit = new TextEdit();
            _descriptionEdit = new MemoEdit();
            _navigationEdit = new TextEdit();
            _parentEdit = new LookUpEdit();
            _sortOrderEdit = new SpinEdit();
            _statusEdit = new ComboBoxEdit();
            _wideEdit = new CheckEdit { Text = "使用寬版卡片" };
            _visibleEdit = new CheckEdit { Text = "在主畫面顯示" };
            _activeEdit = new CheckEdit { Text = "啟用此功能" };

            ConfigureEditors(allFunctions.ToList());
            BuildLayout();
            LoadValues();
        }

        private void ConfigureEditors(List<ApplicationFunction> allFunctions)
        {
            _nameEdit.Properties.MaxLength = 200;
            _descriptionEdit.Properties.MaxLength = 500;
            _navigationEdit.Properties.MaxLength = 300;
            _sortOrderEdit.Properties.IsFloatValue = false;
            _sortOrderEdit.Properties.MinValue = 0;
            _sortOrderEdit.Properties.MaxValue = int.MaxValue;

            _statusEdit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            _statusEdit.Properties.Items.AddRange(new object[]
            {
                NotStartedText,
                InProgressText,
                CompletedText
            });

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

            _parentEdit.Properties.DataSource = parentOptions;
            _parentEdit.Properties.DisplayMember = nameof(ParentOption.DisplayName);
            _parentEdit.Properties.ValueMember = nameof(ParentOption.FunctionId);
            _parentEdit.Properties.NullText = "（頂層群組）";
            _parentEdit.Properties.AllowNullInput = DefaultBoolean.True;
            _parentEdit.Properties.Columns.Add(
                new DevExpress.XtraEditors.Controls.LookUpColumnInfo(nameof(ParentOption.DisplayName), "上層功能"));
        }

        private void BuildLayout()
        {
            var buttonPanel = new PanelControl
            {
                Dock = DockStyle.Bottom,
                Height = 58,
                BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
            };

            var cancelButton = new SimpleButton
            {
                Text = "取消",
                DialogResult = DialogResult.Cancel,
                Size = new Size(110, 36),
                Location = new Point(ClientSize.Width - 122, 10)
            };
            cancelButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cancelButton.ImageOptions.SvgImage = SvgIconCatalog.Cancel;

            var saveButton = new SimpleButton
            {
                Text = "儲存",
                Size = new Size(110, 36),
                Location = new Point(ClientSize.Width - 244, 10)
            };
            saveButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            saveButton.ImageOptions.SvgImage = SvgIconCatalog.Confirm;
            saveButton.Click += SaveButton_Click;

            buttonPanel.Controls.Add(saveButton);
            buttonPanel.Controls.Add(cancelButton);

            var layout = new LayoutControl { Dock = DockStyle.Fill };
            layout.Root.EnableIndentsWithoutBorders = DefaultBoolean.True;
            layout.Root.GroupBordersVisible = false;
            layout.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(18, 18, 18, 12);

            layout.AddItem("功能代碼", _codeEdit);
            layout.AddItem("顯示名稱", _nameEdit);
            layout.AddItem("所屬群組", _parentEdit);
            layout.AddItem("導覽目標", _navigationEdit);
            layout.AddItem("排序", _sortOrderEdit);
            layout.AddItem("開發狀態", _statusEdit);
            var descriptionItem = layout.AddItem("功能說明", _descriptionEdit);
            descriptionItem.SizeConstraintsType = SizeConstraintsType.Custom;
            descriptionItem.MinSize = new Size(0, 110);
            descriptionItem.MaxSize = new Size(0, 110);
            layout.AddItem(string.Empty, _wideEdit).TextVisible = false;
            layout.AddItem(string.Empty, _visibleEdit).TextVisible = false;
            layout.AddItem(string.Empty, _activeEdit).TextVisible = false;

            Controls.Add(layout);
            Controls.Add(buttonPanel);
            AcceptButton = saveButton;
            CancelButton = cancelButton;
        }

        private void LoadValues()
        {
            _codeEdit.Text = _source.FunctionCode;
            _nameEdit.Text = _source.DisplayName;
            _descriptionEdit.Text = _source.Description;
            _navigationEdit.Text = _source.NavigationTarget;
            _parentEdit.EditValue = _source.ParentFunctionId;
            _sortOrderEdit.Value = _source.SortOrder;
            _statusEdit.SelectedItem = GetStatusDisplay(_source.DevelopmentStatus);
            _wideEdit.Checked = _source.IsWide;
            _visibleEdit.Checked = _source.IsVisible;
            _activeEdit.Checked = _source.IsActive;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_nameEdit.Text))
            {
                XtraMessageBox.Show(
                    "請輸入顯示名稱。",
                    "資料不完整",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                _nameEdit.Focus();
                return;
            }

            Value = new ApplicationFunction
            {
                FunctionId = _source.FunctionId,
                FunctionCode = _source.FunctionCode,
                ParentFunctionId = _parentEdit.EditValue == null
                    ? (int?)null
                    : Convert.ToInt32(_parentEdit.EditValue),
                DisplayName = _nameEdit.Text.Trim(),
                Description = _descriptionEdit.Text,
                NavigationTarget = _navigationEdit.Text,
                SortOrder = Convert.ToInt32(_sortOrderEdit.Value),
                DevelopmentStatus = GetStatusCode(_statusEdit.Text),
                IsWide = _wideEdit.Checked,
                IsVisible = _visibleEdit.Checked,
                IsActive = _activeEdit.Checked
            };

            DialogResult = DialogResult.OK;
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
