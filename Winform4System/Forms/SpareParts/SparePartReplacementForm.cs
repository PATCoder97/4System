using Winform4System.DataAccess.Entities.SpareParts;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using Winform4System.Forms.SpareParts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Winform4System.Forms.SpareParts
{
    public partial class SparePartReplacementForm : XtraForm
    {
        private readonly SparePartMaterial sourceMaterial;
        private readonly List<SparePartMaterial> candidateMaterials;

        public int? SelectedReplacementMaterialId { get; private set; }
        public DateTime? SelectedReplacementDate { get; private set; }

        public SparePartReplacementForm(SparePartMaterial sourceMaterial, List<SparePartMaterial> candidateMaterials)
        {
            this.sourceMaterial = sourceMaterial ?? throw new ArgumentNullException(nameof(sourceMaterial));
            this.candidateMaterials = candidateMaterials ?? new List<SparePartMaterial>();

            InitializeComponent();
            SparePartPermissionUi.Apply(this);
            InitializeIcon();
            ConfigureReplacementLookup();
        }

        private void InitializeIcon()
        {
            btnConfirm.ImageOptions.SvgImage = SparePartSvgImages.Confirm;
            btnCancel.ImageOptions.SvgImage = SparePartSvgImages.Cancel;
        }

        private void ConfigureReplacementLookup()
        {
            gvReplacement.Columns.Clear();
            gvReplacement.OptionsBehavior.Editable = false;
            gvReplacement.OptionsSelection.EnableAppearanceFocusedCell = false;
            gvReplacement.OptionsView.ColumnAutoWidth = false;
            gvReplacement.OptionsView.EnableAppearanceOddRow = true;
            gvReplacement.OptionsView.ShowAutoFilterRow = true;
            gvReplacement.OptionsView.ShowGroupPanel = false;
            gvReplacement.FocusRectStyle = DrawFocusRectStyle.RowFocus;

            gvReplacement.Columns.AddVisible(nameof(SparePartMaterial.Code), "\u7269\u6599\u7de8\u865f").Width = 130;
            gvReplacement.Columns.AddVisible(nameof(SparePartMaterial.DisplayName), "\u54c1\u540d\u898f\u683c").Width = 280;
            gvReplacement.Columns.AddVisible(nameof(SparePartMaterial.Location), "\u6599\u4f4d").Width = 110;
            gvReplacement.Columns.AddVisible(nameof(SparePartMaterial.TypeUse), "\u7528\u9014").Width = 110;
        }

        private void LoadData()
        {
            txtSourceCode.EditValue = sourceMaterial.Code ?? string.Empty;
            txtSourceName.EditValue = sourceMaterial.DisplayName ?? string.Empty;

            sleReplacement.Properties.DataSource = candidateMaterials
                .Select(r => new
                {
                    r.Id,
                    r.Code,
                    r.DisplayName,
                    r.Location,
                    r.TypeUse
                })
                .ToList();

            if (sourceMaterial.ReplacementMaterialId != null)
            {
                sleReplacement.EditValue = sourceMaterial.ReplacementMaterialId;
            }

            deReplacementDate.EditValue = sourceMaterial.ReplacementDate?.Date ?? DateTime.Today;
        }

        private bool ValidateInput()
        {
            if (sleReplacement.EditValue == null)
            {
                SparePartMessage.MsgError("\u8acb\u9078\u64c7\u66ff\u4ee3\u6599\u3002");
                return false;
            }

            if (deReplacementDate.EditValue == null)
            {
                SparePartMessage.MsgError("\u8acb\u9078\u64c7\u66ff\u4ee3\u65e5\u671f\u3002");
                return false;
            }

            return true;
        }

        private void SparePartReplacementForm_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnConfirm_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!ValidateInput())
            {
                return;
            }

            SelectedReplacementMaterialId = Convert.ToInt32(sleReplacement.EditValue);
            SelectedReplacementDate = Convert.ToDateTime(deReplacementDate.EditValue).Date;
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
