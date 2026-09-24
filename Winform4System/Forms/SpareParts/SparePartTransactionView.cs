using Winform4System.Business.Services.SpareParts;
using Winform4System.DataAccess.Entities.SpareParts;
using DevExpress.Utils.Behaviors.Common;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;
using Winform4System.Forms.SpareParts;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Util;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using static DevExpress.XtraEditors.Mask.MaskSettings;

namespace Winform4System.Forms.SpareParts
{
    public partial class SparePartTransactionView : DevExpress.XtraEditors.XtraUserControl
    {
        public SparePartTransactionView()
        {
            InitializeComponent();
            SparePartPermissionUi.Apply(this);
            InitializeIcon();
            InitializeMenuItems();
            CreateRuleGV();

            helper = new SparePartRefreshHelper(gvData, "Id");

            Font fontUI12 = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            DevExpress.Utils.AppearanceObject.DefaultMenuFont = fontUI12;
        }

        SparePartRefreshHelper helper;
        BindingSource sourceBases = new BindingSource();
        string idDept2word = SparePartConfiguration.idDept2word;

        List<SparePartUser> users = new List<SparePartUser>();

        List<SparePartTransaction> transactions;
        List<SparePartMachine> machines;
        List<SparePartStorage> storages;
        List<SparePartUnit> units;

        DXMenuItem itemViewInfo;
        DXMenuItem itemUpdatePrice;
        DXMenuItem itemMaterialIn;
        DXMenuItem itemMaterialOut;
        DXMenuItem itemMaterialTransfer;
        DXMenuItem itemMaterialCheck;

        Dictionary<string, string> events = SparePartMaterialView.events;

        public class SparePartTransactionDisplay
        {
            public int Id { get; set; }
            public string IdDept { get; set; }
            public SparePartTransaction Transaction { get; set; }
            public SparePartMaterial Material { get; set; }
            public SparePartStorage Storage { get; set; }
            public KeyValuePair<string, string> Event { get; set; }
            public string User { get; set; }
        }


        private void InitializeIcon()
        {
            btnReload.ImageOptions.SvgImage = SparePartSvgImages.Reload;
            btnExportExcel.ImageOptions.SvgImage = SparePartSvgImages.Excel;
            barCbbDept.ImageOptions.SvgImage = SparePartSvgImages.Dept;
        }

        DXMenuItem CreateMenuItem(string caption, EventHandler clickEvent, SvgImage svgImage)
        {
            var menuItem = new DXMenuItem(caption, clickEvent, svgImage, DXMenuItemPriority.Normal);
            SetMenuItemProperties(menuItem);
            return menuItem;
        }

        void SetMenuItemProperties(DXMenuItem menuItem)
        {
            menuItem.ImageOptions.SvgImageSize = new System.Drawing.Size(24, 24);
            menuItem.AppearanceHovered.ForeColor = Color.Blue;
        }

        private void InitializeMenuItems()
        {
            //itemViewInfo = CreateMenuItem("查看資訊", ItemViewInfo_Click, SparePartSvgImages.View);
            //itemUpdatePrice = CreateMenuItem("更新單價", ItemUpdatePrice_Click, SparePartSvgImages.Money);

            //itemMaterialIn = CreateMenuItem("收料", ItemMaterialIn_Click, SparePartSvgImages.Num1);
            //itemMaterialOut = CreateMenuItem("領用", ItemMaterialOut_Click, SparePartSvgImages.Num2);
            //itemMaterialTransfer = CreateMenuItem("轉庫", ItemMaterialTransfer_Click, SparePartSvgImages.Num3);
            //itemMaterialCheck = CreateMenuItem("盤點", ItemMaterialCheck_Click, SparePartSvgImages.Num4);
        }

        private void CreateRuleGV()
        {
            // Quy tắc cảnh báo khi số lượng trong kho + máy < min
            var ruleCHECK = new GridFormatRule
            {
                Column = gColEvent,
                Name = "RuleCheck",
                Rule = new FormatConditionRuleExpression
                {
                    Expression = "[Transaction.TransactionType] = 'check'",
                    Appearance = { ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Critical, }
                }
            };
            gvData.FormatRules.Add(ruleCHECK);

            var ruleIN = new GridFormatRule
            {
                Column = gColEvent,
                Name = "RuleIn",
                Rule = new FormatConditionRuleExpression
                {
                    Expression = "[Transaction.TransactionType] = 'in'",
                    Appearance = { ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Question, }
                }
            };
            gvData.FormatRules.Add(ruleIN);

            var ruleTransfer = new GridFormatRule
            {
                Column = gColEvent,
                Name = "RuleTransfer",
                Rule = new FormatConditionRuleExpression
                {
                    Expression = "[Transaction.TransactionType] = 'transfer'",
                    Appearance = { ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Information, }
                }
            };
            gvData.FormatRules.Add(ruleTransfer);

        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                helper.SaveViewInfo();

                string deptGetData = string.IsNullOrWhiteSpace(barCbbDept.EditValue?.ToString()) ? "NoDept" : barCbbDept.EditValue.ToString().Split(' ')[0];
                transactions = SparePartTransactionService.Instance.GetList();

                var materials = SparePartMaterialService.Instance.GetListByStartIdDept(deptGetData);
                storages = SparePartStorageService.Instance.GetList();
                users = SparePartUserService.Instance.GetList();
                units = SparePartUnitService.Instance.GetList();

                var displayData = (from t in transactions
                                   join m in materials on t.MaterialId equals m.Id
                                   join s in storages on t.StorageId equals s.Id
                                   join e in events on t.TransactionType equals e.Key
                                   let user = users.FirstOrDefault(u => u.Id == t.UserDo)
                                   select new SparePartTransactionDisplay
                                   {
                                       Id = t.Id,
                                       IdDept = m.IdDept,
                                       Transaction = t,
                                       Material = m,
                                       Storage = s,
                                       Event = e,
                                       User = user?.DisplayName
                                   }).ToList();

                sourceBases.DataSource = displayData;

                gvData.BestFitColumns();
                gvData.CollapseAllDetails();

                helper.LoadViewInfo();
            }
        }

