using Winform4System.Business.Services.SpareParts;
using Winform4System.DataAccess.Entities.SpareParts;
using DevExpress.CodeParser;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraExport.Helpers;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraReports.UI;
using DevExpress.XtraSplashScreen;
using Winform4System.Forms.SpareParts;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using Color = System.Drawing.Color;
using Font = System.Drawing.Font;
using GridView = DevExpress.XtraGrid.Views.Grid.GridView;
using Tuple = System.Tuple;

using Winform4System.Helpers;

namespace Winform4System.Forms.SpareParts
{
    public partial class SparePartMaterialView : DevExpress.XtraEditors.XtraUserControl
    {
        public SparePartMaterialView()
        {
            InitializeComponent();
            SparePartPermissionUi.Apply(this);
            InitializeIcon();
            InitializeMenuItems();

            helper = new GridViewStateManager(gvData, "Id");

            Font fontUI12 = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            DevExpress.Utils.AppearanceObject.DefaultMenuFont = fontUI12;

            barCbbDept.EditValueChanged += CbbDept_EditValueChanged;
        }

        GridViewStateManager helper;
        BindingSource sourceBases = new BindingSource();
        string idDept2word = SparePartConfiguration.idDept2word;
        string deptGetData = "";

        List<SparePartUser> users = new List<SparePartUser>();
        List<SparePartUnit> units;

        List<SparePartMaterial> materials;
        List<SparePartMaterialPhoto> materialPhotos = new List<SparePartMaterialPhoto>();
        List<SparePartStorage> storages;

        public static Dictionary<string, string> events = new Dictionary<string, string>()
        {
            {"in","入庫" },
            {"out","出庫" },
            {"check","盤點" },
            {"transfer","轉庫" },
        };

        DXMenuItem itemViewInfo;
        DXMenuItem itemSetReplacement;
        DXMenuItem itemClearReplacement;
        DXMenuItem itemViewReplacementChain;
        DXMenuItem itemUploadPhoto;
        DXMenuItem itemViewPhoto;
        DXMenuItem itemUpdatePrice;
        DXMenuItem itemMaterialIn;
        DXMenuItem itemMaterialOut;
        DXMenuItem itemMaterialTransfer;
        DXMenuItem itemMaterialGetFromOther;
        DXMenuItem itemDisable;
        DXMenuItem itemEnable;
        DXMenuItem itemMultiselect;
        DXMenuItem itemPrintStamp;

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = SvgIconCatalog.Add;
            btnReload.ImageOptions.SvgImage = SvgIconCatalog.Refresh;
            btnExportExcel.ImageOptions.SvgImage = SvgIconCatalog.ExportExcel;
            barCbbDept.ImageOptions.SvgImage = SvgIconCatalog.Department;

            btnExcelBySpare.ImageOptions.SvgImage = SvgIconCatalog.Step1;
            btnExcelByMachine.ImageOptions.SvgImage = SvgIconCatalog.Step2;
            btnExcelByNotify.ImageOptions.SvgImage = SvgIconCatalog.Step3;
        }

