using DevExpress.XtraEditors;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Winform4System.Business.Services;
using Winform4System.Core.Security;
using Winform4System.DataAccess.Configuration;
using Winform4System.DataAccess.Entities;
using Winform4System.Helpers;

namespace Winform4System.Forms.SystemManagement
{
    public sealed class FunctionManagementView : XtraUserControl
    {
        private readonly ApplicationFunctionService _service;
        private readonly TreeList _tree;
        private readonly SimpleButton _editButton;

        private List<ApplicationFunction> _functions = new List<ApplicationFunction>();

        public FunctionManagementView()
        {
            _service = new ApplicationFunctionService(new ConnectionStringProvider());

            var toolbar = new PanelControl
            {
                Dock = DockStyle.Top,
                Height = 58,
                BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder,
                Padding = new Padding(12, 10, 12, 8)
            };

            var refreshButton = CreateButton("重新整理", SvgIconCatalog.Refresh, 12);
            refreshButton.Click += (sender, args) => LoadData();
            toolbar.Controls.Add(refreshButton);

            _editButton = CreateButton("編輯功能卡", SvgIconCatalog.Edit, 144);
            _editButton.Enabled = CurrentAuthorization.HasPermission("SYSTEM.FUNCTION.ADMIN");
            _editButton.Click += (sender, args) => EditSelected();
            toolbar.Controls.Add(_editButton);

            _tree = new TreeList { Dock = DockStyle.Fill };
            ConfigureTree();

            Controls.Add(_tree);
            Controls.Add(toolbar);
            Load += (sender, args) => LoadData();
        }

        private static SimpleButton CreateButton(string text, DevExpress.Utils.Svg.SvgImage image, int left)
        {
            return new SimpleButton
            {
                Text = text,
                ImageOptions = { SvgImage = image },
                Location = new Point(left, 9),
                Size = new Size(125, 36)
            };
        }

        private void ConfigureTree()
        {
            _tree.KeyFieldName = nameof(FunctionTreeRow.FunctionId);
            _tree.ParentFieldName = nameof(FunctionTreeRow.ParentFunctionId);
            _tree.OptionsBehavior.Editable = false;
            _tree.OptionsSelection.EnableAppearanceFocusedCell = false;
            _tree.OptionsView.ShowAutoFilterRow = true;
            _tree.OptionsView.ShowIndicator = true;
            _tree.Appearance.HeaderPanel.Font = new Font("Microsoft JhengHei UI", 11F);
            _tree.Appearance.HeaderPanel.Options.UseFont = true;
            _tree.Appearance.Row.Font = new Font("Microsoft JhengHei UI", 10.5F);
            _tree.Appearance.Row.Options.UseFont = true;
            _tree.DoubleClick += (sender, args) => EditSelected();

            AddColumn(nameof(FunctionTreeRow.DisplayName), "功能階層", 210);
            AddColumn(nameof(FunctionTreeRow.FunctionCode), "功能代碼", 200);
            AddColumn(nameof(FunctionTreeRow.Description), "功能說明", 260);
            AddColumn(nameof(FunctionTreeRow.NavigationTarget), "導覽目標", 190);
            AddColumn(nameof(FunctionTreeRow.SortOrder), "排序", 65);
            AddColumn(nameof(FunctionTreeRow.DevelopmentStatus), "開發狀態", 100);
            AddColumn(nameof(FunctionTreeRow.IsWide), "寬卡", 65);
            AddColumn(nameof(FunctionTreeRow.IsVisible), "顯示", 65);
            AddColumn(nameof(FunctionTreeRow.IsActive), "啟用", 65);
        }

        private void AddColumn(string fieldName, string caption, int width)
        {
            var column = new TreeListColumn
            {
                FieldName = fieldName,
                Caption = caption,
                Visible = true,
                VisibleIndex = _tree.Columns.Count,
                Width = width
            };
            _tree.Columns.Add(column);
        }

        private void LoadData()
        {
            try
            {
                _functions = _service.GetList().ToList();
                _tree.DataSource = _functions.Select(item => new FunctionTreeRow
                {
                    FunctionId = item.FunctionId,
                    ParentFunctionId = item.ParentFunctionId,
                    FunctionCode = item.FunctionCode,
                    DisplayName = item.DisplayName,
                    Description = item.Description,
                    NavigationTarget = item.NavigationTarget,
                    SortOrder = item.SortOrder,
                    DevelopmentStatus = GetDevelopmentStatusDisplay(item.DevelopmentStatus),
                    IsWide = item.IsWide,
                    IsVisible = item.IsVisible,
                    IsActive = item.IsActive
                }).ToList();
                _tree.ExpandAll();
                _tree.BestFitColumns();
            }
            catch (Exception exception)
            {
                XtraMessageBox.Show(
                    exception.Message,
                    "載入功能卡失敗",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void EditSelected()
        {
            if (!CurrentAuthorization.HasPermission("SYSTEM.FUNCTION.ADMIN"))
            {
                XtraMessageBox.Show(
                    "您沒有管理功能卡的權限。",
                    "權限不足",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            var row = _tree.FocusedNode == null
                ? null
                : _tree.GetDataRecordByNode(_tree.FocusedNode) as FunctionTreeRow;
            var selected = row == null
                ? null
                : _functions.FirstOrDefault(item => item.FunctionId == row.FunctionId);
            if (selected == null)
            {
                XtraMessageBox.Show(
                    "請先選擇需要編輯的功能卡。",
                    "功能卡管理",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            using (var form = new FunctionEditForm(selected, _functions))
            {
                if (form.ShowDialog(FindForm()) != DialogResult.OK)
                    return;

                try
                {
                    _service.Update(form.Value);
                    LoadData();
                    XtraMessageBox.Show(
                        "功能卡設定已儲存。",
                        "儲存成功",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                catch (Exception exception)
                {
                    XtraMessageBox.Show(
                        exception.Message,
                        "儲存功能卡失敗",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private static string GetDevelopmentStatusDisplay(string status)
        {
            switch ((status ?? string.Empty).Trim().ToUpperInvariant())
            {
                case "COMPLETED":
                    return "已完成";
                case "IN_PROGRESS":
                    return "開發中";
                default:
                    return "尚未開始";
            }
        }

        private sealed class FunctionTreeRow
        {
            public int FunctionId { get; set; }
            public int? ParentFunctionId { get; set; }
            public string FunctionCode { get; set; }
            public string DisplayName { get; set; }
            public string Description { get; set; }
            public string NavigationTarget { get; set; }
            public int SortOrder { get; set; }
            public string DevelopmentStatus { get; set; }
            public bool IsWide { get; set; }
            public bool IsVisible { get; set; }
            public bool IsActive { get; set; }
        }
    }
}
