using Winform4System.Business.Services.SpareParts;
using Winform4System.DataAccess.Entities.SpareParts;
using DevExpress.Data;
using DevExpress.Security;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Localization;
using DevExpress.XtraGrid.Menu;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraSplashScreen;
using ExcelDataReader;
using Winform4System.Forms.SpareParts;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Drawing.Chart.Style;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Util;
using System.Windows.Forms;

using Winform4System.Helpers;

namespace Winform4System.Forms.SpareParts
{
    public partial class SparePartRecheckView : DevExpress.XtraEditors.XtraUserControl
    {
        public SparePartRecheckView()
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
        bool canApproveRecovery = false;

        List<SparePartUser> users = new List<SparePartUser>();

        List<SparePartMaterial> materials;
        List<SparePartInspectionItem> inspectionBatchMaterials;
        List<SparePartUnit> units;

        DXMenuItem itemDownCheckFile;
        DXMenuItem itemDownCheckFileProxy;
        DXMenuItem itemUpdateCheckFile;
        DXMenuItem itemUpdateCheckFileProxy;
        DXMenuItem itemDownUnusualFile;
        DXMenuItem itemMaterialOut;
        DXMenuItem itemMaterialTransfer;
        DXMenuItem itemMaterialCheck;
        DXMenuItem itemMaterialGetFromOther;
        DXMenuItem itemViewCheckPhoto;

        static readonly string[] CheckPhotoColumnAliases = new[]
        {
            "圖片名稱",
            "图片名称",
            "照片名稱",
            "照片名称",
            "ImageName",
            "PhotoName",
            "PhotoFile",
            "Tên ảnh",
            "TênẢnh",
            "TenAnh",
            "Ảnh",
            "Anh"
        };

        static readonly HashSet<string> AllowedCheckPhotoExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg",
            ".jpeg",
            ".png"
        };

        private void InitializeIcon()
        {
            btnReload.ImageOptions.SvgImage = SvgImageCatalog.Reload;
            btnExportExcel.ImageOptions.SvgImage = SvgImageCatalog.Excel;
            barCbbDept.ImageOptions.SvgImage = SvgImageCatalog.Dept;
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

            var ruleNotify = new GridFormatRule
            {
                ApplyToRow = true,
                Name = "RuleNotify",
                Rule = new FormatConditionRuleExpression
                {
                    Expression = "[BatchCancelled] != true And [BatchMaterial.IsComplete] != true",
                    Appearance = { ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Critical }
                }
            };
            gvSparePart.FormatRules.Add(ruleNotify);
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
            itemDownCheckFile = CreateMenuItem("下載盤點表", ItemDownCheckFile_Click, SvgImageCatalog.Excel);
            itemUpdateCheckFile = CreateMenuItem("上傳盤點表", ItemUpdateCheckFile_Click, SvgImageCatalog.UploadFile);

            itemDownCheckFileProxy = CreateMenuItem("下載盤點表(代理)", ItemDownCheckFileProxy_Click, SvgImageCatalog.Excel);
            itemUpdateCheckFileProxy = CreateMenuItem("上傳盤點表(代理)", ItemUpdateCheckFileProxy_Click, SvgImageCatalog.UploadFile);
            itemViewCheckPhoto = CreateMenuItem("查看圖片", ItemViewCheckPhoto_Click, SvgImageCatalog.Search);
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

        private void ItemUpdateCheckFileProxy_Click(object sender, EventArgs e)
        {
            bool isUploadAbnormal = (sender as DXMenuItem).Caption.Contains("異常");
            UpdateCheckFile(isUploadAbnormal, true);
        }

        private void ItemDownCheckFileProxy_Click(object sender, EventArgs e)
        {
            bool isUploadAbnormal = (sender as DXMenuItem).Caption.Contains("異常");
            DownloadCheckFile(isUploadAbnormal, true);
        }

        private string GetProxyUser(List<string> proxyusers)
        {
            var editor = new ComboBoxEdit()
            {
                Font = new System.Drawing.Font("Microsoft JhengHei UI", 14F)
            };
            editor.Properties.AppearanceDropDown.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14F);
            editor.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            editor.Properties.Items.AddRange(proxyusers);

            var args = new XtraInputBoxArgs
            {
                Caption = SparePartConfiguration.SoftNameTW,
                Prompt = "點選《管理員》", // dùng ngoặc đẹp hơn
                Editor = editor,
                DefaultButtonIndex = 0,
                DefaultResponse = proxyusers.FirstOrDefault() ?? ""
            };

            var result = XtraInputBox.Show(args);

            return result?.ToString() ?? "";
        }

