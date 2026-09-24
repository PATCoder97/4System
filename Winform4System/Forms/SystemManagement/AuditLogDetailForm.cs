using DevExpress.XtraEditors;
using Winform4System.Business.Services;

namespace Winform4System.Forms.SystemManagement
{
    public sealed partial class AuditLogDetailForm : XtraForm
    {
        public AuditLogDetailForm(AuditLogListItem item)
        {
            InitializeComponent();
            if (item == null) return;
            txbCreatedAt.Text = item.CreatedAtLocal.ToString("yyyy/MM/dd HH:mm:ss.fff");
            txbUser.Text = string.IsNullOrWhiteSpace(item.UserDisplayName) ? item.UserId : item.UserId + " - " + item.UserDisplayName;
            txbAttemptedUser.Text = item.AttemptedUserId;
            txbAction.Text = item.ActionCode;
            txbEntity.Text = item.EntityName;
            txbEntityId.Text = item.EntityId;
            txbMachine.Text = item.MachineName;
            txbIpAddress.Text = item.IpAddress;
            txbCorrelationId.Text = item.CorrelationId.ToString();
            memoDescription.Text = item.Description;
            memoData.Text = item.DataJson;
        }

        private void btnClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) { Close(); }
    }
}
