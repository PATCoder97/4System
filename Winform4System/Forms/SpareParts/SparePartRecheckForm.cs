using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Winform4System.Business.Services.SpareParts;
using Winform4System.DataAccess.Entities.SpareParts;
using DevExpress.XtraEditors;
using Winform4System.Forms.SpareParts;

namespace Winform4System.Forms.SpareParts
{
    public partial class SparePartRecheckForm : DevExpress.XtraEditors.XtraForm
    {
        public SparePartRecheckForm()
        {
            InitializeComponent();
            SparePartPermissionUi.Apply(this);
            InitializeIcon();
        }

        private void InitializeIcon()
        {
            btnConfirm.ImageOptions.SvgImage = SparePartSvgImages.Confirm;
            btnCancel.ImageOptions.SvgImage = SparePartSvgImages.Cancel;
        }

        public bool _IsUploadAbnormal = false;
        public List<SparePartInspectionItem> InspectionBatchMaterials { get; set; } = new List<SparePartInspectionItem>();
        public Dictionary<int, string> InspectionPhotoNames { get; set; } = new Dictionary<int, string>();
        public Dictionary<int, string> InspectionPhotoPaths { get; set; } = new Dictionary<int, string>();
        public bool _isChecked { get; set; } = false;

        private void SparePartRecheckForm_Load(object sender, EventArgs e)
        {
            var materials = SparePartMaterialService.Instance.GetList();

            var depts = SparePartDepartmentService.Instance.GetList();
            var users = SparePartUserService.Instance.GetList();
            var units = SparePartUnitService.Instance.GetList();
            Dictionary<string, SparePartInspectionAttachment> photoAttachmentsByThread = SparePartInspectionAttachmentService.Instance
                .GetListByThreads(InspectionBatchMaterials.Select(x => SparePartInspectionPhotoHelper.GetThread(x.Id)).ToList())
                .GroupBy(x => x.Thread)
                .ToDictionary(group => group.Key, group => group.OrderByDescending(x => x.Id).First());

            var batchMaterialList = InspectionBatchMaterials
                .Join(materials,
                      bm => bm.MaterialId,
                      m => m.Id,
                      (bm, m) =>
                      {
                          string thread = SparePartInspectionPhotoHelper.GetThread(bm.Id);
                          photoAttachmentsByThread.TryGetValue(thread, out SparePartInspectionAttachment photoAttachment);

                          return new
                          {
                              Material = m,
                              BatchMaterial = bm,
                              Unit = units.FirstOrDefault(r => r.Id == m.IdUnit)?.DisplayName ?? "N/A",
                              UserMngr = users.FirstOrDefault(r => r.Id == m.IdManager)?.DisplayName ?? "N/A",
                              Dept = (depts.Where(r => r.Id == m.IdDept).Select(r => $"{r.Id} {r.DisplayName}").FirstOrDefault() ?? "N/A") + (bm.IsComplete != true ? " - 處理中" : " - 已完成"),
                              UserReCheck = string.IsNullOrEmpty(bm.ConfirmedBy) ? "" : users.FirstOrDefault(r => r.Id == bm.ConfirmedBy)?.DisplayName ?? "N/A",
                              IniQuantity = _IsUploadAbnormal ? bm.InitialQuantity : -1,
                              Desc = bm.Description,
                              PhotoName = InspectionPhotoNames.TryGetValue(bm.Id, out string photoName)
                                  ? photoName
                                  : photoAttachment?.ActualName ?? string.Empty
                          };
                      })
                .ToList();

            gcData.DataSource = batchMaterialList;
            gvSparePart.ReadOnlyGridView();
            gvSparePart.KeyDown += SparePartGridHelper.GridViewCopyCellData_KeyDown;
            gvSparePart.RowCellClick += GvSparePart_RowCellClick;
            gvSparePart.BestFitColumns();
        }

        private void GvSparePart_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            if (e.Button != MouseButtons.Left || e.Clicks < 1 || e.Column == null || e.Column.FieldName != "PhotoName")
            {
                return;
            }

            dynamic row = gvSparePart.GetRow(e.RowHandle);
            if (row == null)
            {
                return;
            }

            SparePartInspectionItem batchMaterial = row.BatchMaterial as SparePartInspectionItem;
            string photoName = row.PhotoName?.ToString() ?? string.Empty;
            if (batchMaterial == null || string.IsNullOrWhiteSpace(photoName))
            {
                return;
            }

            if (!InspectionPhotoPaths.TryGetValue(batchMaterial.Id, out string photoPath) || string.IsNullOrWhiteSpace(photoPath))
            {
                XtraMessageBox.Show("找不到對應的盤點圖片。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SparePartHelper.OpenPhotoFile(photoPath, photoName);
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (_IsUploadAbnormal)
            {
                if (InspectionBatchMaterials.Any(r => string.IsNullOrEmpty(r.Description)))
                {
                    XtraMessageBox.Show("請確認所有物料的異常說明是否已填寫！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else
            {
                if (InspectionBatchMaterials.Any(r => r.ActualQuantity == null))
                {
                    XtraMessageBox.Show("請先確認所有物料的實際數量！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                List<int> missingPhotoMaterials = InspectionBatchMaterials
                    .Where(r => !InspectionPhotoNames.ContainsKey(r.Id) || string.IsNullOrWhiteSpace(InspectionPhotoNames[r.Id]))
                    .Select(r => r.MaterialId)
                    .ToList();

                if (missingPhotoMaterials.Count > 0)
                {
                    string preview = string.Join("、", missingPhotoMaterials.Take(8));
                    if (missingPhotoMaterials.Count > 8)
                    {
                        preview += " ...";
                    }

                    XtraMessageBox.Show($"請先確認所有物料的盤點圖片！缺少圖片的編碼：{preview}", "提示",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            _isChecked = true;
            Close();
        }

        private void btnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            _isChecked = false;
            Close();
        }
    }
}
