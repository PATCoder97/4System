using DevExpress.XtraEditors;
using System;
using System.Drawing;
using System.Windows.Forms;
using Winform4System.Logging;

namespace Winform4System.Forms.Login
{
    public partial class LoginForm : XtraForm
    {
        private readonly IAppLogger _logger;
        private bool _isDragging;
        private Point _dragOffset;

        public LoginForm(IAppLogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            InitializeComponent();
            txtUserId.Text = "DEMO001";
            lblVersion.Text = $"版本 {Application.ProductVersion}";
        }

        public string UserId => txtUserId.Text.Trim().ToUpperInvariant();

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UserId))
            {
                XtraMessageBox.Show(
                    "請輸入帳號。",
                    "登入提示",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                txtUserId.Focus();
                return;
            }

            _logger.Info(nameof(LoginForm), $"Demo login accepted: {UserId}");
            DialogResult = DialogResult.OK;
            Close();
        }

        private void txtPassword_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            txtPassword.Properties.UseSystemPasswordChar = !txtPassword.Properties.UseSystemPasswordChar;
        }

        private void LoginForm_Shown(object sender, EventArgs e)
        {
            txtPassword.Focus();
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
