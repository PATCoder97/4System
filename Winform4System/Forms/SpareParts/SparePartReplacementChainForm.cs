using Winform4System.Business.Services.SpareParts;
using Winform4System.DataAccess.Entities.SpareParts;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using Winform4System.Forms.SpareParts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Winform4System.Helpers;

namespace Winform4System.Forms.SpareParts
{
    public partial class SparePartReplacementChainForm : XtraForm
    {
        private readonly SparePartMaterial sourceMaterial;
        private readonly List<SparePartReplacementChainNode> chainNodes;

        public SparePartReplacementChainForm(SparePartMaterial sourceMaterial, List<SparePartReplacementChainNode> chainNodes)
        {
            this.sourceMaterial = sourceMaterial ?? throw new ArgumentNullException(nameof(sourceMaterial));
            this.chainNodes = chainNodes ?? new List<SparePartReplacementChainNode>();

            InitializeComponent();
            SparePartPermissionUi.Apply(this);
            InitializeIcon();
            ConfigureGrid();
        }

        private void InitializeIcon()
        {
            btnClose.ImageOptions.SvgImage = SvgIconCatalog.Close;
        }

        private void ConfigureGrid()
        {
            gvData.Columns.Clear();
            gvData.ReadOnlyGridView();
            gvData.OptionsBehavior.Editable = false;
            gvData.OptionsSelection.EnableAppearanceFocusedCell = false;
            gvData.OptionsView.ColumnAutoWidth = false;
            gvData.OptionsView.EnableAppearanceOddRow = true;
            gvData.OptionsView.ShowGroupPanel = false;

            gvData.Columns.AddVisible(nameof(SparePartReplacementChainNode.StepNo), "\u9806\u5e8f").Width = 80;
            gvData.Columns.AddVisible(nameof(SparePartReplacementChainNode.Code), "\u7269\u6599\u7de8\u865f").Width = 140;
            gvData.Columns.AddVisible(nameof(SparePartReplacementChainNode.DisplayName), "\u54c1\u540d\u898f\u683c").Width = 320;
            gvData.Columns.AddVisible(nameof(SparePartReplacementChainNode.QuantityInStorage), "\u8ab2\u5eab\u6578\u91cf").Width = 110;
            gvData.Columns.AddVisible(nameof(SparePartReplacementChainNode.QuantityInMachine), "\u6a5f\u908a\u5eab\u6578\u91cf").Width = 120;
            gvData.Columns.AddVisible(nameof(SparePartReplacementChainNode.TotalQuantity), "\u7e3d\u6578\u91cf").Width = 100;

            var colPrice = gvData.Columns.AddVisible(nameof(SparePartReplacementChainNode.Price), "\u55ae\u50f9");
            colPrice.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            colPrice.DisplayFormat.FormatString = "n0";
            colPrice.Width = 110;

            var colReplacementDate = gvData.Columns.AddVisible(nameof(SparePartReplacementChainNode.ReplacementDate), "\u66ff\u4ee3\u65e5\u671f");
            colReplacementDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            colReplacementDate.DisplayFormat.FormatString = "yyyy/MM/dd";
            colReplacementDate.Width = 120;

            gvData.Columns.AddVisible(nameof(SparePartReplacementChainNode.Status), "\u72c0\u614b").Width = 90;
        }

        private void LoadData()
        {
            var displayNodes = chainNodes.Count > 0
                ? chainNodes
                : new List<SparePartReplacementChainNode>
                {
                    new SparePartReplacementChainNode
                    {
                        StepNo = 1,
                        MaterialId = sourceMaterial.Id,
                        Code = sourceMaterial.Code,
                        DisplayName = sourceMaterial.DisplayName,
                        QuantityInStorage = sourceMaterial.QuantityInStorage,
                        QuantityInMachine = sourceMaterial.QuantityInMachine,
                        TotalQuantity = sourceMaterial.QuantityInStorage + sourceMaterial.QuantityInMachine,
                        Price = sourceMaterial.Price,
                        ReplacementDate = sourceMaterial.ReplacementDate,
                        Status = sourceMaterial.IsDisable == true ? "\u505c\u7528" : "\u555f\u7528"
                    }
                };

            lblSummary.Text = $"\u66ff\u4ee3\u93c8\uff1a{string.Join(" -> ", displayNodes.Select(r => r.Code))}";
            gcData.DataSource = displayNodes;
            gvData.BestFitColumns();
        }

        private void SparePartReplacementChainForm_Load(object sender, EventArgs e)
        {
            gvData.KeyDown += DevExpressGridViewHelper.CopyFocusedCellOnCtrlC;
            LoadData();
        }

        private void btnClose_ItemClick(object sender, ItemClickEventArgs e)
        {
            Close();
        }
    }
}
