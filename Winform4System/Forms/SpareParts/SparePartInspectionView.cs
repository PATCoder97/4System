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
using System.Windows.Media.Media3D;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraSplashScreen;
using Winform4System.Forms.SpareParts;
using DevExpress.XtraEditors.Controls;
using System.Web.Util;
using Font = System.Drawing.Font;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraGrid.Views.Items.ViewInfo;
using Color = System.Drawing.Color;
using Winform4System.DataAccess.Entities.SpareParts;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Localization;
using DevExpress.XtraGrid.Menu;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Data;

namespace Winform4System.Forms.SpareParts
{
    /// <summary>
    /// Service sẽ tự động tạo đợt kiểm trả và trigger SQL sẽ tự bắt 20% lượng vật liệu trong kho để thêm vào bảng chi tiết
    /// </summary>
    public partial class SparePartInspectionView : DevExpress.XtraEditors.XtraUserControl
    {
        public SparePartInspectionView()
        {
            InitializeComponent();
            SparePartPermissionUi.Apply(this);
            InitializeIcon();
            InitializeMenuItems();

            helper = new SparePartRefreshHelper(gvData, "Id");

            Font fontUI12 = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            DevExpress.Utils.AppearanceObject.DefaultMenuFont = fontUI12;
        }

        SparePartRefreshHelper helper;
        BindingSource sourceBases = new BindingSource();
        string idDept2word = SparePartConfiguration.idDept2word;

        DXMenuItem itemUpdateRemainDate;
        DXMenuItem itemCancelBatch;
        DXMenuItem itemViewCheckPhoto;

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = SparePartSvgImages.Add;
            btnReload.ImageOptions.SvgImage = SparePartSvgImages.Reload;
            btnExportExcel.ImageOptions.SvgImage = SparePartSvgImages.Excel;
        }

        private void CreateRuleGV()
        {
            var ruleNotifyMain = new GridFormatRule
            {
                ApplyToRow = false,
                Column = gColStatus,
                Name = "RuleNotifyMain",
                Rule = new FormatConditionRuleExpression
                {
                    Expression = "[Status] == '待處理'",
                    Appearance =
                    {
                        ForeColor  = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Critical
                    }
                }
            };
            gvData.FormatRules.Add(ruleNotifyMain);

            var ruleCancelledMain = new GridFormatRule
            {
                ApplyToRow = false,
                Column = gColStatus,
                Name = "RuleCancelledMain",
                Rule = new FormatConditionRuleExpression
                {
                    Expression = "[Status] == '已取消'",
                    Appearance = { ForeColor = Color.Gray }
                }
            };
            gvData.FormatRules.Add(ruleCancelledMain);

            var rule = new GridFormatRule
            {
                ApplyToRow = true,
                Name = $"RuleNotify",
                Rule = new FormatConditionRuleExpression
                {
                    Expression = "[BatchCancelled] != true And [BatchMaterial.ActualQuantity] != [BatchMaterial.InitialQuantity]",
                    Appearance = { ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Critical }
                }
            };
            gvSparePart.FormatRules.Add(rule);
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
            itemUpdateRemainDate = CreateMenuItem("延時提醒", ItemUpdateRemainDate_Click, SparePartSvgImages.DateAdd);
            itemCancelBatch = CreateMenuItem("取消批次", ItemCancelBatch_Click, SparePartSvgImages.Remove);
            itemViewCheckPhoto = CreateMenuItem("查看圖片", ItemViewCheckPhoto_Click, SparePartSvgImages.Search);
            //itemUpdatePrice = CreateMenuItem("更新單價", ItemUpdatePrice_Click, SparePartSvgImages.Money);

            //itemMaterialIn = CreateMenuItem("收料", ItemMaterialIn_Click, SparePartSvgImages.Num1);
            //itemMaterialOut = CreateMenuItem("領用", ItemMaterialOut_Click, SparePartSvgImages.Num2);
            //itemMaterialTransfer = CreateMenuItem("轉庫", ItemMaterialTransfer_Click, SparePartSvgImages.Num3);
            //itemMaterialCheck = CreateMenuItem("盤點", ItemMaterialCheck_Click, SparePartSvgImages.Num4);
            //itemMaterialGetFromOther = CreateMenuItem("調撥", ItemMaterialGetFromOther_Click, SparePartSvgImages.Num5);
        }

        private bool IsBatchCancelled(SparePartInspectionBatch batch)
        {
            return batch != null && batch.IsCancelled;
        }

        private string GetBatchStatus(SparePartInspectionBatch batch, IEnumerable<dynamic> batchMaterialList)
        {
            if (IsBatchCancelled(batch))
            {
                return "已取消";
            }

            return batchMaterialList.Any(r => r.IsComplete != true) ? "待處理" : "已完成";
        }

