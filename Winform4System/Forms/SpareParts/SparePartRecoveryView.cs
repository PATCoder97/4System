using Winform4System.Business.Services.SpareParts;
using Winform4System.DataAccess.Entities.SpareParts;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
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
    public partial class SparePartRecoveryView : XtraUserControl
    {
        private readonly BindingSource sourceTickets = new BindingSource();
        private DXMenuItem itemViewIssue;
        private DXMenuItem itemUploadEvidence;
        private DXMenuItem itemViewEvidence;
        private DXMenuItem itemUpdateTime;
        private DXMenuItem itemConfirmComplete;
        private DXMenuItem itemCancelTicket;
        private GridViewStateManager helper;

        private List<SparePartUser> users = new List<SparePartUser>();
        private bool canApproveRecovery;

        protected virtual bool IsTaskView => false;

        public SparePartRecoveryView()
        {
            InitializeComponent();
            SparePartPermissionUi.Apply(this);
            InitializeIcon();
            InitializeMenuItems();

            helper = new GridViewStateManager(gvData, "Id");

            Font fontUI12 = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            DevExpress.Utils.AppearanceObject.DefaultMenuFont = fontUI12;

            gcData.DataSource = sourceTickets;
            gvData.ReadOnlyGridView();
            gvData.KeyDown += DevExpressGridViewHelper.CopyFocusedCellOnCtrlC;
            gvData.FocusedRowChanged += gvData_FocusedRowChanged;
        }

        private void gvData_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            UpdateActionStates();
        }

        private void InitializeIcon()
        {
            btnReload.ImageOptions.SvgImage = SvgImageCatalog.Reload;
            btnViewIssue.ImageOptions.SvgImage = SvgImageCatalog.View;
            btnDownloadGuide.ImageOptions.SvgImage = SvgImageCatalog.Attach;
            btnManageGuide.ImageOptions.SvgImage = SvgImageCatalog.Edit;
            btnUploadEvidence.ImageOptions.SvgImage = SvgImageCatalog.UploadFile;
            btnViewEvidence.ImageOptions.SvgImage = SvgImageCatalog.Search;
            btnUpdateTime.ImageOptions.SvgImage = SvgImageCatalog.Schedule;
            btnConfirmComplete.ImageOptions.SvgImage = SvgImageCatalog.Confirm;
            btnCancelTicket.ImageOptions.SvgImage = SvgImageCatalog.Remove;
            barCbbDept.ImageOptions.SvgImage = SvgImageCatalog.Dept;
        }

        private void InitializeMenuItems()
        {
            itemViewIssue = CreateMenuItem("查看領用", ItemViewIssueMenu_Click, SvgImageCatalog.View);
            itemUploadEvidence = CreateMenuItem("上傳證明", ItemUploadEvidenceMenu_Click, SvgImageCatalog.UploadFile);
            itemViewEvidence = CreateMenuItem("查看證明", ItemViewEvidenceMenu_Click, SvgImageCatalog.Search);
            itemUpdateTime = CreateMenuItem("更新日期", ItemUpdateTimeMenu_Click, SvgImageCatalog.Schedule);
            itemConfirmComplete = CreateMenuItem("確認完成", ItemConfirmCompleteMenu_Click, SvgImageCatalog.Confirm);
            itemCancelTicket = CreateMenuItem("取消單", ItemCancelTicketMenu_Click, SvgImageCatalog.Remove);
        }

        private DXMenuItem CreateMenuItem(string caption, EventHandler clickEvent, DevExpress.Utils.Svg.SvgImage svgImage)
        {
            var menuItem = new DXMenuItem(caption, clickEvent, svgImage, DXMenuItemPriority.Normal);
            SetMenuItemProperties(menuItem);
            return menuItem;
        }

        private void SetMenuItemProperties(DXMenuItem menuItem)
        {
            menuItem.ImageOptions.SvgImageSize = new Size(24, 24);
            menuItem.AppearanceHovered.ForeColor = Color.Blue;
        }

        private void ItemViewIssueMenu_Click(object sender, EventArgs e) => btnViewIssue_ItemClick(sender, null);

        private void ItemUploadEvidenceMenu_Click(object sender, EventArgs e) => btnUploadEvidence_ItemClick(sender, null);

        private void ItemViewEvidenceMenu_Click(object sender, EventArgs e) => btnViewEvidence_ItemClick(sender, null);

        private void ItemUpdateTimeMenu_Click(object sender, EventArgs e) => btnUpdateTime_ItemClick(sender, null);

        private void ItemConfirmCompleteMenu_Click(object sender, EventArgs e) => btnConfirmComplete_ItemClick(sender, null);

        private void ItemCancelTicketMenu_Click(object sender, EventArgs e) => btnCancelTicket_ItemClick(sender, null);

        private void SparePartRecoveryView_Load(object sender, EventArgs e)
        {
            try
            {
                InitializePermissions();
                LoadData();
            }
            catch (Exception ex)
            {
                Enabled = false;
                XtraMessageBox.Show(ex.Message, SparePartConfiguration.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void InitializePermissions()
        {

            var departments = SparePartHelper.GetAccessibleDepartments();
            canApproveRecovery = SparePartHelper.CanManageAllDepartments();
            btnManageGuide.Visibility = !IsTaskView && canApproveRecovery
                ? BarItemVisibility.Always
                : BarItemVisibility.Never;
            btnDownloadGuide.Visibility = BarItemVisibility.Always;

            cbbDept.Items.Clear();
            cbbDept.Items.AddRange(departments
                .OrderBy(dept => dept.Id)
                .Select(dept => $"{dept.Id} {dept.DisplayName}")
                .ToArray());

            barCbbDept.EditValue = cbbDept.Items.Count > 0
                ? cbbDept.Items[0]?.ToString()
                : string.Empty;

            users = SparePartUserService.Instance.GetList()
                .Where(r => (r.Status ?? 0) == 0)
                .OrderBy(r => r.IdDepartment)
                .ThenBy(r => r.DisplayName)
                .ToList();
        }

        private void LoadData()
        {
            string deptPrefix = ParseToken(barCbbDept.EditValue?.ToString());
            string assignedUserId = IsTaskView ? SparePartConfiguration.LoginUser.Id : string.Empty;

            var tickets = SparePartRecoveryService.Instance.GetList(deptPrefix, string.Empty, null, null, assignedUserId);
            sourceTickets.DataSource = BuildTicketRows(tickets);
            gvData.BestFitColumns();
            UpdateActionStates();
        }

        private List<SparePartRecoveryTicketGridRow> BuildTicketRows(List<SparePartRecoveryTicket> tickets)
        {
            tickets = tickets ?? new List<SparePartRecoveryTicket>();
            if (tickets.Count == 0)
            {
                return new List<SparePartRecoveryTicketGridRow>();
            }

            var materialIds = tickets
                .SelectMany(ticket => new int?[] { ticket.NewMaterialId, ticket.OldBaseMaterialId, ticket.OldRecoveryMaterialId })
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .Distinct()
                .ToList();

            var storageIds = tickets
                .SelectMany(ticket => new int?[] { ticket.SourceStorageId, ticket.RestockStorageId })
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .Distinct()
                .ToList();

            var materials = SparePartMaterialService.Instance.GetListByIds(materialIds)
                .ToDictionary(item => item.Id, item => item);
            var storages = SparePartStorageService.Instance.GetList()
                .Where(item => storageIds.Contains(item.Id))
                .ToDictionary(item => item.Id, item => item);
            var userMap = users.ToDictionary(item => item.Id, item => item);
            var evidenceCountMap = SparePartRecoveryService.Instance.GetEvidenceCountMap(tickets.Select(item => item.Id));

            return tickets.Select(ticket =>
            {
                materials.TryGetValue(ticket.NewMaterialId, out SparePartMaterial newMaterial);
                materials.TryGetValue(ticket.OldBaseMaterialId, out SparePartMaterial oldBaseMaterial);

                SparePartMaterial oldRecoveryMaterial = null;
                if (ticket.OldRecoveryMaterialId.HasValue)
                {
                    materials.TryGetValue(ticket.OldRecoveryMaterialId.Value, out oldRecoveryMaterial);
                }

                storages.TryGetValue(ticket.SourceStorageId, out SparePartStorage sourceStorage);

                SparePartStorage restockStorage = null;
                if (ticket.RestockStorageId.HasValue)
                {
                    storages.TryGetValue(ticket.RestockStorageId.Value, out restockStorage);
                }

                userMap.TryGetValue(ticket.AssignedUserId ?? string.Empty, out SparePartUser assignedUser);
                userMap.TryGetValue(ticket.CreatedBy ?? string.Empty, out SparePartUser createdUser);

                return new SparePartRecoveryTicketGridRow
                {
                    Id = ticket.Id,
                    TicketNo = ticket.TicketNo,
                    IssueTransactionId = ticket.IssueTransactionId,
                    RestockInTransactionId = ticket.RestockInTransactionId,
                    NewMaterialId = ticket.NewMaterialId,
                    OldBaseMaterialId = ticket.OldBaseMaterialId,
                    OldRecoveryMaterialId = ticket.OldRecoveryMaterialId,
                    NewMaterialCode = newMaterial?.Code,
                    NewMaterialDisplayName = newMaterial?.DisplayName,
                    OldBaseMaterialCode = oldBaseMaterial?.Code,
                    OldBaseMaterialDisplayName = oldBaseMaterial?.DisplayName,
                    OldRecoveryMaterialCode = oldRecoveryMaterial?.Code,
                    OldRecoveryMaterialDisplayName = oldRecoveryMaterial?.DisplayName,
                    RecoveryOption = ticket.RecoveryOption,
                    Quantity = ticket.Quantity,
                    SourceStorageId = ticket.SourceStorageId,
                    SourceStorageName = sourceStorage?.DisplayName,
                    RestockStorageId = ticket.RestockStorageId,
                    RestockStorageName = restockStorage?.DisplayName,
                    AssignedUserId = ticket.AssignedUserId,
                    AssignedUserName = assignedUser?.DisplayName,
                    PlannedDisposeDate = ticket.PlannedDisposeDate,
                    ActualDisposeDate = ticket.ActualDisposeDate,
                    Status = ticket.Status,
                    Description = ticket.Description,
                    ResultNote = ticket.ResultNote,
                    EvidenceCount = evidenceCountMap.TryGetValue(ticket.Id, out int evidenceCount)
                        ? evidenceCount
                        : 0,
                    CreatedBy = ticket.CreatedBy,
                    CreatedByName = createdUser?.DisplayName,
                    CreatedDate = ticket.CreatedDate,
                    IdDept = newMaterial?.IdDept
                };
            }).ToList();
        }

        private string ParseToken(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                return string.Empty;
            }

            return raw.Split(' ')[0];
        }

        private SparePartRecoveryTicketGridRow GetFocusedRow()
        {
            if (gvData.FocusedRowHandle < 0)
            {
                return null;
            }

            return gvData.GetRow(gvData.FocusedRowHandle) as SparePartRecoveryTicketGridRow;
        }

        private void Filter_EditValueChanged(object sender, EventArgs e)
        {
            if (!IsHandleCreated)
            {
                return;
            }

            LoadData();
        }

        private void btnReload_ItemClick(object sender, ItemClickEventArgs e)
        {
            LoadData();
        }

        private void UpdateActionStates()
        {
            var row = GetFocusedRow();
            bool hasRow = row != null;
            bool isScrap = hasRow && SparePartRecoveryConstants.IsScrap(row.RecoveryOption);
            bool isRestock = hasRow && SparePartRecoveryConstants.IsRestock(row.RecoveryOption);
            bool isScheduled = hasRow && string.Equals(row.Status, SparePartRecoveryConstants.RecoveryStatusScheduled, StringComparison.OrdinalIgnoreCase);
            bool isAwaitManagerConfirm = hasRow && string.Equals(row.Status, SparePartRecoveryConstants.RecoveryStatusAwaitManagerConfirm, StringComparison.OrdinalIgnoreCase);
            bool isCompleted = hasRow && string.Equals(row.Status, SparePartRecoveryConstants.RecoveryStatusCompleted, StringComparison.OrdinalIgnoreCase);
            bool isCancelled = hasRow && string.Equals(row.Status, SparePartRecoveryConstants.RecoveryStatusCancelled, StringComparison.OrdinalIgnoreCase);
            bool isCreator = hasRow && string.Equals(row.CreatedBy, SparePartConfiguration.LoginUser.Id, StringComparison.OrdinalIgnoreCase);

            btnViewIssue.Enabled = hasRow;
            btnDownloadGuide.Enabled = true;
            btnUploadEvidence.Enabled = IsTaskView && hasRow && isScrap && !isCompleted && !isCancelled;
            btnViewEvidence.Enabled = hasRow && row.EvidenceCount > 0;
            btnUpdateTime.Enabled = IsTaskView && hasRow && isScrap && isScheduled;
            btnConfirmComplete.Enabled = !IsTaskView && hasRow && isScrap && isAwaitManagerConfirm && (canApproveRecovery || isCreator);
            btnCancelTicket.Enabled = !IsTaskView && hasRow && !isCompleted && !isRestock;
        }

        private void gvData_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (!e.HitInfo.InRowCell || !e.HitInfo.InDataRow || e.Menu == null)
            {
                return;
            }

            gvData.FocusedRowHandle = e.HitInfo.RowHandle;
            var row = GetFocusedRow();
            if (row == null)
            {
                return;
            }

            UpdateActionStates();

            itemViewIssue.Enabled = btnViewIssue.Enabled;
            itemUploadEvidence.Enabled = btnUploadEvidence.Enabled;
            itemViewEvidence.Enabled = btnViewEvidence.Enabled;
            itemUpdateTime.Enabled = btnUpdateTime.Enabled;
            itemConfirmComplete.Enabled = btnConfirmComplete.Enabled;
            itemCancelTicket.Enabled = btnCancelTicket.Enabled;

            itemViewIssue.BeginGroup = true;
            itemUploadEvidence.BeginGroup = true;
            itemViewEvidence.BeginGroup = false;
            itemUpdateTime.BeginGroup = false;
            itemConfirmComplete.BeginGroup = false;
            itemCancelTicket.BeginGroup = true;

            e.Menu.Items.Add(itemViewIssue);

            if (IsTaskView && itemUploadEvidence.Enabled)
            {
                e.Menu.Items.Add(itemUploadEvidence);
            }

            if (itemViewEvidence.Enabled)
            {
                e.Menu.Items.Add(itemViewEvidence);
            }

            if (IsTaskView && itemUpdateTime.Enabled)
            {
                e.Menu.Items.Add(itemUpdateTime);
            }

            if (!IsTaskView && itemConfirmComplete.Enabled)
            {
                e.Menu.Items.Add(itemConfirmComplete);
            }

            if (!IsTaskView && itemCancelTicket.Enabled)
            {
                e.Menu.Items.Add(itemCancelTicket);
            }
        }

        private void btnViewIssue_ItemClick(object sender, ItemClickEventArgs e)
        {
            var row = GetFocusedRow();
            if (row == null)
            {
                return;
            }

            string message = $"回收單號: {row.TicketNo}\r\n" +
                $"新物料: {row.NewMaterialCode} / {row.NewMaterialDisplayName}\r\n" +
                $"舊物料: {row.OldBaseMaterialCode} / {row.OldBaseMaterialDisplayName}\r\n" +
                $"方式: {row.RecoveryOptionDisplay}\r\n" +
                $"數量: {row.Quantity}\r\n" +
                $"來源倉庫: {row.SourceStorageName}\r\n" +
                $"建立時間: {row.CreatedDate:yyyy/MM/dd HH:mm}\r\n" +
                $"備註: {row.Description}";

            XtraMessageBox.Show(message, SparePartConfiguration.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnDownloadGuide_ItemClick(object sender, ItemClickEventArgs e)
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

        private void btnManageGuide_ItemClick(object sender, ItemClickEventArgs e)
        {
            using (var form = new SparePartRecoveryGuideForm())
            {
                form.ShowDialog();
            }
        }

        private void btnUploadEvidence_ItemClick(object sender, ItemClickEventArgs e)
        {
            var row = GetFocusedRow();
            if (row == null)
            {
                return;
            }

            if (!SparePartRecoveryConstants.IsScrap(row.RecoveryOption) ||
                string.Equals(row.Status, SparePartRecoveryConstants.RecoveryStatusCancelled, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(row.Status, SparePartRecoveryConstants.RecoveryStatusCompleted, StringComparison.OrdinalIgnoreCase))
            {
                SparePartMessage.MsgError("目前狀態不可上傳證明。");
                return;
            }

            using (var form = new SparePartRecoveryEvidenceForm(row.TicketNo, row.ActualDisposeDate, row.ResultNote))
            {
                if (form.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                var evidenceItems = form.SelectedFiles.Select(file =>
                {
                    var saved = SparePartRecoveryFileHelper.SaveEvidenceFile(row.Id, file);
                    return new SparePartRecoveryEvidence
                    {
                        RecoveryTicketId = row.Id,
                        ActualName = saved.actualName,
                        EncryptionName = saved.encryptionName,
                        FileExt = saved.extension,
                        UploadedBy = SparePartConfiguration.LoginUser.Id,
                        UploadedDate = DateTime.Now,
                        IsActive = true
                    };
                }).ToList();

                if (!SparePartRecoveryService.Instance.UploadEvidence(
                    row.Id,
                    form.ActualDisposeDate,
                    form.ResultNote,
                    SparePartConfiguration.LoginUser.Id,
                    evidenceItems,
                    out string message))
                {
                    SparePartMessage.MsgError(string.IsNullOrWhiteSpace(message) ? "上傳證明失敗。" : message);
                    return;
                }
            }

            LoadData();
        }

        private void btnViewEvidence_ItemClick(object sender, ItemClickEventArgs e)
        {
            var row = GetFocusedRow();
            if (row == null)
            {
                return;
            }

            var evidences = SparePartRecoveryService.Instance.GetEvidenceListByTicketId(row.Id);
            if (evidences.Count == 0)
            {
                XtraMessageBox.Show("目前尚未上傳證明。", SparePartConfiguration.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SparePartRecoveryFileHelper.OpenEvidenceFiles(evidences);
        }

        private void btnUpdateTime_ItemClick(object sender, ItemClickEventArgs e)
        {
            var row = GetFocusedRow();
            if (row == null)
            {
                return;
            }

            if (!SparePartRecoveryConstants.IsScrap(row.RecoveryOption) ||
                !string.Equals(row.Status, SparePartRecoveryConstants.RecoveryStatusScheduled, StringComparison.OrdinalIgnoreCase))
            {
                SparePartMessage.MsgError("只有已安排的報廢案件可更新日期。");
                return;
            }

            var availableUsers = users
                .Where(r => !string.IsNullOrWhiteSpace(r.IdDepartment) &&
                    (!string.IsNullOrWhiteSpace(row.IdDept)
                        ? r.IdDepartment.StartsWith(row.IdDept, StringComparison.OrdinalIgnoreCase)
                        : true))
                .ToList();

            using (var form = new SparePartRecoveryScheduleForm(availableUsers, row.AssignedUserId, row.PlannedDisposeDate))
            {
                if (form.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                if (!SparePartRecoveryService.Instance.UpdateSchedule(row.Id, form.AssignedUserId, form.PlannedDisposeDate, SparePartConfiguration.LoginUser.Id, out string message))
                {
                    SparePartMessage.MsgError(string.IsNullOrWhiteSpace(message) ? "更新日期失敗。" : message);
                    return;
                }
            }

            LoadData();
        }

        private void btnConfirmComplete_ItemClick(object sender, ItemClickEventArgs e)
        {
            var row = GetFocusedRow();
            if (row == null)
            {
                return;
            }

            if (!SparePartRecoveryService.Instance.ConfirmCompleted(row.Id, SparePartConfiguration.LoginUser.Id, row.ResultNote, out string message))
            {
                SparePartMessage.MsgError(string.IsNullOrWhiteSpace(message) ? "確認完成失敗。" : message);
                return;
            }

            LoadData();
        }

        private void btnCancelTicket_ItemClick(object sender, ItemClickEventArgs e)
        {
            var row = GetFocusedRow();
            if (row == null)
            {
                return;
            }

            string reason = XtraInputBox.Show(new XtraInputBoxArgs
            {
                Caption = SparePartConfiguration.SoftNameTW,
                Prompt = "請輸入取消原因",
                DefaultResponse = string.Empty,
                Editor = new TextEdit()
            })?.ToString();

            if (!SparePartRecoveryService.Instance.CancelTicket(row.Id, SparePartConfiguration.LoginUser.Id, reason, out string message))
            {
                SparePartMessage.MsgError(string.IsNullOrWhiteSpace(message) ? "取消失敗。" : message);
                return;
            }

            LoadData();
        }
    }
}