        private void SparePartTransactionView_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += SparePartGridHelper.GridViewCopyCellData_KeyDown;
            gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;
            gvSparePart.ReadOnlyGridView();
            gvSparePart.KeyDown += SparePartGridHelper.GridViewCopyCellData_KeyDown;

            // Kiểm tra quyền từng ke để có quyền truy cập theo nhóm
            var departmentItems = SparePartHelper.GetAccessibleDepartments()
                .Select(dept => new ComboBoxItem { Value = $"{dept.Id} {dept.DisplayName}" })
                .ToArray();

            cbbDept.Items.AddRange(departmentItems);
            barCbbDept.EditValue = departmentItems.FirstOrDefault()?.Value ?? string.Empty;

            LoadData();
            gcData.DataSource = sourceBases;

            gvData.BestFitColumns();

            gvData.OptionsDetail.EnableMasterViewMode = true;
            gvData.OptionsView.ShowGroupPanel = false;
        }

        private void barCbbDept_EditValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnReload_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        public static List<int> GetVisibleDataIds(GridView view)
        {
            var ids = new List<int>();

            for (int i = 0; i < view.DataRowCount; i++)
            {
                int rowHandle = view.GetVisibleRowHandle(i);

                // Cách nhanh: nếu có cột/field "data.Id" trong Grid (cột ẩn cũng được)
                var cell = view.GetRowCellValue(rowHandle, "Id");
                if (cell != null && int.TryParse(cell.ToString(), out int idFromCell))
                {
                    ids.Add(idFromCell);
                    continue;
                }

                // Fallback: lấy từ object ẩn danh bằng reflection
                var row = view.GetRow(rowHandle);
                if (row == null) continue;

                var dataProp = row.GetType().GetProperty("data");
                var dataVal = dataProp?.GetValue(row, null);
                var idProp = dataVal?.GetType().GetProperty("Id");
                var idVal = idProp?.GetValue(dataVal, null)?.ToString();

                if (int.TryParse(idVal, out int idFromProp))
                    ids.Add(idFromProp);
            }

            return ids;
        }

        private void btnExportExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            List<int> ids = GetVisibleDataIds(gvData);

            var allData = sourceBases.DataSource as IEnumerable<SparePartTransactionDisplay>;

            var exportData = allData
                .Where(d => ids.Contains(d.Id))
                .Select(d => new
                {
                    單位 = d.IdDept,
                    時期 = d.Transaction.CreatedDate,
                    材料編號 = d.Material.Code,
                    品名規格 = d.Material.DisplayName,
                    事件 = d.Event.Value,
                    倉庫 = d.Storage.DisplayName,
                    數量 = d.Transaction.Quantity,
                    數量後 = d.Transaction.AftQuantity,
                    總數量 = d.Transaction.TotalQuantity,
                    經辦人 = d.User,
                    備註 = d.Transaction.Desc
                })
                .ToList();

            string documentsPath = SparePartConfiguration.DocumentPath();
            if (!Directory.Exists(documentsPath))
                Directory.CreateDirectory(documentsPath);

            string filePath = Path.Combine(documentsPath, $"進出庫-{DateTime.Now:yyyyMMddHHmmss}.xlsx");

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (ExcelPackage pck = new ExcelPackage(filePath))
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Sheet1");
                ws.Cells.Style.Font.Name = "Microsoft JhengHei";
                ws.Cells.Style.Font.Size = 14;

                // Xuất dữ liệu từ list excelDatas sang Table
                ws.Cells["A1"].LoadFromCollection(exportData, true, OfficeOpenXml.Table.TableStyles.Medium2);
                // Bật WrapText cho tất cả các ô
                //ws.Column(3).Style.WrapText = true;
                ws.Cells[ws.Dimension.Address].AutoFitColumns();
                ws.Column(2).Style.Numberformat.Format = "yyyy/mm/dd hh:mm";

                // Lưu file
                pck.Save();
            }

            Process.Start(filePath);
        }
    }
}
