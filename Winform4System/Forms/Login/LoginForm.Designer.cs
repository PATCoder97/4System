namespace Winform4System.Forms.Login
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnLogin;
        private DevExpress.XtraEditors.TextEdit txtUserId;
        private DevExpress.XtraEditors.ButtonEdit txtPassword;
        private DevExpress.XtraEditors.LabelControl lblApplicationName;
        private DevExpress.XtraEditors.LabelControl lblVersion;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            DevExpress.XtraEditors.Controls.EditorButtonImageOptions passwordButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
            DevExpress.Utils.SerializableAppearanceObject passwordButtonAppearance = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject passwordButtonHoveredAppearance = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject passwordButtonPressedAppearance = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject passwordButtonDisabledAppearance = new DevExpress.Utils.SerializableAppearanceObject();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.btnLogin = new DevExpress.XtraEditors.SimpleButton();
            this.txtUserId = new DevExpress.XtraEditors.TextEdit();
            this.txtPassword = new DevExpress.XtraEditors.ButtonEdit();
            this.lblApplicationName = new DevExpress.XtraEditors.LabelControl();
            this.lblVersion = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.txtUserId.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Appearance.BorderColor = System.Drawing.Color.Black;
            this.btnCancel.Appearance.Font = new System.Drawing.Font("DFKai-SB", 14F, System.Drawing.FontStyle.Regular);
            this.btnCancel.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(78)))), ((int)(((byte)(162)))));
            this.btnCancel.Appearance.Options.UseBorderColor = true;
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.Appearance.Options.UseForeColor = true;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(602, 340);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(110, 42);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "取消";
            // 
            // btnLogin
            // 
            this.btnLogin.Appearance.BorderColor = System.Drawing.Color.Black;
            this.btnLogin.Appearance.Font = new System.Drawing.Font("DFKai-SB", 14F, System.Drawing.FontStyle.Regular);
            this.btnLogin.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(78)))), ((int)(((byte)(162)))));
            this.btnLogin.Appearance.Options.UseBorderColor = true;
            this.btnLogin.Appearance.Options.UseFont = true;
            this.btnLogin.Appearance.Options.UseForeColor = true;
            this.btnLogin.Location = new System.Drawing.Point(485, 340);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(110, 42);
            this.btnLogin.TabIndex = 2;
            this.btnLogin.Text = "登入";
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // txtUserId
            // 
            this.txtUserId.Location = new System.Drawing.Point(500, 213);
            this.txtUserId.Name = "txtUserId";
            this.txtUserId.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.txtUserId.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.txtUserId.Properties.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(78)))), ((int)(((byte)(162)))));
            this.txtUserId.Properties.Appearance.Options.UseBackColor = true;
            this.txtUserId.Properties.Appearance.Options.UseFont = true;
            this.txtUserId.Properties.Appearance.Options.UseForeColor = true;
            this.txtUserId.Properties.AutoHeight = false;
            this.txtUserId.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.txtUserId.Properties.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtUserId.Properties.MaxLength = 10;
            this.txtUserId.Size = new System.Drawing.Size(197, 30);
            this.txtUserId.TabIndex = 0;
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(500, 278);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.txtPassword.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.txtPassword.Properties.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(78)))), ((int)(((byte)(162)))));
            this.txtPassword.Properties.Appearance.Options.UseBackColor = true;
            this.txtPassword.Properties.Appearance.Options.UseFont = true;
            this.txtPassword.Properties.Appearance.Options.UseForeColor = true;
            this.txtPassword.Properties.AutoHeight = false;
            this.txtPassword.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            passwordButtonImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("editorButtonImageOptions1.SvgImage")));
            passwordButtonImageOptions.SvgImageSize = new System.Drawing.Size(16, 16);
            this.txtPassword.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, passwordButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), passwordButtonAppearance, passwordButtonHoveredAppearance, passwordButtonPressedAppearance, passwordButtonDisabledAppearance, "顯示／隱藏密碼", null, null, DevExpress.Utils.ToolTipAnchor.Default)});
            this.txtPassword.Properties.MaxLength = 100;
            this.txtPassword.Properties.UseSystemPasswordChar = true;
            this.txtPassword.Size = new System.Drawing.Size(197, 30);
            this.txtPassword.TabIndex = 1;
            this.txtPassword.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtPassword_ButtonClick);
            // 
            // lblApplicationName
            // 
            this.lblApplicationName.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.lblApplicationName.Appearance.Font = new System.Drawing.Font("DFKai-SB", 32F, System.Drawing.FontStyle.Bold);
            this.lblApplicationName.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(43)))), ((int)(((byte)(64)))));
            this.lblApplicationName.Appearance.Options.UseBackColor = true;
            this.lblApplicationName.Appearance.Options.UseFont = true;
            this.lblApplicationName.Appearance.Options.UseForeColor = true;
            this.lblApplicationName.Appearance.Options.UseTextOptions = true;
            this.lblApplicationName.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblApplicationName.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.lblApplicationName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblApplicationName.Location = new System.Drawing.Point(12, 30);
            this.lblApplicationName.Name = "lblApplicationName";
            this.lblApplicationName.Size = new System.Drawing.Size(443, 56);
            this.lblApplicationName.TabIndex = 4;
            this.lblApplicationName.Text = global::Winform4System.ApplicationMetadata.DisplayName;
            // 
            // lblVersion
            // 
            this.lblVersion.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.lblVersion.Appearance.Font = new System.Drawing.Font("DFKai-SB", 11F, System.Drawing.FontStyle.Regular);
            this.lblVersion.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(75)))), ((int)(((byte)(105)))));
            this.lblVersion.Appearance.Options.UseBackColor = true;
            this.lblVersion.Appearance.Options.UseFont = true;
            this.lblVersion.Appearance.Options.UseForeColor = true;
            this.lblVersion.Appearance.Options.UseTextOptions = true;
            this.lblVersion.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblVersion.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.lblVersion.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblVersion.Location = new System.Drawing.Point(302, 86);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(153, 24);
            this.lblVersion.TabIndex = 5;
            this.lblVersion.Text = "版本 1.0.0.0";
            // 
            // LoginForm
            // 
            this.AcceptButton = this.btnLogin;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::Winform4System.Properties.Resources.LoginBackground;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(750, 450);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.lblApplicationName);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.txtUserId);
            this.Controls.Add(this.txtPassword);
            this.FormBorderEffect = DevExpress.XtraEditors.FormBorderEffect.Shadow;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.IconOptions.Icon = global::Winform4System.Properties.Resources.AppIcon;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(750, 450);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(750, 450);
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = global::Winform4System.ApplicationMetadata.LoginWindowTitle;
            this.Shown += new System.EventHandler(this.LoginForm_Shown);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.LoginForm_MouseUp);
            ((System.ComponentModel.ISupportInitialize)(this.txtUserId.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
