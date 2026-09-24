using Winform4System.DataAccess.Entities.SpareParts;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using Winform4System.Forms.SpareParts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using Winform4System.Helpers;

namespace Winform4System.Forms.SpareParts
{
    public partial class SparePartRecoveryScheduleForm : XtraForm
    {
        private readonly List<SparePartUser> users;

        public SparePartRecoveryScheduleForm(List<SparePartUser> users, string assignedUserId, DateTime? plannedDisposeDate)
        {
            this.users = users ?? new List<SparePartUser>();

            InitializeComponent();
            SparePartPermissionUi.Apply(this);
            InitializeIcon();
            ConfigureUserLookup();

            sleAssignedUser.Properties.DataSource = this.users;
            sleAssignedUser.EditValue = assignedUserId;
            dePlannedDisposeDate.EditValue = (plannedDisposeDate ?? DateTime.Today).Date;
        }

        public string AssignedUserId => sleAssignedUser.EditValue?.ToString();

        public DateTime PlannedDisposeDate => dePlannedDisposeDate.DateTime.Date;

        private void InitializeIcon()
        {
            btnConfirm.ImageOptions.SvgImage = SvgIconCatalog.Confirm;
            btnCancel.ImageOptions.SvgImage = SvgIconCatalog.Cancel;
        }

        private void ConfigureUserLookup()
        {
            gvUserLookup.Columns.Clear();
            gvUserLookup.Appearance.HeaderPanel.Font = SparePartConfiguration.fontUI14;
            gvUserLookup.Appearance.HeaderPanel.Options.UseFont = true;
            gvUserLookup.Appearance.Row.Font = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            gvUserLookup.Appearance.Row.Options.UseFont = true;
            gvUserLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
            gvUserLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
            gvUserLookup.OptionsView.ColumnAutoWidth = false;
            gvUserLookup.OptionsView.EnableAppearanceOddRow = true;
            gvUserLookup.OptionsView.ShowAutoFilterRow = true;
            gvUserLookup.OptionsView.ShowGroupPanel = false;

            gvUserLookup.Columns.AddVisible("Id", "\u5de5\u865f").Width = 120;
            gvUserLookup.Columns.AddVisible("DisplayName", "\u59d3\u540d").Width = 180;
            gvUserLookup.Columns.AddVisible("IdDepartment", "\u55ae\u4f4d").Width = 140;
        }

        private void btnConfirm_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(AssignedUserId))
            {
                SparePartMessage.MsgError("\u8acb\u9078\u64c7\u5831\u5ee2\u7d93\u8fa6\u3002");
                return;
            }

            if (dePlannedDisposeDate.EditValue == null)
            {
                SparePartMessage.MsgError("\u8acb\u586b\u5beb\u9810\u8a08\u65e5\u671f\u3002");
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_ItemClick(object sender, ItemClickEventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