        private void ItemUpdateRemainDate_Click(object sender, EventArgs e)
        {
            var editor = new TextEdit { Font = new System.Drawing.Font("Microsoft JhengHei UI", 14F) };

            // Thiết lập mask để buộc nhập đúng định dạng
            editor.Properties.MaskSettings.Set("MaskManagerType", typeof(DevExpress.Data.Mask.DateTimeMaskManager));
            editor.Properties.MaskSettings.Set("mask", "yyyy/MM/dd");
            editor.Properties.MaskSettings.Set("useAdvancingCaret", true);

            var result = XtraInputBox.Show(new XtraInputBoxArgs
            {
                Caption = SparePartConfiguration.SoftNameTW,
                Prompt = "輸入時間",
                Editor = editor,
                DefaultButtonIndex = 0,
                DefaultResponse = DateTime.Now.ToString("yyyy/MM/dd")
            });

            if (string.IsNullOrEmpty(result?.ToString())) return;

            DateTime respTime;
            DateTime.TryParse(result.ToString(), out respTime);

            GridView view = gvData;
            var batch = (view.GetRow(view.FocusedRowHandle) as dynamic).Batch as SparePartInspectionBatch;
            if (IsBatchCancelled(batch))
            {
                XtraMessageBox.Show("已取消批次不可延時提醒。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (respTime <= batch.CreatedDate || respTime < DateTime.Today)
            {
                XtraMessageBox.Show("日期不得早於建立日期或當前日期。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            batch.ExpiryDate = respTime;

            var resultUpdate = SparePartInspectionBatchService.Instance.AddOrUpdate(batch);

            if (resultUpdate)
            {
                LoadData();
            }
            else
            {
                SparePartMessage.MsgErrorDB();
            }
        }

        private void ItemCancelBatch_Click(object sender, EventArgs e)
        {
            GridView view = gvData;
            var batch = (view.GetRow(view.FocusedRowHandle) as dynamic).Batch as SparePartInspectionBatch;
            if (batch == null)
            {
                return;
            }

            if (IsBatchCancelled(batch))
            {
                XtraMessageBox.Show("此批次已取消。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string cancelReason = XtraInputBox.Show(new XtraInputBoxArgs
            {
                Caption = SparePartConfiguration.SoftNameTW,
                Prompt = "請輸入取消原因",
                DefaultResponse = string.Empty,
                Editor = new MemoEdit()
            })?.ToString();

            if (string.IsNullOrWhiteSpace(cancelReason))
            {
                XtraMessageBox.Show("請先輸入取消原因。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (XtraMessageBox.Show(
                    $"確定取消批次「{batch.BatchName}」嗎？",
                    "提示",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                return;
            }

            if (!SparePartInspectionBatchService.Instance.CancelBatch(batch.Id, SparePartConfiguration.LoginUser.Id, cancelReason, out string message))
            {
                SparePartMessage.MsgError(string.IsNullOrWhiteSpace(message) ? "取消批次失敗。" : message);
                return;
            }

            LoadData();
        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                helper.SaveViewInfo();

                var inspectionBatch = SparePartInspectionBatchService.Instance.GetList();
                var inspectionBatchMaterials = SparePartInspectionItemService.Instance.GetList();
                var materials = SparePartMaterialService.Instance.GetAll();

                var depts = SparePartDepartmentService.Instance.GetList();
                var users = SparePartUserService.Instance.GetList();
                var units = SparePartUnitService.Instance.GetList();
                Dictionary<string, SparePartInspectionAttachment> photoAttachmentsByThread = SparePartInspectionAttachmentService.Instance
                    .GetListByThreads(inspectionBatchMaterials.Select(x => SparePartInspectionPhotoHelper.GetThread(x.Id)).ToList())
                    .GroupBy(x => x.Thread)
                    .ToDictionary(group => group.Key, group => group.OrderByDescending(x => x.Id).First());

                var displayData = inspectionBatch.Select(batch =>
                {
                    var batchMaterialList = inspectionBatchMaterials
                        .Where(bm => bm.BatchId == batch.Id)
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
                                  Dept = (depts.Where(r => r.Id == m.IdDept).Select(r => $"{r.Id} {r.DisplayName}").FirstOrDefault() ?? "N/A"),
                                  UserReCheck = string.IsNullOrEmpty(bm.ConfirmedBy) ? "" : users.FirstOrDefault(r => r.Id == bm.ConfirmedBy)?.DisplayName ?? "N/A",
                                  PhotoActualName = photoAttachment?.ActualName ?? string.Empty,
                                  PhotoAttachment = photoAttachment,
                                  BatchCancelled = batch.IsCancelled,
                                  IsComplete = bm.IsComplete
                                  };
                              })
                        .ToList();

                    return new
                    {
                        Batch = batch,
                        Spare = batchMaterialList,
                        Status = GetBatchStatus(batch, batchMaterialList)
                    };
                }).ToList();

                sourceBases.DataSource = displayData;

                gvData.BestFitColumns();
                gvData.CollapseAllDetails();

                helper.LoadViewInfo();

                int rowHandle = gvData.FocusedRowHandle;
                if (gvData.IsMasterRow(rowHandle))
                {
                    gvData.ExpandMasterRow(rowHandle);
                }
            }
        }

        private void SparePartInspectionView_Load(object sender, EventArgs e)
        {
            gvSparePart.OptionsCustomization.AllowGroup = false;

            gvData.ReadOnlyGridView();
            gvData.KeyDown += SparePartGridHelper.GridViewCopyCellData_KeyDown;
            gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;
            gvSparePart.ReadOnlyGridView();
            gvSparePart.KeyDown += SparePartGridHelper.GridViewCopyCellData_KeyDown;
            gvSparePart.DoubleClick += GvSparePart_DoubleClick;

            LoadData();
            CreateRuleGV();
            gcData.DataSource = sourceBases;

            gvData.BestFitColumns();

            gvData.OptionsDetail.EnableMasterViewMode = true;
            gvData.OptionsView.ShowGroupPanel = false;
        }

        private void gvData_MasterRowGetRelationCount(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = 1;
        }

        private void gvData_MasterRowGetRelationName(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationNameEventArgs e)
        {
            e.RelationName = "備品";
        }

        private void gvData_MasterRowExpanded(object sender, DevExpress.XtraGrid.Views.Grid.CustomMasterRowEventArgs e)
        {
            GridView masterView = sender as GridView;
            int visibleDetailRelationIndex = masterView.GetVisibleDetailRelationIndex(e.RowHandle);
            GridView detailView = masterView.GetDetailView(e.RowHandle, visibleDetailRelationIndex) as GridView;

            detailView.BestFitColumns();
        }

        private void gvData_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (!e.HitInfo.InRowCell || !e.HitInfo.InDataRow || e.Menu == null)
            {
                return;
            }

            GridView view = sender as GridView;
            var batch = (view?.GetRow(e.HitInfo.RowHandle) as dynamic)?.Batch as SparePartInspectionBatch;
            if (batch == null || IsBatchCancelled(batch))
            {
                return;
            }

            e.Menu.Items.Add(itemUpdateRemainDate);
            e.Menu.Items.Add(itemCancelBatch);
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private bool TryOpenCheckPhoto(GridView view, bool showMessageWhenMissing)
        {
            if (view == null)
            {
                return false;
            }

            dynamic row = view.GetFocusedRow();
            if (row == null)
            {
                return false;
            }

            SparePartInspectionItem batchMaterial = row.BatchMaterial as SparePartInspectionItem;
            SparePartInspectionAttachment photoAttachment = row.PhotoAttachment as SparePartInspectionAttachment;
            if (batchMaterial == null || photoAttachment == null)
            {
                if (showMessageWhenMissing)
                {
                    XtraMessageBox.Show("目前尚未上傳盤點圖片。", SparePartConfiguration.SoftNameTW,
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                return false;
            }

            SparePartInspectionPhotoHelper.OpenPhotoFile(batchMaterial.Id, photoAttachment);
            return true;
        }

        private void ItemViewCheckPhoto_Click(object sender, EventArgs e)
        {
            TryOpenCheckPhoto(gcData.FocusedView as GridView, true);
        }

        private void GvSparePart_DoubleClick(object sender, EventArgs e)
        {
            TryOpenCheckPhoto(sender as GridView, true);
        }

        private void gvSparePart_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.MenuType == GridMenuType.Group)
            {
                GridViewGroupPanelMenu gridViewMenu = e.Menu as GridViewGroupPanelMenu;

                foreach (DXMenuItem menuItem in gridViewMenu.Items)
                {
                    if (menuItem.Caption.Equals(GridLocalizer.Active.GetLocalizedString(GridStringId.MenuGroupPanelClearGrouping))
                        || menuItem.Caption.Equals(GridLocalizer.Active.GetLocalizedString(GridStringId.MenuGroupPanelHide)))
                    {
                        menuItem.Visible = false;
                    }
                }

                return;
            }

            if (!e.HitInfo.InRowCell || !e.HitInfo.InDataRow || e.Menu == null)
            {
                return;
            }

            GridView view = sender as GridView;
            dynamic row = view?.GetRow(e.HitInfo.RowHandle);
            if (row == null)
            {
                return;
            }

            SparePartInspectionAttachment photoAttachment = row.PhotoAttachment as SparePartInspectionAttachment;
            if (photoAttachment == null)
            {
                return;
            }

            view.FocusedRowHandle = e.HitInfo.RowHandle;
            view.FocusedColumn = e.HitInfo.Column;
            e.Menu.Items.Add(itemViewCheckPhoto);
        }

        //private int GetRowCountRecursive(GridView view, int rowHandle)
        //{
        //    int totalCount = 0;
        //    int childrenCount = view.GetChildRowCount(rowHandle);
        //    for (int i = 0; i < childrenCount; i++)
        //    {
        //        var childRowHandle = view.GetChildRowHandle(rowHandle, i);
        //        if (view.IsGroupRow(childRowHandle))
        //        {
        //            totalCount += GetRowCountRecursive(view, childRowHandle);
        //        }
        //        else
        //        {
        //            totalCount++;
        //        }
        //    }
        //    return totalCount;
        //}

        private (int completeCount, int totalCount, int abnormalCount) GetRowCounts(GridView view, int rowHandle)
        {
            int completeCount = 0;
            int totalCount = 0;
            int abnormalCount = 0;

            int childrenCount = view.GetChildRowCount(rowHandle);
            for (int i = 0; i < childrenCount; i++)
            {
                int childRowHandle = view.GetChildRowHandle(rowHandle, i);

                if (view.IsGroupRow(childRowHandle))
                {
                    var (cCount, tCount, aCount) = GetRowCounts(view, childRowHandle);
                    completeCount += cCount;
                    totalCount += tCount;
                    abnormalCount += aCount;
                }
                else
                {
                    totalCount++;
                    object cellValue = view.GetRowCellValue(childRowHandle, gColIsComplete);
                    if (cellValue != null && bool.TryParse(cellValue.ToString(), out bool isComplete) && isComplete)
                    {
                        completeCount++;
                    }

                    object cellValueDesc = view.GetRowCellValue(childRowHandle, gColDesc);
                    if (cellValueDesc != null && !string.IsNullOrEmpty(cellValueDesc.ToString()))
                    {
                        abnormalCount++;
                    }
                }
            }

            return (completeCount, totalCount, abnormalCount);
        }

        private void gvSparePart_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            var view = (GridView)sender;
            var info = (GridGroupRowInfo)e.Info;
            var caption = info.Column.Caption;
            if (info.Column.Caption == string.Empty)
            {
                caption = info.Column.ToString();
            }

            var groupInfo = info.RowKey as GroupRowInfo;

            var (complete, total, abnormal) = GetRowCounts(view, e.RowHandle);

            bool isCancelled = false;
            int childCount = view.GetChildRowCount(e.RowHandle);
            for (int i = 0; i < childCount; i++)
            {
                int childRowHandle = view.GetChildRowHandle(e.RowHandle, i);
                if (view.IsGroupRow(childRowHandle))
                {
                    continue;
                }

                object cancelledValue = view.GetRowCellValue(childRowHandle, "BatchCancelled");
                if (cancelledValue != null && bool.TryParse(cancelledValue.ToString(), out bool batchCancelled) && batchCancelled)
                {
                    isCancelled = true;
                    break;
                }
            }

            bool groupComplete = total == complete;
            string colorName = isCancelled ? "Gray" : (groupComplete ? "Green" : "Red");
            string groupValue = isCancelled ? "已取消" : (groupComplete ? "已完成" : "處理中");

            info.GroupText = $" <color={colorName}>{groupValue}</color>：{info.GroupValueText}《<color=Blue>{total}物品</color></color>{(abnormal == 0 ? "" : $" <color=Red>{abnormal}異常</color>")}》";
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var editor = new TextEdit { Font = new System.Drawing.Font("Microsoft JhengHei UI", 14F) };

            var result = XtraInputBox.Show(new XtraInputBoxArgs
            {
                Caption = SparePartConfiguration.SoftNameTW,
                Prompt = "輸入名稱",
                Editor = editor,
            });

            if (string.IsNullOrEmpty(result?.ToString())) return;

            string batchName = $"【經理室】{result}";

            if (XtraMessageBox.Show($"你確定新增批次名稱為 {batchName} 嗎?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                return;

            var newBatchId = SparePartInspectionBatchService.Instance.Add(new SparePartInspectionBatch
            {
                CreatedDate = DateTime.Now,
                BatchName = batchName,
                ExpiryDate = DateTime.Now.AddDays(15),
                NotifyNo = 0,
                IsCancelled = false
            });

            if (newBatchId != -1)
            {
                LoadData();
            }
            else
            {
                SparePartMessage.MsgErrorDB();
            }
        }
    }
}
