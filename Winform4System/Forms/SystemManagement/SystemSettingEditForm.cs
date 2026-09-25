using DevExpress.XtraEditors;
using System;
using System.Windows.Forms;
using Winform4System.Business.Services;

namespace Winform4System.Forms.SystemManagement
{
    public sealed partial class SystemSettingEditForm : XtraForm
    {
        private readonly SystemSettingListItem _source;
        public SystemSettingEditModel Value { get; private set; }

        public SystemSettingEditForm(SystemSettingListItem source)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            InitializeComponent();
            txbKey.Text = source.SettingKey; txbName.Text = source.DisplayName; memoDescription.Text = source.Description;
            ConfigureValueEditor();
        }

        private void ConfigureValueEditor()
        {
            layoutTextValue.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutIntegerValue.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutBooleanValue.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            if (_source.ValueType == "INTEGER")
            {
                layoutIntegerValue.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                spinIntegerValue.Properties.MinValue = _source.MinimumValue ?? int.MinValue;
                spinIntegerValue.Properties.MaxValue = _source.MaximumValue ?? int.MaxValue;
                spinIntegerValue.Value = decimal.TryParse(_source.SettingValue, out decimal value) ? value : 0;
            }
            else if (_source.ValueType == "BOOLEAN")
            {
                layoutBooleanValue.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                chkBooleanValue.Checked = string.Equals(_source.SettingValue, "true", StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                layoutTextValue.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                txbTextValue.Text = _source.SettingValue;
            }
        }

        private void btnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string value = _source.ValueType == "INTEGER" ? Convert.ToInt32(spinIntegerValue.Value).ToString()
                : _source.ValueType == "BOOLEAN" ? (chkBooleanValue.Checked ? "true" : "false") : txbTextValue.Text;
            if (XtraMessageBox.Show("確定要更新系統參數「" + _source.DisplayName + "」嗎？", "更新系統參數確認",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            Value = new SystemSettingEditModel { SettingKey = _source.SettingKey, SettingValue = value, RowVersion = _source.RowVersion };
            DialogResult = DialogResult.OK; Close();
        }

        private void btnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) { DialogResult = DialogResult.Cancel; Close(); }
    }
}
