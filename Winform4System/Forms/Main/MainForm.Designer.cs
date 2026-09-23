namespace Winform4System.Forms.Main
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private DevExpress.XtraEditors.PanelControl headerPanel;
        private DevExpress.XtraEditors.LabelControl lblAppName;
        private DevExpress.XtraEditors.LabelControl lblGreeting;
        private DevExpress.XtraEditors.LabelControl lblWelcome;
        private DevExpress.XtraEditors.LabelControl lblSession;
        private DevExpress.XtraEditors.LabelControl lblLogout;
        private DevExpress.XtraEditors.TileControl tileMain;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.headerPanel = new DevExpress.XtraEditors.PanelControl();
            this.lblAppName = new DevExpress.XtraEditors.LabelControl();
            this.lblGreeting = new DevExpress.XtraEditors.LabelControl();
            this.lblWelcome = new DevExpress.XtraEditors.LabelControl();
            this.lblSession = new DevExpress.XtraEditors.LabelControl();
            this.lblLogout = new DevExpress.XtraEditors.LabelControl();
            this.tileMain = new DevExpress.XtraEditors.TileControl();
            ((System.ComponentModel.ISupportInitialize)(this.headerPanel)).BeginInit();
            this.headerPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // headerPanel
            // 
            this.headerPanel.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(43)))), ((int)(((byte)(64)))));
            this.headerPanel.Appearance.Options.UseBackColor = true;
            this.headerPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.headerPanel.Controls.Add(this.lblAppName);
            this.headerPanel.Controls.Add(this.lblGreeting);
            this.headerPanel.Controls.Add(this.lblWelcome);
            this.headerPanel.Controls.Add(this.lblLogout);
            this.headerPanel.Controls.Add(this.lblSession);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(1360, 72);
            this.headerPanel.TabIndex = 0;
            this.headerPanel.SizeChanged += new System.EventHandler(this.headerPanel_SizeChanged);
            // 
            // lblAppName
            // 
            this.lblAppName.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 32F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAppName.Appearance.ForeColor = System.Drawing.Color.White;
            this.lblAppName.Appearance.Options.UseFont = true;
            this.lblAppName.Appearance.Options.UseForeColor = true;
            this.lblAppName.Appearance.Options.UseTextOptions = true;
            this.lblAppName.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.lblAppName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblAppName.Location = new System.Drawing.Point(28, 0);
            this.lblAppName.Name = "lblAppName";
            this.lblAppName.Size = new System.Drawing.Size(350, 72);
            this.lblAppName.TabIndex = 0;
            this.lblAppName.Text = "軋鋼部系統";
            // 
            // lblGreeting
            // 
            this.lblGreeting.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblGreeting.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblGreeting.Appearance.ForeColor = System.Drawing.Color.White;
            this.lblGreeting.Appearance.Options.UseFont = true;
            this.lblGreeting.Appearance.Options.UseForeColor = true;
            this.lblGreeting.Location = new System.Drawing.Point(1172, 10);
            this.lblGreeting.Name = "lblGreeting";
            this.lblGreeting.Size = new System.Drawing.Size(38, 18);
            this.lblGreeting.TabIndex = 1;
            this.lblGreeting.Text = "您好，";
            // 
            // lblWelcome
            // 
            this.lblWelcome.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblWelcome.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblWelcome.Appearance.ForeColor = System.Drawing.Color.White;
            this.lblWelcome.Appearance.Options.UseFont = true;
            this.lblWelcome.Appearance.Options.UseForeColor = true;
            this.lblWelcome.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblWelcome.Location = new System.Drawing.Point(1210, 10);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.Size = new System.Drawing.Size(84, 18);
            this.lblWelcome.TabIndex = 1;
            this.lblWelcome.Text = "示範使用者";
            this.lblWelcome.Click += new System.EventHandler(this.lblWelcome_Click);
            // 
            // lblSession
            // 
            this.lblSession.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSession.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(205)))), ((int)(((byte)(222)))));
            this.lblSession.Appearance.Options.UseFont = true;
            this.lblSession.Appearance.Options.UseForeColor = true;
            this.lblSession.Appearance.Options.UseTextOptions = true;
            this.lblSession.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblSession.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.lblSession.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblSession.Location = new System.Drawing.Point(720, 38);
            this.lblSession.Name = "lblSession";
            this.lblSession.Size = new System.Drawing.Size(612, 22);
            this.lblSession.TabIndex = 2;
            this.lblSession.Text = "登入資訊";
            // 
            // lblLogout
            // 
            this.lblLogout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLogout.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblLogout.Appearance.ForeColor = System.Drawing.Color.LightCoral;
            this.lblLogout.Appearance.Options.UseFont = true;
            this.lblLogout.Appearance.Options.UseForeColor = true;
            this.lblLogout.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblLogout.Location = new System.Drawing.Point(1294, 10);
            this.lblLogout.Name = "lblLogout";
            this.lblLogout.Size = new System.Drawing.Size(38, 18);
            this.lblLogout.TabIndex = 2;
            this.lblLogout.Text = "，登出";
            this.lblLogout.Click += new System.EventHandler(this.lblLogout_Click);
            // 
            // tileMain
            // 
            this.tileMain.AllowDrag = false;
            this.tileMain.AllowDragTilesBetweenGroups = false;
            this.tileMain.AppearanceGroupText.Font = new System.Drawing.Font("Microsoft JhengHei UI", 18F, System.Drawing.FontStyle.Bold);
            this.tileMain.AppearanceGroupText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(55)))), ((int)(((byte)(72)))));
            this.tileMain.AppearanceGroupText.Options.UseFont = true;
            this.tileMain.AppearanceGroupText.Options.UseForeColor = true;
            this.tileMain.BackColor = System.Drawing.Color.Transparent;
            this.tileMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tileMain.Location = new System.Drawing.Point(0, 72);
            this.tileMain.Name = "tileMain";
            this.tileMain.Padding = new System.Windows.Forms.Padding(24, 20, 24, 20);
            this.tileMain.ShowGroupText = true;
            this.tileMain.Size = new System.Drawing.Size(1360, 696);
            this.tileMain.TabIndex = 1;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::Winform4System.Properties.Resources.MainBackgroundRollingMill;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1360, 768);
            this.Controls.Add(this.tileMain);
            this.Controls.Add(this.headerPanel);
            this.MinimumSize = new System.Drawing.Size(1024, 640);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Winform4System - 綜合管理系統";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.headerPanel)).EndInit();
            this.headerPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}
