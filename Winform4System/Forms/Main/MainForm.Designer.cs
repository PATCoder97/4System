namespace Winform4System.Forms.Main
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private DevExpress.XtraEditors.PanelControl headerPanel;
        private DevExpress.XtraEditors.LabelControl lblAppName;
        private DevExpress.XtraEditors.LabelControl lblWelcome;
        private DevExpress.XtraEditors.LabelControl lblSession;
        private DevExpress.XtraEditors.SimpleButton btnUser;
        private DevExpress.XtraEditors.SimpleButton btnExit;
        private DevExpress.XtraEditors.TileControl tileMain;
        private DevExpress.XtraEditors.PanelControl footerPanel;
        private DevExpress.XtraEditors.LabelControl lblStatus;

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
            this.lblWelcome = new DevExpress.XtraEditors.LabelControl();
            this.lblSession = new DevExpress.XtraEditors.LabelControl();
            this.btnUser = new DevExpress.XtraEditors.SimpleButton();
            this.btnExit = new DevExpress.XtraEditors.SimpleButton();
            this.tileMain = new DevExpress.XtraEditors.TileControl();
            this.footerPanel = new DevExpress.XtraEditors.PanelControl();
            this.lblStatus = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.headerPanel)).BeginInit();
            this.headerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.footerPanel)).BeginInit();
            this.footerPanel.SuspendLayout();
            this.SuspendLayout();
            //
            // headerPanel
            //
            this.headerPanel.Appearance.BackColor = System.Drawing.Color.FromArgb(20, 43, 64);
            this.headerPanel.Appearance.Options.UseBackColor = true;
            this.headerPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.headerPanel.Controls.Add(this.lblAppName);
            this.headerPanel.Controls.Add(this.lblWelcome);
            this.headerPanel.Controls.Add(this.lblSession);
            this.headerPanel.Controls.Add(this.btnUser);
            this.headerPanel.Controls.Add(this.btnExit);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(1360, 94);
            this.headerPanel.TabIndex = 0;
            //
            // lblAppName
            //
            this.lblAppName.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 22F, System.Drawing.FontStyle.Bold);
            this.lblAppName.Appearance.ForeColor = System.Drawing.Color.White;
            this.lblAppName.Appearance.Options.UseFont = true;
            this.lblAppName.Appearance.Options.UseForeColor = true;
            this.lblAppName.Location = new System.Drawing.Point(28, 15);
            this.lblAppName.Name = "lblAppName";
            this.lblAppName.Size = new System.Drawing.Size(294, 41);
            this.lblAppName.TabIndex = 0;
            this.lblAppName.Text = "WINFORM 4 SYSTEM";
            //
            // lblWelcome
            //
            this.lblWelcome.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.lblWelcome.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.lblWelcome.Appearance.ForeColor = System.Drawing.Color.White;
            this.lblWelcome.Appearance.Options.UseFont = true;
            this.lblWelcome.Appearance.Options.UseForeColor = true;
            this.lblWelcome.Appearance.Options.UseTextOptions = true;
            this.lblWelcome.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblWelcome.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblWelcome.Location = new System.Drawing.Point(732, 17);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.Size = new System.Drawing.Size(430, 25);
            this.lblWelcome.TabIndex = 1;
            this.lblWelcome.Text = "您好";
            //
            // lblSession
            //
            this.lblSession.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.lblSession.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSession.Appearance.ForeColor = System.Drawing.Color.FromArgb(180, 205, 222);
            this.lblSession.Appearance.Options.UseFont = true;
            this.lblSession.Appearance.Options.UseForeColor = true;
            this.lblSession.Appearance.Options.UseTextOptions = true;
            this.lblSession.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblSession.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblSession.Location = new System.Drawing.Point(640, 47);
            this.lblSession.Name = "lblSession";
            this.lblSession.Size = new System.Drawing.Size(522, 22);
            this.lblSession.TabIndex = 2;
            this.lblSession.Text = "登入資訊";
            //
            // btnUser
            //
            this.btnUser.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.btnUser.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.btnUser.Appearance.Options.UseFont = true;
            this.btnUser.Location = new System.Drawing.Point(1178, 21);
            this.btnUser.Name = "btnUser";
            this.btnUser.Size = new System.Drawing.Size(92, 42);
            this.btnUser.TabIndex = 3;
            this.btnUser.Text = "帳戶";
            this.btnUser.Click += new System.EventHandler(this.btnUser_Click);
            //
            // btnExit
            //
            this.btnExit.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.btnExit.Appearance.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.btnExit.Appearance.Options.UseFont = true;
            this.btnExit.Location = new System.Drawing.Point(1276, 21);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(64, 42);
            this.btnExit.TabIndex = 4;
            this.btnExit.Text = "離開";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            //
            // tileMain
            //
            this.tileMain.AllowDrag = false;
            this.tileMain.AllowDragTilesBetweenGroups = false;
            this.tileMain.AppearanceGroupText.Font = new System.Drawing.Font("Segoe UI Semibold", 18F, System.Drawing.FontStyle.Bold);
            this.tileMain.AppearanceGroupText.ForeColor = System.Drawing.Color.FromArgb(35, 55, 72);
            this.tileMain.AppearanceGroupText.Options.UseFont = true;
            this.tileMain.AppearanceGroupText.Options.UseForeColor = true;
            this.tileMain.BackColor = System.Drawing.Color.FromArgb(241, 246, 249);
            this.tileMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tileMain.Location = new System.Drawing.Point(0, 94);
            this.tileMain.Name = "tileMain";
            this.tileMain.Padding = new System.Windows.Forms.Padding(24, 20, 24, 20);
            this.tileMain.ShowGroupText = true;
            this.tileMain.Size = new System.Drawing.Size(1360, 634);
            this.tileMain.TabIndex = 1;
            //
            // footerPanel
            //
            this.footerPanel.Appearance.BackColor = System.Drawing.Color.FromArgb(226, 235, 241);
            this.footerPanel.Appearance.Options.UseBackColor = true;
            this.footerPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.footerPanel.Controls.Add(this.lblStatus);
            this.footerPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.footerPanel.Location = new System.Drawing.Point(0, 728);
            this.footerPanel.Name = "footerPanel";
            this.footerPanel.Size = new System.Drawing.Size(1360, 40);
            this.footerPanel.TabIndex = 2;
            //
            // lblStatus
            //
            this.lblStatus.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblStatus.Appearance.ForeColor = System.Drawing.Color.FromArgb(70, 90, 105);
            this.lblStatus.Appearance.Options.UseFont = true;
            this.lblStatus.Appearance.Options.UseForeColor = true;
            this.lblStatus.Location = new System.Drawing.Point(28, 11);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(45, 15);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "系統就緒";
            //
            // MainForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1360, 768);
            this.Controls.Add(this.tileMain);
            this.Controls.Add(this.footerPanel);
            this.Controls.Add(this.headerPanel);
            this.MinimumSize = new System.Drawing.Size(1024, 640);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Winform4System - 綜合管理系統";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.headerPanel)).EndInit();
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.footerPanel)).EndInit();
            this.footerPanel.ResumeLayout(false);
            this.footerPanel.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}