        private void ItemUpdateCheckFile_Click(object sender, EventArgs e)
        {
            bool isUploadAbnormal = (sender as DXMenuItem).Caption.Contains("異常");
            bool isManagerReCheck = (sender as DXMenuItem).Caption.StartsWith("【管理】");
            UpdateCheckFile(isUploadAbnormal, isManagerReCheck);
        }

        private void ItemDownCheckFile_Click(object sender, EventArgs e)
        {
            bool isUploadAbnormal = (sender as DXMenuItem).Caption.Contains("異常");
            bool isManagerReCheck = (sender as DXMenuItem).Caption.StartsWith("【管理】");
            DownloadCheckFile(isUploadAbnormal, isManagerReCheck);
        }

        private DataColumn FindExcelColumn(DataTable table, params string[] aliases)
        {
            if (table == null || aliases == null || aliases.Length == 0)
            {
                return null;
            }

            HashSet<string> normalizedAliases = new HashSet<string>(
                aliases
                    .Where(alias => !string.IsNullOrWhiteSpace(alias))
                    .Select(NormalizeExcelHeader));

            return table.Columns
                .Cast<DataColumn>()
                .FirstOrDefault(column => normalizedAliases.Contains(NormalizeExcelHeader(column.ColumnName)));
        }

        private string NormalizeExcelHeader(string header)
        {
            return (header ?? string.Empty)
                .Trim()
                .Replace(" ", string.Empty)
                .Replace("　", string.Empty)
                .Replace("_", string.Empty)
                .Replace("-", string.Empty)
                .ToUpperInvariant();
        }

        private string GetExcelCellString(DataRow row, DataColumn column)
        {
            if (row == null || column == null)
            {
                return string.Empty;
            }

            object value = row[column];
            return value == null || value == DBNull.Value
                ? string.Empty
                : value.ToString().Trim();
        }

        private Dictionary<int, string> ReadExcelHyperlinks(string excelFilePath, int columnIndex, IEnumerable<int> rowNumbers)
        {
            var hyperlinks = new Dictionary<int, string>();

            try
            {
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                using (var stream = File.Open(excelFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null)
                    {
                        return hyperlinks;
                    }

                    foreach (int rowNumber in rowNumbers.Distinct())
                    {
                        Uri hyperlink = worksheet.Cells[rowNumber, columnIndex].Hyperlink;
                        if (hyperlink != null)
                        {
                            hyperlinks[rowNumber] = hyperlink.OriginalString;
                        }
                    }
                }
            }
            catch
            {
                // Hyperlink là dữ liệu bổ sung; vẫn cho phép xử lý giá trị hiển thị trong ô.
            }

            return hyperlinks;
        }

