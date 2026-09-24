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
using Winform4System.Business.Services.SpareParts;
using Winform4System.DataAccess.Entities.SpareParts;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraSplashScreen;
using Winform4System.Forms.SpareParts;
using static DevExpress.XtraEditors.Mask.MaskSettings;

using Winform4System.Helpers;

namespace Winform4System.Forms.SpareParts
{
    public partial class SparePartCostView : DevExpress.XtraEditors.XtraUserControl
    {
        public SparePartCostView()
        {
            InitializeComponent();
            SparePartPermissionUi.Apply(this);
            InitializeIcon();
            //InitializeMenuItems();

            helper = new GridViewStateManager(gvData, "Id");

            System.Drawing.Font fontUI12 = new System.Drawing.Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            DevExpress.Utils.AppearanceObject.DefaultMenuFont = fontUI12;
        }

        GridViewStateManager helper;
        BindingSource sourceBases = new BindingSource();

        DateTime dateFrom, dateTo;

        private void InitializeIcon()
        {
            btnReload.ImageOptions.SvgImage = SvgImageCatalog.Search;
            btnExportExcel.ImageOptions.SvgImage = SvgImageCatalog.Excel;
        }

        private void LoadData()
        {
            SparePartDateRangeControl ucInfo = new SparePartDateRangeControl();
            if (XtraDialog.Show(ucInfo, "選時間", MessageBoxButtons.OKCancel) != DialogResult.OK)
                return;

            if (ucInfo.DateTo >= ucInfo.DateForm)
            {
                dateFrom = ucInfo.DateForm;
                dateTo = ucInfo.DateTo;
            }
            else
            {
                XtraMessageBox.Show("結束時間必須大於或等於開始時間。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                helper.SaveViewInfo();

                var depts = SparePartDepartmentService.Instance.GetList();
                var users = SparePartUserService.Instance.GetList();
                var materials = SparePartMaterialService.Instance.GetList();
                var storages = SparePartStorageService.Instance.GetList();

                var datas = SparePartTransactionService.Instance.GetListByDate(dateFrom, dateTo).Where(r => r.TransactionType == "out")
                    .Select(r => new SparePartTransaction
                    {
                        Id = r.Id,
                        StorageId = r.StorageId,
                        MaterialId = r.MaterialId,
                        Quantity = -r.Quantity,
                        TransactionType = r.TransactionType,
                        CreatedDate = r.CreatedDate,
                        AftQuantity = r.AftQuantity,
                        TotalQuantity = r.TotalQuantity,
                        UserDo = r.UserDo,
                        Desc = r.Desc
                    }).ToList();

                var displayData = (from data in datas
                                   join material in materials on data.MaterialId equals material.Id
                                   group new { data, material } by material.IdDept into deptGroup
                                   select new
                                   {
                                       Dept = depts.Where(d => d.Id == deptGroup.Key).Select(d => $"{d.Id} {d.DisplayName}").FirstOrDefault() ?? "N/A",
                                       IdDept = deptGroup.Key,
                                       TotalAmount = deptGroup.Sum(d => d.data.Quantity * d.material.Price),  // Tổng tiền của Dept
                                       Materials = deptGroup.GroupBy(x => x.data.StorageId).Select(storageGroup => new
                                       {
                                           StorageName = storages.Where(s => s.Id == storageGroup.Key).Select(s => s.DisplayName).FirstOrDefault() ?? "N/A",
                                           StorageId = storageGroup.Key,  // Kho lưu trữ
                                           TotalAmount = storageGroup.Sum(d => d.data.Quantity * d.material.Price),  // Tổng tiền của Storage
                                           Items = storageGroup
                                           .GroupBy(item => item.data.MaterialId)
                                           .Select(materialGroup => new
                                           {
                                               Material = materialGroup.First().material, // vật liệu đại diện
                                               Count = materialGroup.Count(), // số dòng xuất hiện
                                               UserMngr = users.FirstOrDefault(u => u.Id == materialGroup.First().material.IdManager)?.DisplayName ?? "N/A",
                                               TotalQuantity = materialGroup.Sum(d => d.data.Quantity),
                                               OutputTime = materialGroup.Count(),  // số lần xuất hiện (hoặc riêng biệt theo thời gian nếu có thể)
                                               TotalAmount = materialGroup.Sum(d => d.data.Quantity * d.material.Price)
                                           }).ToList()
                                       }).ToList()
                                   }).ToList();

                sourceBases.DataSource = displayData;

                gvData.BestFitColumns();
                gvData.CollapseAllDetails();

                helper.LoadViewInfo();
            }
        }

        private void gvData_MasterRowGetRelationCount(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = 1;
        }

        private void gvData_MasterRowGetRelationName(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationNameEventArgs e)
        {
            e.RelationName = "庫存";
        }

        private void gvStorage_MasterRowGetRelationCount(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = 1;
        }

        private void gvStorage_MasterRowGetRelationName(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationNameEventArgs e)
        {
            e.RelationName = "物料";
        }

        private void gridView_MasterRowExpanded(object sender, DevExpress.XtraGrid.Views.Grid.CustomMasterRowEventArgs e)
        {
            GridView masterView = sender as GridView;
            int visibleDetailRelationIndex = masterView.GetVisibleDetailRelationIndex(e.RowHandle);
            GridView detailView = masterView.GetDetailView(e.RowHandle, visibleDetailRelationIndex) as GridView;

            detailView.BestFitColumns();
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void btnExportExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string documentsPath = SparePartConfiguration.DocumentPath();
            if (!Directory.Exists(documentsPath))
                Directory.CreateDirectory(documentsPath);

            string filePath = Path.Combine(documentsPath, $"成本計算 - {DateTime.Now:yyyyMMddHHmm}.xlsx");

            gcData.ExportToXlsx(filePath);
            Process.Start(filePath);
        }

        private void SparePartCostView_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += DevExpressGridViewHelper.CopyFocusedCellOnCtrlC;
            gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;
            gvStorage.ReadOnlyGridView();
            gvStorage.KeyDown += DevExpressGridViewHelper.CopyFocusedCellOnCtrlC;
            gvStorage.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;
            gvMaterial.ReadOnlyGridView();
            gvMaterial.KeyDown += DevExpressGridViewHelper.CopyFocusedCellOnCtrlC;

            //// Kiểm tra quyền từng ke để có quyền truy cập theo nhóm
            //LoadData();
            //CreateRuleGV();
            gcData.DataSource = sourceBases;

            gvData.BestFitColumns();

            gvData.OptionsDetail.EnableMasterViewMode = true;
            gvData.OptionsView.ShowGroupPanel = false;
        }
    }
}
