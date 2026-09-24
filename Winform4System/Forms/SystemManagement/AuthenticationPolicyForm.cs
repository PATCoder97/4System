using DevExpress.XtraEditors;
using System;
using System.Windows.Forms;
using Winform4System.Business.Services;

namespace Winform4System.Forms.SystemManagement
{
    public sealed partial class AuthenticationPolicyForm : XtraForm
    {
        private readonly AuthenticationPolicy _source;

        public AuthenticationPolicy Value { get; private set; }

        public AuthenticationPolicyForm(AuthenticationPolicy source)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            InitializeComponent();
            spinMaximumFailedAttempts.Value = source.MaximumFailedAttempts;
            spinLockoutMinutes.Value = source.LockoutMinutes;
        }

        private void btnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Value = new AuthenticationPolicy
            {
                MaximumFailedAttempts = Convert.ToInt32(spinMaximumFailedAttempts.Value),
                LockoutMinutes = Convert.ToInt32(spinLockoutMinutes.Value),
                MaximumFailedAttemptsRowVersion = _source.MaximumFailedAttemptsRowVersion,
                LockoutMinutesRowVersion = _source.LockoutMinutesRowVersion
            };
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
