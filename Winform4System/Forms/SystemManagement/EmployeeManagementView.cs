using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Winform4System.Business.Services;
using Winform4System.Core.Security;
using Winform4System.DataAccess.Configuration;
using Winform4System.Helpers;

namespace Winform4System.Forms.SystemManagement
{
    public sealed partial class EmployeeManagementView : XtraUserControl
    {
        private readonly EmployeeManagementService _service = new EmployeeManagementService(new ConnectionStringProvider());
        private readonly AuthenticationPolicyService _policyService = new AuthenticationPolicyService(new ConnectionStringProvider());
        private readonly DXMenuItem _editItem;
        private readonly DXMenuItem _deactivateItem;
        private readonly DXMenuItem _accountStatusItem;
        private readonly DXMenuItem _unlockAccountItem;
        private readonly DXMenuItem _revokeSessionsItem;

        public EmployeeManagementView()
        {
            InitializeComponent();
            _editItem = CreateMenuItem("編輯人員", EditSelected, SvgIconCatalog.Edit);
            _deactivateItem = CreateMenuItem("停用人員", DeactivateSelected, SvgIconCatalog.SuspendUser);
            _accountStatusItem = CreateMenuItem("停用登入帳號", ChangeAccountStatus, SvgIconCatalog.Disabled);
            _unlockAccountItem = CreateMenuItem("解除帳號鎖定", UnlockAccount, SvgIconCatalog.Confirm);
            _revokeSessionsItem = CreateMenuItem("撤銷登入工作階段", RevokeSessions, SvgIconCatalog.Denied);
            btnAdd.Visibility = CurrentAuthorization.HasPermission("SYSTEM.USER.ADMIN") ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
            btnSecuritySettings.Visibility = CurrentAuthorization.HasPermission("SYSTEM.USER.ADMIN") ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
        }

        private static DXMenuItem CreateMenuItem(string caption, EventHandler handler, DevExpress.Utils.Svg.SvgImage image)
        {
            var item = new DXMenuItem(caption, handler, image, DXMenuItemPriority.Normal);
            item.ImageOptions.SvgImageSize = new Size(24, 24);
            return item;
        }