        private string GetPhotoFileName(string reference)
        {
            string value = (reference ?? string.Empty).Trim().Trim('"');
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            if (Uri.TryCreate(value, UriKind.Absolute, out Uri uri)
                && (uri.IsFile || uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            {
                return Path.GetFileName(uri.IsFile ? uri.LocalPath : uri.AbsolutePath);
            }

            return Path.GetFileName(value.Replace('/', Path.DirectorySeparatorChar));
        }

        private bool TryGetLocalPhotoPath(string excelFilePath, string reference, out string localPath, out bool isRemoteUrl)
        {
            localPath = string.Empty;
            isRemoteUrl = false;

            string value = (reference ?? string.Empty).Trim().Trim('"');
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            try
            {
                if (Uri.TryCreate(value, UriKind.Absolute, out Uri uri))
                {
                    if (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
                    {
                        isRemoteUrl = true;
                        return false;
                    }

                    if (uri.IsFile)
                    {
                        localPath = uri.LocalPath;
                        return true;
                    }
                }

                localPath = Path.IsPathRooted(value)
                    ? Path.GetFullPath(value)
                    : Path.GetFullPath(Path.Combine(Path.GetDirectoryName(excelFilePath) ?? string.Empty, value));
                return true;
            }
            catch
            {
                localPath = string.Empty;
                return false;
            }
        }

        private bool TryResolveCheckPhoto(
            string excelFilePath,
            string displayedValue,
            string hyperlinkValue,
            out string photoName,
            out string photoPath,
            out string errorMessage)
        {
            photoName = string.Empty;
            photoPath = string.Empty;
            errorMessage = string.Empty;

            var references = new[] { displayedValue, hyperlinkValue }
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (references.Count == 0)
            {
                errorMessage = "圖片名稱或連結不可為空。";
                return false;
            }

            string imagesFolder = Path.Combine(Path.GetDirectoryName(excelFilePath) ?? string.Empty, "images");
            var fileNames = references
                .Select(GetPhotoFileName)
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            foreach (string fileName in fileNames)
            {
                if (!AllowedCheckPhotoExtensions.Contains(Path.GetExtension(fileName)))
                {
                    continue;
                }

                string imageFolderPath = Path.Combine(imagesFolder, fileName);
                if (File.Exists(imageFolderPath))
                {
                    photoName = fileName;
                    photoPath = imageFolderPath;
                    return true;
                }
            }

            bool hasRemoteUrl = false;
            foreach (string reference in new[] { hyperlinkValue, displayedValue })
            {
                if (!TryGetLocalPhotoPath(excelFilePath, reference, out string localPath, out bool isRemoteUrl))
                {
                    hasRemoteUrl |= isRemoteUrl;
                    continue;
                }

                string localFileName = Path.GetFileName(localPath);
                if (!AllowedCheckPhotoExtensions.Contains(Path.GetExtension(localFileName)) || !File.Exists(localPath))
                {
                    continue;
                }

                photoName = localFileName;
                photoPath = localPath;
                return true;
            }

            if (fileNames.Count > 0 && fileNames.All(fileName => !AllowedCheckPhotoExtensions.Contains(Path.GetExtension(fileName))))
            {
                errorMessage = "圖片副檔名只支援 .jpg、.jpeg、.png。";
            }
            else if (hasRemoteUrl)
            {
                errorMessage = "不支援網路圖片連結，請使用本機檔案或 images 資料夾。";
            }
            else
            {
                errorMessage = $"找不到圖片檔案「{fileNames.FirstOrDefault() ?? displayedValue}」，請確認路徑或 images 資料夾。";
            }

            return false;
        }

        private string NormalizeMaterialCode(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return string.Empty;
            }

            string text = value.ToString().Trim();
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            if (long.TryParse(text, out long longValue))
            {
                return longValue.ToString();
            }

            if (double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out double invariantDouble))
            {
                return Math.Abs(invariantDouble % 1) < 0.000001
                    ? Convert.ToInt64(invariantDouble).ToString()
                    : invariantDouble.ToString(CultureInfo.InvariantCulture);
            }

            if (double.TryParse(text, NumberStyles.Any, CultureInfo.CurrentCulture, out double currentDouble))
            {
                return Math.Abs(currentDouble % 1) < 0.000001
                    ? Convert.ToInt64(currentDouble).ToString()
                    : currentDouble.ToString(CultureInfo.CurrentCulture);
            }

            return text;
        }

        private bool TryParseExcelDouble(object value, out double result)
        {
            result = 0;

            if (value == null || value == DBNull.Value)
            {
                return false;
            }

            if (value is double doubleValue)
            {
                result = doubleValue;
                return true;
            }

            if (value is float floatValue)
            {
                result = floatValue;
                return true;
            }

            if (value is decimal decimalValue)
            {
                result = (double)decimalValue;
                return true;
            }

            string text = value.ToString().Trim();
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            return double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out result)
                || double.TryParse(text, NumberStyles.Any, CultureInfo.CurrentCulture, out result);
        }

