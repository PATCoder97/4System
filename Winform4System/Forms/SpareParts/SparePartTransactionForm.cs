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
using Winform4System.DataAccess.Entities.SpareParts;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using Winform4System.Forms.SpareParts;

using Winform4System.Helpers;

namespace Winform4System.Forms.SpareParts
{
    public partial class SparePartTransactionForm : DevExpress.XtraEditors.XtraForm
    {
        private const int RecoveryOptionNoneValue = 0;
        private const int RecoveryOptionScrapValue = 1;
        private const int RecoveryOptionRestockValue = 2;

        public SparePartTransactionForm()
        {
            InitializeComponent();
            SparePartPermissionUi.Apply(this);
            InitializeIcon();
        }

        public string eventInfo = "";
        public int idMaterial = -1;

        SparePartTransaction transaction = new SparePartTransaction();
        SparePartMaterial material;
        List<SparePartStorage> storages;

        List<LayoutControlItem> lcControls;
        List<LayoutControlItem> lcImpControls;
        Dictionary<LayoutControlItem, string> lcBaseTexts = new Dictionary<LayoutControlItem, string>();

        private void InitializeIcon()
        {
            btnEdit.ImageOptions.SvgImage = SvgIconCatalog.Edit;
            btnDelete.ImageOptions.SvgImage = SvgIconCatalog.Delete;
            btnConfirm.ImageOptions.SvgImage = SvgIconCatalog.Confirm;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            // BarManager chỉ hoàn tất chiều cao dock sau khi cửa sổ được hiển thị.
            AdjustFormSize();
        }

        private void AdjustFormSize()
        {
            if (lcControls == null || lcControls.Count == 0) return;
            // Đo trên chiều rộng hiện tại sau khi các dòng theo nghiệp vụ đã được ẩn/hiện.
            layoutControl1.PerformLayout();
            layoutControl1.Root.BestFit();
            layoutControl1.BestFit();
            var preferred = layoutControl1.GetPreferredSize(new Size(layoutControl1.ClientSize.Width, 0));
            int breathingRoom = Math.Max(8, (int)Math.Round(8 * DeviceDpi / 96f));
            int naturalHeight = barDockControlTop.Height + Math.Max(preferred.Height, Root.MinSize.Height) + breathingRoom;
            int availableHeight = Screen.FromControl(this).WorkingArea.Height - (Height - ClientSize.Height) - breathingRoom * 2;
            ClientSize = new Size(ClientSize.Width, Math.Min(naturalHeight, availableHeight));
            layoutControl1.AutoScroll = true;
        }

        private int GetSelectedRecoveryOptionValue()
        {
            return int.TryParse(cbbRecoveryOption.EditValue?.ToString(), out int optionValue)
                ? optionValue
                : RecoveryOptionNoneValue;
        }

        private string GetSelectedRecoveryOption()
        {
            switch (GetSelectedRecoveryOptionValue())
            {
                case RecoveryOptionScrapValue:
                    return SparePartRecoveryConstants.RecoveryOptionScrap;
                case RecoveryOptionRestockValue:
                    return SparePartRecoveryConstants.RecoveryOptionRestock;
                default:
                    return SparePartRecoveryConstants.RecoveryOptionNone;
            }
        }

        private void cbbRecoveryOption_EditValueChanged(object sender, EventArgs e)
        {
            UpdateRecoverySectionVisibility();
        }