        private void EmployeeManagementView_Load(object sender, EventArgs e) { LoadData(); }
        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) { LoadData(); }
        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) { OpenEditor(null); }

        private void btnSecuritySettings_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                using (var form = new AuthenticationPolicyForm(_policyService.GetForManagement()))
                {
                    if (form.ShowDialog(FindForm()) != DialogResult.OK) return;
                    _policyService.Save(form.Value);
                    XtraMessageBox.Show("帳號安全設定已儲存。", "儲存成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex) { ShowError(ex.Message, "儲存帳號安全設定失敗"); }
        }

        private void gridViewEmployees_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.Menu == null || e.MenuType != DevExpress.XtraGrid.Views.Grid.GridMenuType.Row || e.HitInfo.RowHandle < 0 || !CurrentAuthorization.HasPermission("SYSTEM.USER.ADMIN")) return;
            gridViewEmployees.FocusedRowHandle = e.HitInfo.RowHandle;
            _editItem.BeginGroup = e.Menu.Items.Count > 0;
            _deactivateItem.BeginGroup = true;
            EmployeeListItem selected = GetSelected();
            _deactivateItem.Enabled = selected?.EmploymentStatus != 0;
            _accountStatusItem.BeginGroup = true;
            _accountStatusItem.Caption = selected?.IsAccountActive == true ? "停用登入帳號" : "啟用登入帳號";
            _accountStatusItem.ImageOptions.SvgImage = selected?.IsAccountActive == true ? SvgIconCatalog.Disabled : SvgIconCatalog.Confirm;
            _accountStatusItem.Enabled = selected?.HasAccount == true
                && (selected.IsAccountActive == false || !string.Equals(selected.UserId, CurrentAuthorization.UserId, StringComparison.OrdinalIgnoreCase));
            _unlockAccountItem.Enabled = selected?.HasAccount == true && (selected.FailedLoginCount > 0 || selected.LockoutEndUtc.HasValue);
            _revokeSessionsItem.Enabled = selected?.HasAccount == true && selected.IsAccountActive;
            e.Menu.Items.Add(_editItem);
            e.Menu.Items.Add(_deactivateItem);
            e.Menu.Items.Add(_accountStatusItem);
            e.Menu.Items.Add(_unlockAccountItem);
            e.Menu.Items.Add(_revokeSessionsItem);
        }

        private EmployeeListItem GetSelected() => gridViewEmployees.GetFocusedRow() as EmployeeListItem;
        private void EditSelected(object sender, EventArgs e) { var item = GetSelected(); if (item != null) OpenEditor(item); }

        private void OpenEditor(EmployeeListItem item)
        {
            using (var form = new EmployeeEditForm(item, _service.GetDepartments(), _service.GetJobTitles()))
            {
                if (form.ShowDialog(FindForm()) != DialogResult.OK) return;
                try { _service.Save(form.Value); LoadData(form.Value.UserId); }
                catch (Exception ex) { ShowError(ex.Message, "儲存人員失敗"); }
            }
        }

        private void DeactivateSelected(object sender, EventArgs e)
        {
            var item = GetSelected();
            if (item == null) return;
            if (XtraMessageBox.Show($"確定要停用人員「{GetName(item)}」及其登入帳號嗎？", "停用確認", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            try { _service.Deactivate(item.UserId, item.EmployeeRowVersion, item.AccountRowVersion); LoadData(item.UserId); }
            catch (Exception ex) { ShowError(ex.Message, "停用人員失敗"); }
        }

        private void ChangeAccountStatus(object sender, EventArgs e)
        {
            EmployeeListItem item = GetSelected();
            if (item == null || !item.HasAccount) return;
            bool enable = !item.IsAccountActive;
            string action = enable ? "啟用" : "停用";
            if (XtraMessageBox.Show($"確定要{action}「{GetName(item)}」的登入帳號嗎？\n此操作不會變更人員任職狀態。", action + "帳號確認", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            try { _service.SetAccountActive(item.UserId, enable, item.AccountRowVersion); LoadData(item.UserId); }
            catch (Exception ex) { ShowError(ex.Message, action + "帳號失敗"); }
        }

        private void UnlockAccount(object sender, EventArgs e)
        {
            EmployeeListItem item = GetSelected();
            if (item == null || !item.HasAccount) return;
            if (XtraMessageBox.Show($"確定要解除「{GetName(item)}」的帳號鎖定並清除登入失敗次數嗎？", "解除鎖定確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try { _service.UnlockAccount(item.UserId, item.AccountRowVersion); LoadData(item.UserId); }
            catch (Exception ex) { ShowError(ex.Message, "解除帳號鎖定失敗"); }
        }

        private void RevokeSessions(object sender, EventArgs e)
        {
            EmployeeListItem item = GetSelected();
            if (item == null || !item.HasAccount) return;
            if (XtraMessageBox.Show($"確定要撤銷「{GetName(item)}」目前所有登入工作階段嗎？\n使用者最晚會在 30 秒內被要求重新登入。", "撤銷工作階段確認", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            try { _service.RevokeSessions(item.UserId, item.AccountRowVersion); LoadData(item.UserId); }
            catch (Exception ex) { ShowError(ex.Message, "撤銷登入工作階段失敗"); }
        }

        private void LoadData(string selectedUserId = null)
        {
            try
            {
                gridEmployees.DataSource = _service.GetEmployees().ToList();
                if (!string.IsNullOrWhiteSpace(selectedUserId))
                {
                    int handle = gridViewEmployees.LocateByValue("UserId", selectedUserId);
                    if (handle >= 0) gridViewEmployees.FocusedRowHandle = handle;
                }
            }
            catch (Exception ex) { ShowError(ex.Message, "載入人員失敗"); }
        }

        private static string GetName(EmployeeListItem item) => !string.IsNullOrWhiteSpace(item.DisplayNameTW) ? item.DisplayNameTW : !string.IsNullOrWhiteSpace(item.DisplayNameVN) ? item.DisplayNameVN : item.UserId;
        private static void ShowError(string message, string title) { XtraMessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }
}