        private void ShowImportErrors(IEnumerable<string> errors, string title)
        {
            if (errors == null)
            {
                return;
            }

            List<string> errorList = errors
                .Where(error => !string.IsNullOrWhiteSpace(error))
                .Distinct()
                .ToList();

            if (errorList.Count == 0)
            {
                return;
            }

            const int maxDisplay = 12;
            string message = string.Join(Environment.NewLine, errorList.Take(maxDisplay));
            if (errorList.Count > maxDisplay)
            {
                message += Environment.NewLine + $"... 尚有 {errorList.Count - maxDisplay} 筆錯誤";
            }

            XtraMessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private bool TryReadCheckUploadData(
            string excelFilePath,
            DataTable table,
            List<SparePartInspectionItem> excelDatas,
            bool isUploadAbnormal,
            out Dictionary<int, string> inspectionPhotoNames,
            out Dictionary<int, string> inspectionPhotoPaths)
        {
            inspectionPhotoNames = new Dictionary<int, string>();
            inspectionPhotoPaths = new Dictionary<int, string>();

            if (table == null)
            {
                XtraMessageBox.Show("Excel 內容為空，請確認檔案後再試。", SparePartConfiguration.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            DataColumn codeColumn = FindExcelColumn(table, "編碼", "编码", "MaterialId", "MaterialID", "Id");
            if (codeColumn == null)
            {
                XtraMessageBox.Show("Excel 缺少「編碼」欄位，請使用系統下載的模板。", SparePartConfiguration.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            DataColumn quantityColumn = isUploadAbnormal ? null : FindExcelColumn(table, "盤點數量", "盘点数量", "ActualQuantity");
            DataColumn descriptionColumn = isUploadAbnormal ? FindExcelColumn(table, "異常說明", "异常说明", "Description") : null;
            DataColumn photoColumn = isUploadAbnormal ? null : FindExcelColumn(table, CheckPhotoColumnAliases);

            if (!isUploadAbnormal && quantityColumn == null)
            {
                XtraMessageBox.Show("Excel 缺少「盤點數量」欄位，請使用系統下載的模板。", SparePartConfiguration.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!isUploadAbnormal && photoColumn == null)
            {
                XtraMessageBox.Show("Excel 缺少「圖片名稱」欄位，請使用系統下載的模板。", SparePartConfiguration.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (isUploadAbnormal && descriptionColumn == null)
            {
                XtraMessageBox.Show("Excel 缺少「異常說明」欄位，請使用系統下載的模板。", SparePartConfiguration.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            var rowInfos = table.AsEnumerable()
                .Select((row, index) => new
                {
                    Row = row,
                    RowNumber = index + 2,
                    Code = NormalizeMaterialCode(row[codeColumn])
                })
                .Where(x => !string.IsNullOrWhiteSpace(x.Code))
                .ToList();

            Dictionary<int, string> photoHyperlinks = isUploadAbnormal
                ? new Dictionary<int, string>()
                : ReadExcelHyperlinks(excelFilePath, photoColumn.Ordinal + 1, rowInfos.Select(x => x.RowNumber));

            List<string> errors = new List<string>();

            var duplicateCodes = rowInfos
                .GroupBy(x => x.Code)
                .Where(group => group.Count() > 1)
                .ToList();

            foreach (var duplicate in duplicateCodes)
            {
                errors.Add($"編碼 {duplicate.Key} 重複出現在第 {string.Join("、", duplicate.Select(x => x.RowNumber))} 列。");
            }

            if (errors.Count > 0)
            {
                ShowImportErrors(errors, "Excel 檢查失敗");
                return false;
            }

            var rowInfoByCode = rowInfos.ToDictionary(x => x.Code, x => x);

            foreach (var data in excelDatas)
            {
                if (!rowInfoByCode.TryGetValue(data.MaterialId.ToString(), out var rowInfo))
                {
                    errors.Add($"找不到編碼 {data.MaterialId} 的資料列。");
                    continue;
                }

                if (isUploadAbnormal)
                {
                    data.Description = GetExcelCellString(rowInfo.Row, descriptionColumn);
                    continue;
                }

                object quantityValue = rowInfo.Row[quantityColumn];
                string quantityText = GetExcelCellString(rowInfo.Row, quantityColumn);
                if (TryParseExcelDouble(quantityValue, out double actualQty))
                {
                    data.ActualQuantity = actualQty;
                }
                else if (!string.IsNullOrWhiteSpace(quantityText))
                {
                    errors.Add($"第 {rowInfo.RowNumber} 列：盤點數量格式不正確。");
                }

                string displayedPhotoValue = GetExcelCellString(rowInfo.Row, photoColumn);
                photoHyperlinks.TryGetValue(rowInfo.RowNumber, out string hyperlinkValue);

                if (!TryResolveCheckPhoto(
                    excelFilePath,
                    displayedPhotoValue,
                    hyperlinkValue,
                    out string photoFileName,
                    out string photoPath,
                    out string photoError))
                {
                    errors.Add($"第 {rowInfo.RowNumber} 列：{photoError}");
                    continue;
                }

                inspectionPhotoNames[data.Id] = photoFileName;
                inspectionPhotoPaths[data.Id] = photoPath;
            }

            if (errors.Count > 0)
            {
                ShowImportErrors(errors, "Excel 檢查失敗");
                return false;
            }

            return true;
        }

        private bool ReplaceInspectionCheckPhoto(int batchMaterialId, string sourceFilePath)
        {
            if (string.IsNullOrWhiteSpace(sourceFilePath) || !File.Exists(sourceFilePath))
            {
                return false;
            }

            string thread = SparePartInspectionPhotoHelper.GetThread(batchMaterialId);

            try
            {
                var oldAttachments = SparePartInspectionAttachmentService.Instance.GetListByThread(thread)
                    .OrderByDescending(x => x.Id)
                    .ToList();

                var savedPhoto = SparePartInspectionPhotoHelper.SavePhoto(batchMaterialId, sourceFilePath);
                var newAttachment = new SparePartInspectionAttachment
                {
                    Thread = thread,
                    EncryptionName = savedPhoto.encryptionName,
                    ActualName = savedPhoto.actualName
                };

                int newAttachmentId = SparePartInspectionAttachmentService.Instance.Add(newAttachment);
                if (newAttachmentId <= 0)
                {
                    SparePartInspectionPhotoHelper.DeletePhotoFile(batchMaterialId, newAttachment);
                    return false;
                }

                foreach (var oldAttachment in oldAttachments)
                {
                    try
                    {
                        SparePartInspectionPhotoHelper.DeletePhotoFile(batchMaterialId, oldAttachment);
                        SparePartInspectionAttachmentService.Instance.RemoveById(oldAttachment.Id);
                    }
                    catch
                    {
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void UpdateCheckFile(bool IsUploadAbnormal = false, bool managerRecheck = false)
        {
            var batch = (gvData.GetRow(gvData.FocusedRowHandle) as dynamic).Batch as SparePartInspectionBatch;
            if (IsBatchCancelled(batch))
            {
                XtraMessageBox.Show("已取消批次不可再上傳盤點資料。", SparePartConfiguration.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int batchId = batch.Id;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Excel Files|*.xls;*.xlsx";
                openFileDialog.Title = "Chọn tệp Excel";

                if (openFileDialog.ShowDialog() != DialogResult.OK) return;

                string filePath = openFileDialog.FileName;
                try
                {
                    using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration
                            {
                                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                                {
                                    UseHeaderRow = true
                                }
                            });

                            if (result.Tables.Count == 0)
                            {
                                XtraMessageBox.Show("Excel 內容為空，請確認檔案後再試。", SparePartConfiguration.SoftNameTW,
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            DataTable table = result.Tables[0];

                            var excelDatas = inspectionBatchMaterials
                                .Where(r => r.BatchId == batchId && r.IsComplete != true)
                                .Join(
                                    materials.Where(x => x.IdDept.StartsWith(deptGetData)
                                    && (managerRecheck ? true : x.IdManager == SparePartConfiguration.LoginUser.Id)),
                                    bm => bm.MaterialId,
                                    m => m.Id,
                                    (bm, m) => bm
                                )
                                .ToList();

                            if (!TryReadCheckUploadData(filePath, table, excelDatas, IsUploadAbnormal,
                                out Dictionary<int, string> inspectionPhotoNames,
                                out Dictionary<int, string> inspectionPhotoPaths))
                            {
                                return;
                            }

                            SparePartRecheckForm reCheckInfo = new SparePartRecheckForm
                            {
                                InspectionBatchMaterials = excelDatas.ToList(),
                                Text = $"物料盤點明細表",
                                _IsUploadAbnormal = IsUploadAbnormal,
                                InspectionPhotoNames = inspectionPhotoNames,
                                InspectionPhotoPaths = inspectionPhotoPaths
                            };
                            reCheckInfo.ShowDialog();

                            bool _isChecked = reCheckInfo._isChecked;
                            if (!_isChecked)
                            {
                                LoadData();
                                return;
                            }

                            if (IsUploadAbnormal)
                            {
                                List<string> saveErrors = new List<string>();
                                foreach (var data in excelDatas.Where(r => r.ActualQuantity != null && !string.IsNullOrEmpty(r.Description)))
                                {
                                    data.ConfirmationDate = DateTime.Today;
                                    data.ConfirmedBy = SparePartConfiguration.LoginUser.Id;
                                    data.IsComplete = !string.IsNullOrEmpty(data.Description);
                                    if (!SparePartInspectionItemService.Instance.AddOrUpdate(data))
                                    {
                                        saveErrors.Add($"材料編碼 {data.MaterialId}: 異常資料保存失敗。");
                                        continue;
                                    }

                                    SparePartTransactionService.Instance.Add(new SparePartTransaction()
                                    {
                                        CreatedDate = DateTime.Now,
                                        MaterialId = data.MaterialId,
                                        TransactionType = "check",
                                        Quantity = (double)data.ActualQuantity,
                                        UserDo = SparePartConfiguration.LoginUser.Id,
                                        Desc = $"定期盤點，異常說明：{data.Description}",
                                        StorageId = 1
                                    });
                                }

                                LoadData();
                                ShowImportErrors(saveErrors, "部分資料處理失敗");
                            }
                            else
                            {
                                List<string> saveErrors = new List<string>();
                                foreach (var data in excelDatas.Where(r => r.ActualQuantity != null))
                                {
                                    bool isPhotoSaved = inspectionPhotoPaths.TryGetValue(data.Id, out string photoPath)
                                        && ReplaceInspectionCheckPhoto(data.Id, photoPath);

                                    data.ConfirmationDate = DateTime.Today;
                                    data.ConfirmedBy = SparePartConfiguration.LoginUser.Id;
                                    data.IsComplete = isPhotoSaved && data.InitialQuantity == data.ActualQuantity;

                                    if (!isPhotoSaved)
                                    {
                                        saveErrors.Add($"材料編碼 {data.MaterialId}: 圖片保存失敗。");
                                    }

                                    if (!SparePartInspectionItemService.Instance.AddOrUpdate(data))
                                    {
                                        saveErrors.Add($"材料編碼 {data.MaterialId}: 盤點資料保存失敗。");
                                    }
                                }

                                LoadData();
                                ShowImportErrors(saveErrors, "部分資料處理失敗");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show("匯入失敗：" + ex.Message, SparePartConfiguration.SoftNameTW,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DownloadCheckFile(bool IsUploadAbnormal = false, bool managerRecheck = false)
        {
            var batch = (gvData.GetRow(gvData.FocusedRowHandle) as dynamic).Batch as SparePartInspectionBatch;
            if (IsBatchCancelled(batch))
            {
                XtraMessageBox.Show("已取消批次不可再下載盤點表。", SparePartConfiguration.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int batchId = batch.Id;

            var excelDatas = inspectionBatchMaterials
                .Where(r => r.BatchId == batchId && r.IsComplete != true)
                .Join(
                    materials.Where(x => deptGetData.Contains(x.IdDept)),
                    bm => bm.MaterialId,
                    m => m.Id,
                    (bm, m) => new
                    {
                        Material = m,
                        BatchMaterial = bm
                    }
                )
                .Select(r =>
                {
                    var manager = users.FirstOrDefault(x => x.Id == r.Material.IdManager);
                    return new
                    {
                        r.Material.Id,
                        r.Material.Code,
                        r.Material.DisplayName,
                        r.Material.Location,
                        Usermanager = manager != null ? $"{manager.Id} {manager.DisplayName}" : "",
                        Unit = units.FirstOrDefault(x => x.Id == r.Material.IdUnit)?.DisplayName ?? "",
                        Quantity = IsUploadAbnormal ? r.BatchMaterial.InitialQuantity : -1,
                        r.BatchMaterial.ActualQuantity,
                        Desc = "",
                        PhotoName = ""
                    };
                })
                .ToList();

            string userManagerSpare = $"{SparePartConfiguration.LoginUser.Id} {SparePartConfiguration.LoginUser.DisplayName}";
            if (!managerRecheck)
            {
                excelDatas = excelDatas.Where(r => r.Usermanager == userManagerSpare).ToList();
            }

            if (excelDatas.Count == 0)
            {
                XtraMessageBox.Show("未找到適用於您的盤點清單", "提示");
                return;
            }

            string documentsPath = SparePartConfiguration.DocumentPath();
            if (!Directory.Exists(documentsPath))
                Directory.CreateDirectory(documentsPath);

            string exportFolder = Path.Combine(
                documentsPath,
                $"{(IsUploadAbnormal ? "異常表" : "盤點表")}-{(managerRecheck ? "管理" : userManagerSpare)}-{DateTime.Now:yyyyMMddHHmmss}");

            if (!Directory.Exists(exportFolder))
            {
                Directory.CreateDirectory(exportFolder);
            }

            if (!IsUploadAbnormal)
            {
                Directory.CreateDirectory(Path.Combine(exportFolder, "images"));
            }

            string filePath = Path.Combine(exportFolder, $"{(IsUploadAbnormal ? "異常表" : "盤點表")}.xlsx");

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            using (ExcelPackage pck = new ExcelPackage(filePath))
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Sheet1");
                ws.Cells.Style.Font.Name = "Microsoft JhengHei";
                ws.Cells.Style.Font.Size = 14;

                ws.Cells["A1"].LoadFromCollection(excelDatas, true, OfficeOpenXml.Table.TableStyles.Medium2);

                ws.Cells["A1"].Value = "編碼";
                ws.Cells["B1"].Value = "料號";
                ws.Cells["C1"].Value = "材料名稱";
                ws.Cells["D1"].Value = "地位";
                ws.Cells["E1"].Value = "管理員";
                ws.Cells["F1"].Value = "單位";

                if (IsUploadAbnormal)
                {
                    // Bảng bất thường chỉ cần số hệ thống, số kiểm kê và giải thích.
                    ws.DeleteColumn(10);
                    ws.Cells["G1"].Value = "系統上數量";
                    ws.Cells["H1"].Value = "盤點數量";
                    ws.Cells["I1"].Value = "異常說明";
                }
                else
                {
                    // Kiểm kê lần đầu không hiện số hệ thống và chưa nhập giải thích bất thường.
                    ws.DeleteColumn(9);
                    ws.DeleteColumn(7);
                    ws.Cells["G1"].Value = "盤點數量";
                    ws.Cells["H1"].Value = "圖片名稱";
                }

                // Thiết lập độ rộng sau khi đã loại các cột không dùng.
                ws.Column(1).Hidden = true;
                ws.Column(2).Width = 30;
                ws.Column(3).Width = 50;
                ws.Column(4).Width = 15;
                ws.Column(5).Width = 25;
                ws.Column(6).Width = 15;
                ws.Column(7).Width = 15;
                ws.Column(8).Width = IsUploadAbnormal ? 15 : 25;
                if (IsUploadAbnormal)
                {
                    ws.Column(9).Width = 30;
                }

                ws.Cells.Style.WrapText = true;

                pck.Save();
            }

            Process.Start(filePath);
        }

        private void LoadData()
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
            {
                helper.SaveViewInfo();

                deptGetData = string.IsNullOrWhiteSpace(barCbbDept.EditValue?.ToString()) ? "NoDept" : barCbbDept.EditValue.ToString().Split(' ')[0];

                var inspectionBatch = SparePartInspectionBatchService.Instance.GetList();
                inspectionBatchMaterials = SparePartInspectionItemService.Instance.GetList();
                materials = SparePartMaterialService.Instance.GetAllByStartIdDept(deptGetData);

                users = SparePartUserService.Instance.GetList();
                units = SparePartUnitService.Instance.GetList();
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
                                      UserReCheck = string.IsNullOrEmpty(bm.ConfirmedBy) ? "" : users.FirstOrDefault(r => r.Id == bm.ConfirmedBy)?.DisplayName ?? "N/A",
                                      IniQuantity = bm.ActualQuantity.HasValue ? bm.InitialQuantity : (double?)null,
                                      Description = bm.Description,
                                      BatchCancelled = batch.IsCancelled,
                                      PhotoActualName = photoAttachment?.ActualName ?? string.Empty,
                                      PhotoAttachment = photoAttachment,
                                      IsComplete = bm.IsComplete
                                  };
                              })
                        .ToList();

                    return batchMaterialList.Any() ? new
                    {
                        Batch = batch,
                        Spare = batchMaterialList,
                        Status = GetBatchStatus(batch, batchMaterialList)
                    } : null;
                }).Where(x => x != null).ToList();

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

        private void SparePartRecheckView_Load(object sender, EventArgs e)
        {
            gvSparePart.OptionsCustomization.AllowGroup = false;

            gvData.ReadOnlyGridView();
            gvData.KeyDown += DevExpressGridViewHelper.CopyFocusedCellOnCtrlC;
            gvData.OptionsDetail.AllowOnlyOneMasterRowExpanded = true;
            gvSparePart.ReadOnlyGridView();
            gvSparePart.KeyDown += DevExpressGridViewHelper.CopyFocusedCellOnCtrlC;
            gvSparePart.DoubleClick += GvSparePart_DoubleClick;

            // Kiểm tra quyền từng ke để có quyền truy cập theo nhóm
            var departmentItems = SparePartHelper.GetAccessibleDepartments()
                .Select(dept => new ComboBoxItem { Value = $"{dept.Id} {dept.DisplayName}" })
                .ToArray();

            canApproveRecovery = SparePartHelper.CanManageAllDepartments();

            cbbDept.Items.AddRange(departmentItems);
            barCbbDept.EditValue = departmentItems.FirstOrDefault()?.Value ?? string.Empty;

            LoadData();
            CreateRuleGV();
            gcData.DataSource = sourceBases;

            gvData.BestFitColumns();

            gvData.OptionsDetail.EnableMasterViewMode = true;
            gvData.OptionsView.ShowGroupPanel = false;
        }

        private void CbbDept_EditValueChanged(object sender, EventArgs e)
        {
            LoadData();
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

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
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

        private void gvData_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (deptGetData.Length != 4)
            {
                XtraMessageBox.Show("請您選擇「課」來查看物料", "錯誤",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra tính hợp lệ của sự kiện
            if (!e.HitInfo.InRowCell || !e.HitInfo.InDataRow || e.Menu == null)
                return;

            var masterView = sender as GridView;
            int rowHandle = e.HitInfo.RowHandle;

            // Lấy detail view từ dòng cha
            var detailView = masterView.GetDetailView(rowHandle, 0) as GridView;
            if (detailView == null)
                return;

            // Lấy dòng dữ liệu cha
            dynamic masterRow = masterView.GetRow(rowHandle);
            if (masterRow == null)
                return;

            var batch = masterRow.Batch as SparePartInspectionBatch;
            if (batch == null || IsBatchCancelled(batch))
                return;

            // Nếu không phải “每月盤點” thì chỉ quản lý của PGD mới có thể làm
            bool managerRecheck = !batch.BatchName.StartsWith("【每月盤點】");
            if (managerRecheck && !canApproveRecovery)
            {
                return;
            }

            var spareList = masterRow.Spare as IEnumerable<dynamic>;
            if (spareList == null)
                return;

            // Lọc vật liệu do người đăng nhập quản lý
            var userMaterials = spareList
                .Where(x => managerRecheck ? true : x?.Material is SparePartMaterial m && m.IdManager == SparePartConfiguration.LoginUser.Id)
                .Select(x => x?.BatchMaterial as SparePartInspectionItem)
                .Where(x => x != null)
                .ToList();

            if (userMaterials.Count == 0)
                return;

            // Kiểm tra trạng thái
            bool anyCompleted = userMaterials.All(x => x.ActualQuantity != null);
            bool anyIncomplete = userMaterials.Any(x => x.IsComplete != true);

            // Cập nhật caption theo trạng thái
            itemDownCheckFile.Caption = anyCompleted ? "下載異常表" : "下載盤點表";
            itemUpdateCheckFile.Caption = anyCompleted ? "上傳異常表" : "上傳盤點表";

            if (managerRecheck)
            {
                itemDownCheckFile.Caption = "【管理】" + itemDownCheckFile.Caption;
                itemUpdateCheckFile.Caption = "【管理】" + itemUpdateCheckFile.Caption;
            }

            // Nếu tất cả đã hoàn thành thì không hiển thị menu
            if (!anyIncomplete)
                return;

            // Thêm menu thao tác
            e.Menu.Items.Add(itemDownCheckFile);
            e.Menu.Items.Add(itemUpdateCheckFile);
        }

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

            info.GroupText = $" <color={colorName}>{groupValue}</color>：{info.GroupValueText}《<color=Blue>{total}物品</color>{(abnormal == 0 ? "" : $" <color=Red>{abnormal}異常</color>")}》";
        }
    }
}
