using DevExpress.XtraEditors;
using System;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Winform4System.Business.Services;
using Winform4System.Core.Models;
using Winform4System.Logging;

namespace Winform4System.Forms.Login
{
    public partial class LoginForm : XtraForm
    {
        private static readonly Regex UserIdPattern = new Regex(@"^VNW\d{7}$", RegexOptions.CultureInvariant);
        private readonly IAppLogger _logger;
        private readonly IAuthenticationService _authenticationService;
        private bool _isAuthenticating;
        private bool _isDragging;
        private Point _dragOffset;

        public LoginForm(IAuthenticationService authenticationService, IAppLogger logger)
        {
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            InitializeComponent();
            lblVersion.Text = $"版本 {Application.ProductVersion}";
        }

        public string UserId => txtUserId.Text.Trim().ToUpperInvariant();
        public UserSession Session { get; private set; }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            if (_isAuthenticating)
                return;

            if (string.IsNullOrWhiteSpace(UserId) || string.IsNullOrEmpty(txtPassword.Text))
            {
                XtraMessageBox.Show(
                    "請輸入帳號及密碼。",
                    ApplicationMetadata.DisplayName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                (string.IsNullOrWhiteSpace(UserId) ? (Control)txtUserId : txtPassword).Focus();
                return;
            }

            if (!UserIdPattern.IsMatch(UserId))
            {
                XtraMessageBox.Show(
                    "使用者代碼格式必須為 VNW 加上 7 位數字，例如 VNW0014732。",
                    ApplicationMetadata.DisplayName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                txtUserId.Focus();
                return;
            }

            SetAuthenticatingState(true);
            try
            {
                string loginName = UserId;
                string password = txtPassword.Text;
                AuthenticationResult result = await Task.Run(() => _authenticationService.Authenticate(loginName, password));

                txtPassword.Text = string.Empty;
                if (!result.Succeeded)
                {
                    ShowAuthenticationFailure(result.FailureReason);
                    return;
                }

                Session = result.Session;
                _logger.Info(nameof(LoginForm), $"Login succeeded: {Session.UserId}");
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (DataException exception)
            {
                _logger.Error(nameof(LoginForm), "Database error during login.", exception);
                XtraMessageBox.Show(
                    "目前無法連線至登入服務，請確認網路連線後再試一次。",
                    ApplicationMetadata.DisplayName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (Exception exception)
            {
                _logger.Error(nameof(LoginForm), "Unexpected error during login.", exception);
                XtraMessageBox.Show(
                    "登入時發生錯誤，請稍後再試。",
                    ApplicationMetadata.DisplayName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                if (!IsDisposed)
                    SetAuthenticatingState(false);
            }
        }

        private void ShowAuthenticationFailure(AuthenticationFailureReason reason)
        {
            _logger.Info(nameof(LoginForm), $"Login rejected: {UserId}; reason: {reason}");
            if (reason == AuthenticationFailureReason.DomainUnavailable)
            {
                XtraMessageBox.Show(
                    "目前無法連線至公司網域。請確認公司網路或 VPN 連線後再試一次。",
                    ApplicationMetadata.DisplayName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                txtPassword.Focus();
                return;
            }
            XtraMessageBox.Show(
                "帳號或密碼不正確，或此帳號目前無法登入。",
                ApplicationMetadata.DisplayName,
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            txtPassword.Focus();
        }

        private void SetAuthenticatingState(bool isAuthenticating)
        {
            _isAuthenticating = isAuthenticating;
            txtUserId.Enabled = !isAuthenticating;
            txtPassword.Enabled = !isAuthenticating;
            btnLogin.Enabled = !isAuthenticating;
            btnCancel.Enabled = !isAuthenticating;
            btnLogin.Text = isAuthenticating ? "登入中…" : "登入";
            Cursor = isAuthenticating ? Cursors.WaitCursor : Cursors.Default;
        }

        private void txtPassword_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            txtPassword.Properties.UseSystemPasswordChar = !txtPassword.Properties.UseSystemPasswordChar;
        }

        private void LoginForm_Shown(object sender, EventArgs e)
        {
            txtUserId.Focus();

#if DEBUG
            txtUserId.Text = "VNW0014732";
            txtPassword.Text = "Ab123456";
#endif
        }

        private void LoginForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            _isDragging = true;
            _dragOffset = e.Location;
        }

        private void LoginForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging)
                return;

            Point cursorPosition = Cursor.Position;
            Location = new Point(cursorPosition.X - _dragOffset.X, cursorPosition.Y - _dragOffset.Y);
        }

        private void LoginForm_MouseUp(object sender, MouseEventArgs e)
        {
            _isDragging = false;
        }
    }
}