        private void btnViewGuideDocs_Click(object sender, EventArgs e)
        {
            var guides = SparePartRecoveryService.Instance.GetGuideList();
            if (guides.Count == 0)
            {
                XtraMessageBox.Show("目前尚未上傳報廢指引。", SparePartConfiguration.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SparePartRecoveryFileHelper.OpenGuideFiles(guides);
        }

        private void LoadRecoveryLookups()
        {
            if (material == null)
            {
                return;
            }

            var activeUsers = SparePartUserService.Instance.GetList()
                .Where(r => (r.Status ?? 0) == 0
                    && !string.IsNullOrWhiteSpace(r.IdDepartment)
                    && r.IdDepartment.StartsWith(material.IdDept, StringComparison.OrdinalIgnoreCase))
                .OrderBy(r => r.Id)
                .ThenBy(r => r.DisplayName)
                .ToList();

            sleAssignedUser.Properties.DataSource = activeUsers;
            sleAssignedUser.EditValue = SparePartConfiguration.LoginUser.Id;

            cbbRestockStorage.Properties.DataSource = storages;
            cbbRestockStorage.EditValue = storages.FirstOrDefault(r => r.Id == 2)?.Id ?? storages.FirstOrDefault()?.Id;
        }

        private void UpdateRecoverySectionVisibility()
        {
            bool isIssue = eventInfo == "領用";
            int option = GetSelectedRecoveryOptionValue();
            bool showRecoveryFields = isIssue && option != RecoveryOptionNoneValue;
            bool isScrap = isIssue && option == RecoveryOptionScrapValue;
            bool isRestock = isIssue && option == RecoveryOptionRestockValue;

            lcRecoveryOption.Visibility = isIssue
                ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            lcRecoveryNote.Visibility = showRecoveryFields
                ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            lcAssignedUser.Visibility = isScrap
                ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            lcPlannedDisposeDate.Visibility = isScrap
                ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            lcGuideDocs.Visibility = isScrap
                ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            lcRestockStorage.Visibility = isRestock
                ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

            RebuildRequiredItems();
            RefreshLayoutCaptions();
            AdjustFormSize();
        }

        private void RebuildRequiredItems()
        {
            lcImpControls = new List<LayoutControlItem> { lcStorageFrom, lcStorageTo, lcQuantity };

            switch (eventInfo)
            {
                case "領用":
                    lcImpControls.Add(lcDesc);
                    if (GetSelectedRecoveryOptionValue() == RecoveryOptionScrapValue)
                    {
                        lcImpControls.Add(lcAssignedUser);
                        lcImpControls.Add(lcPlannedDisposeDate);
                    }

                    if (GetSelectedRecoveryOptionValue() == RecoveryOptionRestockValue)
                    {
                        lcImpControls.Add(lcRestockStorage);
                    }
                    break;
                case "收貨":
                    lcImpControls.Add(lcPrice);
                    break;
                case "盤點":
                    lcImpControls.Add(lcDesc);
                    break;
            }
        }

        private void RefreshLayoutCaptions()
        {
            foreach (var item in lcControls)
            {
                if (!lcBaseTexts.ContainsKey(item))
                {
                    lcBaseTexts[item] = item.Text.Replace("<color=#000000>", string.Empty)
                        .Replace("</color>", string.Empty)
                        .Replace("<color=red>*</color>", string.Empty);
                }

                string baseText = lcBaseTexts[item];
                item.AllowHtmlStringInCaption = true;
                item.Text = $"<color=#000000>{baseText}</color>";

                bool isRequired = lcImpControls.Contains(item)
                    && item.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                    && (!item.TextVisible || item.Control == null || item.Control.Enabled);

                if (isRequired)
                {
                    item.Text += "<color=red>*</color>";
                }
            }
        }

        private bool EnsureMaterialActive(bool closeWhenBlocked = false)
        {
            if (material == null)
            {
                SparePartMessage.MsgError("找不到對應物料");
                if (closeWhenBlocked) Close();
                return false;
            }

            if (material.IsDisable == true)
            {
                XtraMessageBox.Show("停用中的物料不可進行庫存作業。", SparePartConfiguration.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                if (closeWhenBlocked) Close();
                return false;
            }

            return true;
        }

        private void LockControl()
        {
            Text = $"新增{eventInfo}事件";

            btnConfirm.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

            txbStorageFromQuantity.Enabled = false;
            txbStorageToQuantity.Enabled = false;

            lcPrice.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            lcExpDate.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;


            switch (eventInfo)
            {
                case "收貨":
                    cbbStorageTo.EditValue = 2;
                    cbbStorageTo.Enabled = false;
                    cbbStorageFrom.Enabled = false;

                    lcPrice.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lcExpDate.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;

                    break;

                case "調撥":
                    cbbStorageTo.EditValue = 1;
                    cbbStorageTo.Enabled = true;
                    cbbStorageFrom.Enabled = false;
                    break;

                case "領用":
                    cbbStorageFrom.Enabled = true;
                    cbbStorageTo.Enabled = false;

                    break;

                case "轉庫":

                    cbbStorageFrom.EditValue = 2;
                    cbbStorageTo.Enabled = true;
                    cbbStorageFrom.Enabled = true;
                    break;

                case "盤點": goto case "領用";
                default:
                    break;
            }

            UpdateRecoverySectionVisibility();
        }

        private void SparePartTransactionForm_Load(object sender, EventArgs e)
        {
            lcControls = new List<LayoutControlItem>()
            {
                lcStorageFrom, lcStorageTo, lcStorageFromQuantity, lcStorageToQuantity,
                lcQuantity, lcDesc, lcPrice, lcExpDate,
                lcRecoveryOption, lcAssignedUser,
                lcPlannedDisposeDate, lcRestockStorage, lcRecoveryNote, lcGuideDocs
            };

            foreach (var item in lcControls.Where(r => r != null))
            {
                if (!lcBaseTexts.ContainsKey(item))
                {
                    lcBaseTexts[item] = item.Text;
                }
            }

            storages = SparePartStorageService.Instance.GetList();
            cbbStorageFrom.Properties.DataSource = storages;
            cbbStorageFrom.Properties.DisplayMember = "DisplayName";
            cbbStorageFrom.Properties.ValueMember = "Id";

            cbbStorageTo.Properties.DataSource = storages;
            cbbStorageTo.Properties.DisplayMember = "DisplayName";
            cbbStorageTo.Properties.ValueMember = "Id";

            material = SparePartMaterialService.Instance.GetItemById(idMaterial);
            if (!EnsureMaterialActive(true))
            {
                return;
            }

            lcBaseTexts[lcQuantity] = lcBaseTexts[lcQuantity].Replace("事件", eventInfo);
            LoadRecoveryLookups();

            LockControl();
        }

        private void cbbStorageFrom_EditValueChanged(object sender, EventArgs e)
        {
            switch (cbbStorageFrom.EditValue)
            {
                case 1:
                    txbStorageFromQuantity.EditValue = material.QuantityInMachine;
                    break;
                case 2:
                    txbStorageFromQuantity.EditValue = material.QuantityInStorage;
                    break;
                default:
                    txbStorageFromQuantity.EditValue = "";
                    break;
            }

            cbbStorageTo.Properties.DataSource = storages.Where(r => r.Id != (int)cbbStorageFrom.EditValue).ToList();
            cbbStorageTo.EditValue = "";
            txbStorageToQuantity.EditValue = "";
        }

        private void cbbStorageTo_EditValueChanged(object sender, EventArgs e)
        {
            switch (cbbStorageTo.EditValue)
            {
                case 1:
                    txbStorageToQuantity.EditValue = material.QuantityInMachine;
                    break;
                case 2:
                    txbStorageToQuantity.EditValue = material.QuantityInStorage;
                    break;
                default:
                    txbStorageToQuantity.EditValue = "";
                    break;
            }
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            material = SparePartMaterialService.Instance.GetItemById(idMaterial);
            if (!EnsureMaterialActive())
            {
                return;
            }

            // Kiểm tra xem đã điền đầy đủ thông tin yêu cầu hay chưa
            bool IsValidate = true;

            foreach (var item in lcImpControls)
            {
                if (item.Control is BaseEdit baseEdit)
                {
                    if (item.Enabled == false || item.Visibility != DevExpress.XtraLayout.Utils.LayoutVisibility.Always) continue;
                    if (string.IsNullOrEmpty(baseEdit.EditValue?.ToString()))
                    {
                        IsValidate = false;
                        break; // Dừng vòng lặp ngay khi phát hiện lỗi
                    }
                }
            }

            if (!IsValidate)
            {
                SparePartMessage.MsgError("請填寫所有信息<color=red>(*)</color>");
                return;
            }


            bool result = false;
            transaction.MaterialId = idMaterial;
            transaction.CreatedDate = DateTime.Now;
            transaction.Desc = txbDesc.EditValue?.ToString();
            transaction.UserDo = SparePartConfiguration.LoginUser.Id;

            double storageQuantityFrom = 0;
            double storageQuantityTo = 0;
            double quantity = 0;

            double.TryParse(txbStorageFromQuantity.EditValue?.ToString(), out storageQuantityFrom);
            double.TryParse(txbStorageToQuantity.EditValue?.ToString(), out storageQuantityTo);
            double.TryParse(txbQuantity.EditValue?.ToString(), out quantity);

            if (quantity <= 0)
            {
                XtraMessageBox.Show($"{eventInfo}數量不得小於零", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            switch (eventInfo)
            {
                case "收貨":

                    var newPrice = Convert.ToInt32(txbPrice.EditValue);
                    if (newPrice <= 0)
                    {
                        XtraMessageBox.Show($"單價不得小於零", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    transaction.TransactionType = "in";
                    transaction.Quantity = quantity;
                    transaction.StorageId = (int)cbbStorageTo.EditValue;

                    result = SparePartTransactionService.Instance.Add(transaction);

                    var resultUpdate = SparePartPriceService.Instance.Add(new SparePartPrice()
                    {
                        MaterialId = idMaterial,
                        Price = newPrice,
                        ChangedAt = DateTime.Now,
                        ChangedBy = SparePartConfiguration.LoginUser.Id
                    });

                    if (!string.IsNullOrEmpty(txbExpDate.Text))
                    {
                        material = SparePartMaterialService.Instance.GetItemById(idMaterial);

                        material.ExpDate = txbExpDate.DateTime;
                        SparePartMaterialService.Instance.AddOrUpdate(material);
                    }

                    break;

                case "調撥":

                    transaction.TransactionType = "in";
                    transaction.Quantity = quantity;
                    transaction.StorageId = (int)cbbStorageTo.EditValue;

                    result = SparePartTransactionService.Instance.Add(transaction);

                    break;

                case "領用":

                    if (quantity > storageQuantityFrom)
                    {
                        XtraMessageBox.Show($"{eventInfo}數量大於庫存數量", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    result = SparePartRecoveryService.Instance.CreateIssueTransactionWithRecovery(
                        idMaterial,
                        (int)cbbStorageFrom.EditValue,
                        quantity,
                        txbDesc.EditValue?.ToString(),
                        SparePartConfiguration.LoginUser.Id,
                        GetSelectedRecoveryOption(),
                        sleAssignedUser.EditValue?.ToString(),
                        dePlannedDisposeDate.EditValue == null
                            ? (DateTime?)null
                            : dePlannedDisposeDate.DateTime.Date,
                        cbbRestockStorage.EditValue == null
                            ? (int?)null
                            : Convert.ToInt32(cbbRestockStorage.EditValue),
                        memoRecoveryNote.EditValue?.ToString(),
                        out string recoveryMessage);

                    if (!result)
                    {
                        SparePartMessage.MsgError(string.IsNullOrWhiteSpace(recoveryMessage)
                            ? "建立領用與回收單失敗。"
                            : recoveryMessage);
                        return;
                    }

                    break;

                case "轉庫":

                    if (quantity > storageQuantityFrom)
                    {
                        XtraMessageBox.Show($"{eventInfo}數量大於庫存數量", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var transferTransactions = new List<SparePartTransaction>
                    {
                        new SparePartTransaction
                        {
                            MaterialId = idMaterial,
                            CreatedDate = transaction.CreatedDate,
                            Desc = transaction.Desc,
                            UserDo = transaction.UserDo,
                            TransactionType = "transfer",
                            Quantity = -quantity,
                            StorageId = (int)cbbStorageFrom.EditValue
                        },
                        new SparePartTransaction
                        {
                            MaterialId = idMaterial,
                            CreatedDate = transaction.CreatedDate,
                            Desc = transaction.Desc,
                            UserDo = transaction.UserDo,
                            TransactionType = "transfer",
                            Quantity = quantity,
                            StorageId = (int)cbbStorageTo.EditValue
                        }
                    };

                    result = SparePartTransactionService.Instance.AddRange(transferTransactions);

                    break;

                case "盤點":

                    transaction.TransactionType = "check";
                    transaction.Quantity = quantity;
                    transaction.StorageId = (int)cbbStorageFrom.EditValue;

                    result = SparePartTransactionService.Instance.Add(transaction);

                    break;
                default:
                    break;
            }

            if (result)
            {
                Close();
            }
            else
            {
                SparePartMessage.MsgErrorDB();
            }
        }
    }
}