        private void CreateRuleGV()
        {
            // Quy tắc định dạng khi TransactionType = 'C'
            var ruleCHECK = new GridFormatRule
            {
                Column = gColEvent,
                Name = "RuleCheck",
                Rule = new FormatConditionRuleExpression
                {
                    Expression = "[data.TransactionType] = 'check'",
                    Appearance = { ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Critical, }
                }
            };
            gvTransactions.FormatRules.Add(ruleCHECK);

            // Quy tắc định dạng khi TransactionType = 'I'
            var ruleIN = new GridFormatRule
            {
                Column = gColEvent,
                Name = "RuleIn",
                Rule = new FormatConditionRuleExpression
                {
                    Expression = "[data.TransactionType] = 'in'",
                    Appearance = { ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Question, }
                }
            };
            gvTransactions.FormatRules.Add(ruleIN);

            // Quy tắc định dạng khi TransactionType = 'I'
            var ruleTransfer = new GridFormatRule
            {
                Column = gColEvent,
                Name = "RuleTransfer",
                Rule = new FormatConditionRuleExpression
                {
                    Expression = "[data.TransactionType] = 'transfer'",
                    Appearance = { ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Information, }
                }
            };
            gvTransactions.FormatRules.Add(ruleTransfer);

            var ruleNotify = new GridFormatRule
            {
                ApplyToRow = false, // Áp dụng cho ô, không phải cả hàng
                Column = gColDisplayName, // Chỉ áp dụng cho cột gColDisplayName
                Name = "RuleNotify",
                Rule = new FormatConditionRuleExpression
                {
                    Expression = "[data.QuantityInStorage] + [data.QuantityInMachine] < [data.MinQuantity]",
                    Appearance =
                    {
                        BackColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Critical,
                        BackColor2 = Color.White,
                        Options = { UseBackColor = true }
                    }
                }
            };
            gvData.FormatRules.Add(ruleNotify);


            // Quy tắc hiển thị biểu tượng tăng/giảm trong lịch sử giao dịch
            var ruleIconSet = new GridFormatRule
            {
                Name = "RuleTransactionTrend",
                Column = gColQuantity,
                Rule = new FormatConditionRuleIconSet
                {
                    IconSet = new FormatConditionIconSet
                    {
                        ValueType = FormatConditionValueType.Automatic,
                        Icons =
                        {
                            new FormatConditionIconSetIcon { PredefinedName = "Arrows3_1.png", Value = 0, ValueComparison = FormatConditionComparisonType.Greater },
                            new FormatConditionIconSetIcon { PredefinedName = "Triangles3_2.png", Value = 0, ValueComparison = FormatConditionComparisonType.GreaterOrEqual },
                            new FormatConditionIconSetIcon { PredefinedName = "Arrows3_3.png", Value = decimal.MinValue, ValueComparison = FormatConditionComparisonType.GreaterOrEqual }
                        }
                    }
                }
            };
            gvTransactions.FormatRules.Add(ruleIconSet);

            var ruleUserError = new GridFormatRule
            {
                Column = gColUserMngr,
                Name = "RuleUserError",
                Rule = new FormatConditionRuleExpression
                {
                    Expression = "[UserError] == true",
                    Appearance =
                    {
                        BackColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Critical,
                        BackColor2 = Color.White,
                        Options = { UseBackColor = true }
                    }
                }
            };
            gvData.FormatRules.Add(ruleUserError);

            var ruleDisabledMaterial = new GridFormatRule
            {
                ApplyToRow = true,
                Name = "RuleDisabledMaterial",
                Rule = new FormatConditionRuleExpression
                {
                    Expression = "[data.IsDisable] = True",
                    Appearance =
                    {
                        ForeColor = Color.DimGray,
                        Font = new Font(gvData.Appearance.Row.Font, FontStyle.Italic),
                        Options =
                        {
                            UseForeColor = true,
                            UseFont = true
                        }
                    }
                }
            };
            gvData.FormatRules.Add(ruleDisabledMaterial);
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
            itemViewInfo = CreateMenuItem("查看資訊", ItemViewInfo_Click, SvgIconCatalog.View);
            itemSetReplacement = CreateMenuItem("設定替代料", ItemSetReplacement_Click, SvgIconCatalog.Edit);
            itemClearReplacement = CreateMenuItem("清除替代料", ItemClearReplacement_Click, SvgIconCatalog.Delete);
            itemViewReplacementChain = CreateMenuItem("查看替代鏈", ItemViewReplacementChain_Click, SvgIconCatalog.Search);
            itemUploadPhoto = CreateMenuItem("上傳圖片", ItemUploadPhoto_Click, SvgIconCatalog.Upload);
            itemViewPhoto = CreateMenuItem("查看圖片", ItemViewPhoto_Click, SvgIconCatalog.Search);
            itemUpdatePrice = CreateMenuItem("更新單價", ItemUpdatePrice_Click, SvgIconCatalog.Cost);

            itemMaterialIn = CreateMenuItem("收料", ItemMaterialIn_Click, SvgIconCatalog.Step1);
            itemMaterialOut = CreateMenuItem("領用", ItemMaterialOut_Click, SvgIconCatalog.Step2);
            itemMaterialTransfer = CreateMenuItem("轉庫", ItemMaterialTransfer_Click, SvgIconCatalog.Step3);
            itemMaterialGetFromOther = CreateMenuItem("調撥", ItemMaterialGetFromOther_Click, SvgIconCatalog.Step4);

            itemDisable = CreateMenuItem("停用", ItemDisable_Click, SvgIconCatalog.Disabled);
            itemEnable = CreateMenuItem("啟用", ItemEnable_Click, SvgIconCatalog.Confirm);
            itemMultiselect = CreateMenuItem("啟用多選", ItemMultiselect_Click, SvgIconCatalog.SelectionChecked);
            itemPrintStamp = CreateMenuItem("執行列印", ItemPrintStamp_Click, SvgIconCatalog.Print);
        }

        private dynamic GetFocusedDisplayRow()
        {
            if (gvData.FocusedRowHandle < 0) return null;
            return gvData.GetRow(gvData.FocusedRowHandle) as dynamic;
        }

        private SparePartMaterial GetFocusedMaterial()
        {
            var row = GetFocusedDisplayRow();
            return row?.data as SparePartMaterial;
        }

        private SparePartMaterialPhoto GetFocusedMaterialPhoto()
        {
            var row = GetFocusedDisplayRow();
            return row?.Photo as SparePartMaterialPhoto;
        }

        private string GetReplacementTargetUsageMessage(SparePartMaterial material)
        {
            if (material == null)
            {
                return "找不到對應的物料。";
            }

            return SparePartMaterialService.Instance.GetReplacementTargetUsageMessage(material.Id);
        }

        private bool EnsureMaterialActive(SparePartMaterial material, string actionText)
        {
            if (material == null)
            {
                XtraMessageBox.Show("找不到對應的物料。", SparePartConfiguration.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (material.IsDisable == true)
            {
                XtraMessageBox.Show($"停用中的物料不可{actionText}。", SparePartConfiguration.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void UploadPhotoForMaterial(SparePartMaterial material)
        {
            if (!EnsureMaterialActive(material, "上傳圖片")) return;

            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "選擇圖片";
                dialog.Filter = "Image Files|*.jpg;*.jpeg;*.png";

                if (dialog.ShowDialog() != DialogResult.OK) return;

                var savedFile = SparePartHelper.SaveMaterialPhoto(material.Id, dialog.FileName);
                int result = SparePartMaterialPhotoService.Instance.AddOrReplace(new SparePartMaterialPhoto()
                {
                    MaterialId = material.Id,
                    EncryptionName = savedFile.encryptionName,
                    ActualName = savedFile.actualName,
                    UploadedBy = SparePartConfiguration.LoginUser.Id,
                    UploadedDate = DateTime.Now,
                    IsActive = true
                });

                if (result <= 0)
                {
                    XtraMessageBox.Show("上傳圖片失敗。", SparePartConfiguration.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            LoadData();
        }

        private void ViewPhoto(SparePartMaterialPhoto photo)
        {
            if (photo == null)
            {
                XtraMessageBox.Show("目前尚未上傳圖片。", SparePartConfiguration.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SparePartHelper.OpenPhotoFile(SparePartHelper.GetMaterialPhotoPath(photo), photo.ActualName);
        }

        private void ToggleMaterialDisable(bool disable)
        {
            SparePartMaterial material = GetFocusedMaterial();
            if (material == null)
            {
                XtraMessageBox.Show("找不到對應的物料。", SparePartConfiguration.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (disable == (material.IsDisable == true)) return;

            if (disable)
            {
                string replacementUsageMessage = GetReplacementTargetUsageMessage(material);
                if (!string.IsNullOrEmpty(replacementUsageMessage))
                {
                    XtraMessageBox.Show(replacementUsageMessage, SparePartConfiguration.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            string actionName = disable ? "停用" : "啟用";
            var confirmUserId = XtraInputBox.Show(new XtraInputBoxArgs
            {
                Caption = SparePartConfiguration.SoftNameTW,
                Prompt = $"請輸入您的工號以確認{actionName}",
                DefaultButtonIndex = 0,
                Editor = new TextEdit(),
                DefaultResponse = ""
            })?.ToString().ToUpper();

            if (string.IsNullOrEmpty(confirmUserId) || confirmUserId != SparePartConfiguration.LoginUser.Id.ToUpper()) return;

            bool actionResult = disable
                ? SparePartMaterialService.Instance.DisableById(material.Id, SparePartConfiguration.LoginUser.Id)
                : SparePartMaterialService.Instance.EnableById(material.Id, SparePartConfiguration.LoginUser.Id);

            if (!actionResult)
            {
                XtraMessageBox.Show($"{actionName}失敗。", SparePartConfiguration.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            LoadData();
        }

        private void ConfigureMaterialReplacement()
        {
            SparePartMaterial material = GetFocusedMaterial();
            if (!EnsureMaterialActive(material, "設定替代料")) return;

            var candidateMaterials = SparePartMaterialService.Instance.GetReplacementCandidateList(material.Id);
            if (candidateMaterials.Count == 0 && material.ReplacementMaterialId == null)
            {
                XtraMessageBox.Show("目前沒有可設定的替代料。", SparePartConfiguration.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var form = new SparePartReplacementForm(material, candidateMaterials))
            {
                if (form.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                string resultMessage = SparePartMaterialService.Instance.UpdateReplacement(
                    material.Id,
                    form.SelectedReplacementMaterialId,
                    form.SelectedReplacementDate);

                if (!string.IsNullOrEmpty(resultMessage))
                {
                    SparePartMessage.MsgError(resultMessage);
                    return;
                }
            }

            LoadData();
        }

        private void ClearMaterialReplacement()
        {
            SparePartMaterial material = GetFocusedMaterial();
            if (!EnsureMaterialActive(material, "清除替代料")) return;

            if (material.ReplacementMaterialId == null)
            {
                XtraMessageBox.Show("目前尚未設定替代料。", SparePartConfiguration.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirmResult = XtraMessageBox.Show(
                $"您確認要清除物料「{material.Code}」的替代設定嗎？",
                SparePartConfiguration.SoftNameTW,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmResult != DialogResult.Yes)
            {
                return;
            }

            string resultMessage = SparePartMaterialService.Instance.UpdateReplacement(material.Id, null, null);
            if (!string.IsNullOrEmpty(resultMessage))
            {
                SparePartMessage.MsgError(resultMessage);
                return;
            }

            LoadData();
        }

        private void ViewMaterialReplacementChain()
        {
            SparePartMaterial material = GetFocusedMaterial();
            if (material == null)
            {
                XtraMessageBox.Show("找不到對應的物料。", SparePartConfiguration.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var chainNodes = SparePartMaterialService.Instance.GetReplacementChain(material.Id);
            using (var form = new SparePartReplacementChainForm(material, chainNodes))
            {
                form.ShowDialog();
            }
        }

        private void ItemPrintStamp_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> stampFormats = new Dictionary<string, string>()
            {
                { "10×5.21 cm", "sparepart-10x5,21.repx" },
                { "10×8 cm", "sparepart-10x8.repx" },
            };

            XtraInputBoxArgs args = new XtraInputBoxArgs();

            args.Caption = "標籤格式";
            args.Prompt = $"請選擇標籤格式";
            args.DefaultButtonIndex = 0;
            ComboBoxEdit editor = new ComboBoxEdit();

            editor.Properties.Items.AddRange(stampFormats.Select(r => r.Key).ToList());
            args.Editor = editor;

            editor.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
            editor.Properties.Appearance.Font = SparePartConfiguration.fontUI14;
            editor.Properties.AppearanceDropDown.Font = SparePartConfiguration.fontUI14;
            editor.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            editor.Properties.NullText = "";

            var result = XtraInputBox.Show(args);
            if (result == null) return;

            string stampName = stampFormats.First(r => r.Key == result.ToString()).Value;

            var selectedItems = gvData.GetSelectedRows()
                .Select(rowHandle => (gvData.GetRow(rowHandle) as dynamic).data as SparePartMaterial)
                .Where(item => item != null)
                .ToList();

            var labels = selectedItems.Select(r => new
            {
                code = r.Code,
                unit = units.FirstOrDefault(x => x.Id == r.IdUnit).DisplayName,
                productname = r.DisplayName,
                place = r.Location
            });
            var report = new XtraReport();
            string reportFormatpath = Path.Combine(SparePartConfiguration.FolderReportFormat, stampName);
            report.LoadLayout(reportFormatpath);
            report.ShowPrintMarginsWarning = false;

            // Gán danh sách làm nguồn dữ liệu
            report.DataSource = labels;

            // Không cần set Parameter nữa
            report.CreateDocument();

            new ReportPrintTool(report).ShowPreviewDialog();
        }

        private void ItemMultiselect_Click(object sender, EventArgs e)
        {
            bool multiSelect = gvData.OptionsSelection.MultiSelect;
            gvData.OptionsSelection.MultiSelect = !multiSelect;
            gvData.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;
        }

        private void HandleMaterialTransaction(string eventInfo)
        {
            SparePartMaterial material = GetFocusedMaterial();
            if (!EnsureMaterialActive(material, "進行庫存作業")) return;

            var transactionForm = new SparePartTransactionForm
            {
                eventInfo = eventInfo,
                idMaterial = material.Id
            };

            transactionForm.ShowDialog();
            LoadData();
        }

        private void ItemMaterialGetFromOther_Click(object sender, EventArgs e) => HandleMaterialTransaction("調撥");

        private void ItemMaterialTransfer_Click(object sender, EventArgs e) => HandleMaterialTransaction("轉庫");

        private void ItemMaterialOut_Click(object sender, EventArgs e) => HandleMaterialTransaction("領用");

        private void ItemMaterialIn_Click(object sender, EventArgs e) => HandleMaterialTransaction("收貨");

        private void ItemUpdatePrice_Click(object sender, EventArgs e)
        {
            SparePartMaterial material = GetFocusedMaterial();
            if (!EnsureMaterialActive(material, "更新單價")) return;

            var result = XtraInputBox.Show(new XtraInputBoxArgs
            {
                Caption = SparePartConfiguration.SoftNameTW,
                Prompt = "請輸入新單價",
                DefaultButtonIndex = 0,
                Editor = new TextEdit
                {
                    Font = new System.Drawing.Font("Microsoft JhengHei UI", 14F),
                    Properties = { Mask = { MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric, EditMask = "N0", UseMaskAsDisplayFormat = true } }
                },
                DefaultResponse = ""
            })?.ToString().ToUpper();

            if (string.IsNullOrEmpty(result)) return;

            var newPrice = Convert.ToInt32(result);
            if (newPrice < 0) return;

            var resultUpdate = SparePartPriceService.Instance.Add(new SparePartPrice()
            {
                MaterialId = material.Id,
                Price = newPrice,
                ChangedAt = DateTime.Now,
                ChangedBy = SparePartConfiguration.LoginUser.Id
            });

            LoadData();
        }

        private void ItemUploadPhoto_Click(object sender, EventArgs e)
        {
            UploadPhotoForMaterial(GetFocusedMaterial());
        }

        private void ItemSetReplacement_Click(object sender, EventArgs e)
        {
            ConfigureMaterialReplacement();
        }

        private void ItemClearReplacement_Click(object sender, EventArgs e)
        {
            ClearMaterialReplacement();
        }

        private void ItemViewReplacementChain_Click(object sender, EventArgs e)
        {
            ViewMaterialReplacementChain();
        }

        private void ItemViewPhoto_Click(object sender, EventArgs e)
        {
            ViewPhoto(GetFocusedMaterialPhoto());
        }

        private void ItemDisable_Click(object sender, EventArgs e)
        {
            ToggleMaterialDisable(true);
        }

        private void ItemEnable_Click(object sender, EventArgs e)
        {
            ToggleMaterialDisable(false);
        }

        private void ItemViewInfo_Click(object sender, EventArgs e)
        {
            if (deptGetData.Length != 4)
            {
                XtraMessageBox.Show("請您選擇「課」來查看物料", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            GridView view = gvData;
            int idMaterial = Convert.ToInt16(view.GetRowCellValue(view.FocusedRowHandle, gColIdMaterial));
            SparePartMaterialForm fInfo = new SparePartMaterialForm()
            {
                eventInfo = SparePartFormMode.View,
                formName = "物料",
                idBase = idMaterial,
                idDeptGetData = deptGetData
            };
            fInfo.ShowDialog();

            LoadData();
        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                helper.SaveViewInfo();

                deptGetData = string.IsNullOrWhiteSpace(barCbbDept.EditValue?.ToString())
                   ? "NoDept" : barCbbDept.EditValue.ToString().Split(' ')[0];
                storages = SparePartStorageService.Instance.GetList();
                users = SparePartUserService.Instance.GetList();
                units = SparePartUnitService.Instance.GetList();
                materialPhotos = SparePartMaterialPhotoService.Instance.GetList().Where(r => r.IsActive).ToList();

                materials = SparePartMaterialService.Instance.GetListByStartIdDept(deptGetData);
                var materialsRechecking = SparePartInspectionItemService.Instance.GetListRechecking().Select(r => r.MaterialId).ToList();
                var activePhotos = materialPhotos
                    .GroupBy(r => r.MaterialId)
                    .ToDictionary(r => r.Key, r => r.OrderByDescending(x => x.UploadedDate).FirstOrDefault());
                var materialsById = materials.ToDictionary(r => r.Id);

                var displayData = materials
                    .Where(r => !materialsRechecking.Contains(r.Id))
                    .Select(x =>
                    {
                        var userMngr = users.FirstOrDefault(u => u.Id == x.IdManager);
                        activePhotos.TryGetValue(x.Id, out SparePartMaterialPhoto photo);
                        SparePartMaterial replacementMaterial = null;
                        if (x.ReplacementMaterialId != null)
                        {
                            materialsById.TryGetValue(x.ReplacementMaterialId.Value, out replacementMaterial);
                        }

                        return new
                        {
                            data = x,
                            Unit = units.FirstOrDefault(u => u.Id == x.IdUnit)?.DisplayName,
                            Status = x.IsDisable == true ? "停用" : "啟用",
                            ReplacementMaterialCode = replacementMaterial?.Code ?? string.Empty,
                            ReplacementDate = x.ReplacementDate,
                            PhotoStatus = photo != null,
                            Photo = photo,
                            UserMngr = userMngr != null ? $"{userMngr.IdDepartment}/{userMngr.DisplayName}" : "",
                            UserError = userMngr != null ? userMngr.Status != 0 || userMngr.IdDepartment != x.IdDept : false
                        };
                    })
                    .ToList();

                sourceBases.DataSource = displayData;

                gvData.BestFitColumns();
                gvData.CollapseAllDetails();
                gColDisplayName.Width = 250;

                helper.LoadViewInfo();
            }
        }

        private void SparePartMaterialView_Load(object sender, EventArgs e)
        {
            gvData.ReadOnlyGridView();
            gvData.KeyDown += DevExpressGridViewHelper.CopyFocusedCellOnCtrlC;
            gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;
            gvTransactions.ReadOnlyGridView();
            gvTransactions.KeyDown += DevExpressGridViewHelper.CopyFocusedCellOnCtrlC;
            gvPrices.ReadOnlyGridView();
            gvPrices.KeyDown += DevExpressGridViewHelper.CopyFocusedCellOnCtrlC;
            gvMachine.ReadOnlyGridView();
            gvMachine.KeyDown += DevExpressGridViewHelper.CopyFocusedCellOnCtrlC;

            gColDisplayName.AppearanceCell.Options.HighPriority = true;

            // Kiểm tra quyền từng ke để có quyền truy cập theo nhóm
            var departmentItems = SparePartHelper.GetAccessibleDepartments()
                .Select(dept => new ComboBoxItem { Value = $"{dept.Id} {dept.DisplayName}" })
                .ToArray();

            cbbDept.Items.AddRange(departmentItems);
            barCbbDept.EditValue = departmentItems.FirstOrDefault()?.Value ?? string.Empty;

            LoadData();
            CreateRuleGV();
            gcData.DataSource = sourceBases;

            gvData.BestFitColumns();
            gColDisplayName.Width = 250;

            gvData.OptionsDetail.EnableMasterViewMode = true;
            gvData.OptionsView.ShowGroupPanel = false;
        }

        private void CbbDept_EditValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (deptGetData.Length != 4)
            {
                XtraMessageBox.Show("請您選擇「課」來新增物料", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SparePartMaterialForm finfo = new SparePartMaterialForm()
            {
                eventInfo = SparePartFormMode.Create,
                formName = "物料",
                idDeptGetData = deptGetData
            };

            finfo.ShowDialog();
            LoadData();
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void gvData_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell && e.HitInfo.InDataRow)
            {
                bool multiSelect = gvData.OptionsSelection.MultiSelect;
                SparePartMaterial material = GetFocusedMaterial();
                SparePartMaterialPhoto photo = GetFocusedMaterialPhoto();
                itemMultiselect.Caption = multiSelect ? "啟用單選" : "啟用多選";
                itemViewPhoto.Enabled = photo != null;
                itemClearReplacement.Enabled = material?.ReplacementMaterialId != null && material?.IsDisable != true;
                itemSetReplacement.Enabled = material != null && material.IsDisable != true;
                itemViewReplacementChain.Enabled = material != null;
                itemEnable.BeginGroup = true;
                itemDisable.BeginGroup = true;
                itemMultiselect.BeginGroup = true;

                DXSubMenuItem replacementMenu = new DXSubMenuItem("替代設定") { SvgImage = SvgIconCatalog.Transfer };
                replacementMenu.ImageOptions.SvgImageSize = new Size(24, 24);
                replacementMenu.Items.Add(itemViewReplacementChain);
                replacementMenu.Items.Add(itemSetReplacement);
                replacementMenu.Items.Add(itemClearReplacement);

                DXSubMenuItem photoMenu = new DXSubMenuItem("圖片") { SvgImage = SvgIconCatalog.Attachment };
                photoMenu.ImageOptions.SvgImageSize = new Size(24, 24);

                e.Menu.Items.Add(itemViewInfo);
                e.Menu.Items.Add(replacementMenu);
                if (material?.IsDisable == true)
                {
                    photoMenu.Items.Add(itemViewPhoto);
                    e.Menu.Items.Add(photoMenu);
                    e.Menu.Items.Add(itemEnable);
                }
                else
                {
                    photoMenu.Items.Add(itemUploadPhoto);
                    photoMenu.Items.Add(itemViewPhoto);
                    e.Menu.Items.Add(photoMenu);
                    e.Menu.Items.Add(itemUpdatePrice);

                    DXSubMenuItem dXSubMenuReports = new DXSubMenuItem("庫存作業") { SvgImage = SvgIconCatalog.UserTransfer };
                    dXSubMenuReports.ImageOptions.SvgImageSize = new Size(24, 24);

                    dXSubMenuReports.Items.Add(itemMaterialIn);
                    dXSubMenuReports.Items.Add(itemMaterialOut);
                    dXSubMenuReports.Items.Add(itemMaterialTransfer);
                    dXSubMenuReports.Items.Add(itemMaterialGetFromOther);
                    dXSubMenuReports.BeginGroup = true;

                    e.Menu.Items.Add(dXSubMenuReports);

                    e.Menu.Items.Add(itemDisable);
                }

                e.Menu.Items.Add(itemMultiselect);
                e.Menu.Items.Add(itemPrintStamp);
            }
        }

        private void gvData_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            if (e.IsGetData && e.Column.FieldName == "TotalPrice")
            {
                SparePartMaterial currentRow = ((dynamic)e.Row).data as SparePartMaterial;
                if (currentRow == null) return;

                double sumQuantity = Convert.ToDouble(currentRow.QuantityInStorage + currentRow.QuantityInMachine);

                // Tạo một mảng các chuỗi và chỉ lấy các chuỗi không rỗng
                e.Value = sumQuantity * currentRow.Price;
            }
        }

        private void gvData_MasterRowEmpty(object sender, MasterRowEmptyEventArgs e)
        {
            e.IsEmpty = false;
        }

        private void gvData_MasterRowGetRelationCount(object sender, MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = 3;
        }

        private void gvData_MasterRowGetRelationName(object sender, MasterRowGetRelationNameEventArgs e)
        {
            switch (e.RelationIndex)
            {
                case 0:
                    e.RelationName = "出入庫記錄";
                    break;
                case 1:
                    e.RelationName = "單價管理";
                    break;
                case 2:
                    e.RelationName = "用於設備";
                    break;
                    //case 3:
                    //    e.RelationName = "單價管理";
                    //    break;
            }
        }

        private void gvData_MasterRowGetChildList(object sender, MasterRowGetChildListEventArgs e)
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                GridView view = sender as GridView;
                SparePartMaterial material = (view.GetRow(e.RowHandle) as dynamic).data as SparePartMaterial;
                int idMaterial = material.Id;

                if (material != null)
                {
                    switch (e.RelationIndex)
                    {
                        case 0:

                            var transactions = SparePartTransactionService.Instance.GetListByidMaterial(idMaterial).Select(r => new
                            {
                                data = r,
                                Event = events[r.TransactionType],
                                UserDo = users.FirstOrDefault(u => u.Id == r.UserDo)?.DisplayName,
                                Starage = storages.FirstOrDefault(u => u.Id == r.StorageId)?.DisplayName,
                            }).OrderByDescending(r => r.data.Id).ToList();
                            e.ChildList = transactions;

                            break;
                        case 1:

                            var prices = SparePartPriceService.Instance.GetListByIdMaterial(idMaterial).Select(r => new
                            {
                                r.Price,
                                r.ChangedAt,
                                ChangedBy = users.FirstOrDefault(x => x.Id == r.ChangedBy).DisplayName
                            }).ToList();

                            e.ChildList = prices;

                            break;
                        case 2:

                            var ids = SparePartMachineMaterialService.Instance.GetListByIdMaterial(idMaterial).Select(r => r.MachineId).ToList();
                            var machines = SparePartMachineService.Instance.GetListByIds(ids);

                            e.ChildList = machines;

                            break;
                            //case 3:
                            //    List<Prices> lsPrices = PricesDAO.Instance.GetListByIdSpare(material.IdSparePart);
                            //    e.ChildList = lsPrices.OrderByDescending(r => r.PriceTime).ToList();
                            //    break;
                    }
                }
            }
        }

        private void gvData_MasterRowExpanded(object sender, CustomMasterRowEventArgs e)
        {
            GridView masterView = sender as GridView;
            int visibleDetailRelationIndex = masterView.GetVisibleDetailRelationIndex(e.RowHandle);
            GridView detailView = masterView.GetDetailView(e.RowHandle, visibleDetailRelationIndex) as GridView;

            detailView.BestFitColumns();
        }

        public static List<int> GetVisibleDataIds(GridView view)
        {
            var ids = new List<int>();

            for (int i = 0; i < view.DataRowCount; i++)
            {
                int rowHandle = view.GetVisibleRowHandle(i);

                // Cách nhanh: nếu có cột/field "data.Id" trong Grid (cột ẩn cũng được)
                var cell = view.GetRowCellValue(rowHandle, "data.Id");
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

        private void ExportExcelBySpare(bool _isNotify = false)
        {
            var ids = GetVisibleDataIds(gvData);
            var materialsRechecking = SparePartInspectionItemService.Instance.GetListRechecking().Select(r => r.MaterialId).ToList();

            var idsSet = new HashSet<int>(ids);
            var recheckingSet = new HashSet<int>(materialsRechecking);

            var filteredMaterials = materials
                .Where(r => idsSet.Contains(r.Id)) // chỉ lấy các item có Id nằm trong ids
                .Where(r => !recheckingSet.Contains(r.Id)) // loại bỏ item đang rechecking
                .Where(r => !_isNotify || (r.QuantityInStorage + r.QuantityInMachine) < r.MinQuantity) // điều kiện notify
                .ToList();

            var excelDatas = filteredMaterials
                .OrderBy(x => x.IdDept)
                .Select(x => new
                {
                    部門 = x.IdDept,
                    材料編號 = x.Code,
                    品名規格 = x.DisplayName,
                    備品用途 = x.TypeUse,
                    料位 = x.Location,
                    單位 = units.FirstOrDefault(u => u.Id == x.IdUnit)?.DisplayName,
                    安全數量 = x.MinQuantity,
                    課庫數量 = x.QuantityInStorage,
                    機邊庫數量 = x.QuantityInMachine,
                    單價 = x.Price,
                    總金額 = x.Price * (x.QuantityInMachine + x.QuantityInStorage),
                    管理員 = users.FirstOrDefault(u => u.Id == x.IdManager)?.DisplayName,
                })
                .ToList();

            string documentsPath = SparePartConfiguration.DocumentPath();
            if (!Directory.Exists(documentsPath))
                Directory.CreateDirectory(documentsPath);

            string filePath = Path.Combine(documentsPath, $"備品管理-{(_isNotify ? "提醒備品" : "按照備品")} - {DateTime.Now:yyyyMMddHHmmss}.xlsx");

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (ExcelPackage pck = new ExcelPackage(filePath))
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Sheet1");
                ws.Cells.Style.Font.Name = "Microsoft JhengHei";
                ws.Cells.Style.Font.Size = 14;

                // Thiết lập độ rộng các cột
                ws.Column(1).Width = 30;
                ws.Column(2).Width = 30;
                ws.Column(3).Width = 50;
                ws.Column(4).Width = 35;
                ws.Column(5).Width = 35;
                ws.Column(6).Width = 35;
                ws.Column(7).Width = 35;
                ws.Column(8).Width = 30;

                // Xuất dữ liệu từ list excelDatas sang Table
                ws.Cells["A1"].LoadFromCollection(excelDatas, true, OfficeOpenXml.Table.TableStyles.Medium2);
                // Bật WrapText cho tất cả các ô
                ws.Column(3).Style.WrapText = true;
                ws.Cells[ws.Dimension.Address].AutoFitColumns();
                ws.Column(3).Width = 35;

                // Lưu file
                pck.Save();
            }

            Process.Start(filePath);
        }

        private void btnExcelBySpare_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ExportExcelBySpare();
        }

        private void btnExcelByNotify_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ExportExcelBySpare(true);
        }

        private void btnExcelByMachine_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var ids = GetVisibleDataIds(gvData);

            var machines = SparePartMachineService.Instance.GetListByStartIdDept(deptGetData);
            var machineMaterials = SparePartMachineMaterialService.Instance.GetList().Where(r => ids.Contains(r.MaterialId)).ToList();

            // Tạo danh sách dạng phẳng: mỗi dòng là 1 vật liệu gắn với máy
            var excelDatas = machines.SelectMany(machine =>
            {
                var machineMaterialList = machineMaterials
                    .Where(mm => mm.MachineId == machine.Id)
                    .Join(materials,
                          mm => mm.MaterialId,
                          m => m.Id,
                          (mm, m) => m)
                    .ToList();

                double totalPrice = machineMaterialList.Sum(m =>
                    Convert.ToDouble(m.Price) *
                    (Convert.ToDouble(m.QuantityInStorage) + Convert.ToDouble(m.QuantityInMachine)));

                return machineMaterialList.Select(m => new
                {
                    部門 = machine.IdDept,
                    設備名稱 = machine.DisplayName,
                    設備位置 = machine.Location,
                    所屬機台總價值 = totalPrice,
                    材料編號 = m.Code,
                    品名規格 = m.DisplayName,
                    備品用途 = m.TypeUse,
                    料位 = m.Location,
                    單位 = units.FirstOrDefault(u => u.Id == m.IdUnit)?.DisplayName ?? "",
                    安全數量 = m.MinQuantity,
                    課庫數量 = m.QuantityInStorage,
                    機邊庫數量 = m.QuantityInMachine,
                    單價 = m.Price,
                    總金額 = m.Price * (m.QuantityInMachine + m.QuantityInStorage),
                    管理員 = users.FirstOrDefault(u => u.Id == m.IdManager)?.DisplayName ?? "",
                });
            }).ToList();

            string documentsPath = SparePartConfiguration.DocumentPath();
            if (!Directory.Exists(documentsPath))
                Directory.CreateDirectory(documentsPath);

            string filePath = Path.Combine(documentsPath, $"備品管理-按照設備 - {DateTime.Now:yyyyMMddHHmmss}.xlsx");

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (ExcelPackage pck = new ExcelPackage(filePath))
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Sheet1");
                ws.Cells.Style.Font.Name = "Microsoft JhengHei";
                ws.Cells.Style.Font.Size = 14;

                // Xuất dữ liệu từ list excelDatas sang Table
                ws.Cells["A1"].LoadFromCollection(excelDatas, true);

                int startRow = 2, endRow = excelDatas.Count + 1;

                // Merge các cột A-D nếu giá trị giống nhau
                for (int col = 1; col <= 4; col++)
                {
                    int mergeStart = startRow;
                    for (int row = startRow + 1; row <= endRow + 1; row++)
                    {
                        var curr = ws.Cells[row, col].Text;
                        var prev = ws.Cells[mergeStart, col].Text;

                        if (curr != prev || row == endRow + 1)
                        {
                            if (row - 1 > mergeStart)
                                ws.Cells[mergeStart, col, row - 1, col].Merge = true;
                            mergeStart = row;
                        }
                    }
                }

                // Bật WrapText cho tất cả các ô
                ws.Column(6).Style.WrapText = true;
                ws.Cells[ws.Dimension.Address].AutoFitColumns();
                ws.Column(6).Width = 35;

                // Định dạng cột D (giả sử D là cột số lượng chẳng hạn)
                ws.Column(4).Style.Numberformat.Format = "#,##0"; // D = 4
                ws.Column(13).Style.Numberformat.Format = "#,##0"; // M = 13
                ws.Column(14).Style.Numberformat.Format = "#,##0"; // N = 14

                // Căn giữa và kẻ ô toàn bảng
                var fullRange = ws.Cells[ws.Dimension.Address];
                fullRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                fullRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                fullRange.Style.Border.Top.Style = fullRange.Style.Border.Bottom.Style =
                    fullRange.Style.Border.Left.Style = fullRange.Style.Border.Right.Style =
                    ExcelBorderStyle.Thin;

                // Lưu file
                pck.Save();
            }

            Process.Start(filePath);
        }
    }
}
