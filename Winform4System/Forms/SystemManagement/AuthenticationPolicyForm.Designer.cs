namespace Winform4System.Forms.SystemManagement
{
    partial class AuthenticationPolicyForm
    {
        private System.ComponentModel.IContainer components;
        private DevExpress.XtraBars.BarManager barManager;
        private DevExpress.XtraBars.Bar barCommands;
        private DevExpress.XtraBars.BarButtonItem btnSave;
        private DevExpress.XtraBars.BarButtonItem btnCancel;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraLayout.LayoutControl layoutControl;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraEditors.SpinEdit spinMaximumFailedAttempts;
        private DevExpress.XtraEditors.SpinEdit spinLockoutMinutes;
        private DevExpress.XtraEditors.LabelControl lblExplanation;
        private DevExpress.XtraLayout.LayoutControlItem layoutMaximumFailedAttempts;
        private DevExpress.XtraLayout.LayoutControlItem layoutLockoutMinutes;
        private DevExpress.XtraLayout.LayoutControlItem layoutExplanation;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpace;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.barManager = new DevExpress.XtraBars.BarManager(this.components);
            this.barCommands = new DevExpress.XtraBars.Bar();
            this.btnSave = new DevExpress.XtraBars.BarButtonItem();
            this.btnCancel = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.layoutControl = new DevExpress.XtraLayout.LayoutControl();
            this.spinMaximumFailedAttempts = new DevExpress.XtraEditors.SpinEdit();
            this.spinLockoutMinutes = new DevExpress.XtraEditors.SpinEdit();
            this.lblExplanation = new DevExpress.XtraEditors.LabelControl();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutMaximumFailedAttempts = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutLockoutMinutes = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutExplanation = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpace = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this.barManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
            this.layoutControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinMaximumFailedAttempts.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinLockoutMinutes.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutMaximumFailedAttempts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutLockoutMinutes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutExplanation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpace)).BeginInit();
            this.SuspendLayout();
            //
            // barManager
            //
            this.barManager.Bars.AddRange(new DevExpress.XtraBars.Bar[] { this.barCommands });
            this.barManager.DockControls.Add(this.barDockControlTop);
            this.barManager.DockControls.Add(this.barDockControlBottom);
            this.barManager.DockControls.Add(this.barDockControlLeft);
            this.barManager.DockControls.Add(this.barDockControlRight);
            this.barManager.Form = this;
            this.barManager.Items.AddRange(new DevExpress.XtraBars.BarItem[] { this.btnSave, this.btnCancel });
            this.barManager.MainMenu = this.barCommands;
            this.barManager.MaxItemId = 2;
            //
            // barCommands
            //
            this.barCommands.BarName = "帳號安全設定工具列";
            this.barCommands.DockCol = 0;
            this.barCommands.DockRow = 0;
            this.barCommands.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.barCommands.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] { new DevExpress.XtraBars.LinkPersistInfo(this.btnSave), new DevExpress.XtraBars.LinkPersistInfo(this.btnCancel) });
            this.barCommands.OptionsBar.AllowQuickCustomization = false;
            this.barCommands.OptionsBar.DrawDragBorder = false;
            this.barCommands.OptionsBar.UseWholeRow = true;
            this.barCommands.BarAppearance.Disabled.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.barCommands.BarAppearance.Disabled.Options.UseFont = true;
            this.barCommands.BarAppearance.Hovered.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.barCommands.BarAppearance.Hovered.Options.UseFont = true;
            this.barCommands.BarAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.barCommands.BarAppearance.Normal.Options.UseFont = true;
            this.barCommands.BarAppearance.Pressed.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.barCommands.BarAppearance.Pressed.Options.UseFont = true;
            //
            // btnSave
            //
            this.btnSave.Caption = "儲存";
            this.btnSave.Id = 0;
            this.btnSave.ImageOptions.SvgImage = Winform4System.Helpers.SvgIconCatalog.Confirm;
            this.btnSave.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnSave.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.btnSave.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnSave_ItemClick);
            //
            // btnCancel
            //
            this.btnCancel.Caption = "取消";
            this.btnCancel.Id = 1;
            this.btnCancel.ImageOptions.SvgImage = Winform4System.Helpers.SvgIconCatalog.Cancel;
            this.btnCancel.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnCancel.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.btnCancel.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnCancel_ItemClick);
            //
            // dock controls
            //
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Manager = this.barManager;
            this.barDockControlTop.Size = new System.Drawing.Size(620, 49);
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Manager = this.barManager;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Manager = this.barManager;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Manager = this.barManager;
            //
            // layoutControl
            //
            this.layoutControl.AllowCustomization = false;
            this.layoutControl.Controls.Add(this.spinMaximumFailedAttempts);
            this.layoutControl.Controls.Add(this.spinLockoutMinutes);
            this.layoutControl.Controls.Add(this.lblExplanation);
            this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl.Location = new System.Drawing.Point(0, 49);
            this.layoutControl.Name = "layoutControl";
            this.layoutControl.Root = this.Root;
            this.layoutControl.Size = new System.Drawing.Size(620, 231);
            //
            // spinMaximumFailedAttempts
            //
            this.spinMaximumFailedAttempts.Location = new System.Drawing.Point(244, 12);
            this.spinMaximumFailedAttempts.Name = "spinMaximumFailedAttempts";
            this.spinMaximumFailedAttempts.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.spinMaximumFailedAttempts.Properties.Appearance.Options.UseFont = true;
            this.spinMaximumFailedAttempts.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            this.spinMaximumFailedAttempts.Properties.IsFloatValue = false;
            this.spinMaximumFailedAttempts.Properties.MaskSettings.Set("mask", "d");
            this.spinMaximumFailedAttempts.Properties.MinValue = 1;
            this.spinMaximumFailedAttempts.Properties.MaxValue = 20;
            this.spinMaximumFailedAttempts.Size = new System.Drawing.Size(364, 32);
            this.spinMaximumFailedAttempts.StyleController = this.layoutControl;
            //
            // spinLockoutMinutes
            //
            this.spinLockoutMinutes.Location = new System.Drawing.Point(244, 48);
            this.spinLockoutMinutes.Name = "spinLockoutMinutes";
            this.spinLockoutMinutes.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.spinLockoutMinutes.Properties.Appearance.Options.UseFont = true;
            this.spinLockoutMinutes.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            this.spinLockoutMinutes.Properties.IsFloatValue = false;
            this.spinLockoutMinutes.Properties.MaskSettings.Set("mask", "d");
            this.spinLockoutMinutes.Properties.MinValue = 1;
            this.spinLockoutMinutes.Properties.MaxValue = 1440;
            this.spinLockoutMinutes.Size = new System.Drawing.Size(364, 32);
            this.spinLockoutMinutes.StyleController = this.layoutControl;
            //
            // lblExplanation
            //
            this.lblExplanation.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.lblExplanation.Appearance.ForeColor = System.Drawing.Color.DimGray;
            this.lblExplanation.Appearance.Options.UseFont = true;
            this.lblExplanation.Appearance.Options.UseForeColor = true;
            this.lblExplanation.Appearance.Options.UseTextOptions = true;
            this.lblExplanation.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lblExplanation.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.lblExplanation.Location = new System.Drawing.Point(12, 84);
            this.lblExplanation.Name = "lblExplanation";
            this.lblExplanation.Size = new System.Drawing.Size(596, 46);
            this.lblExplanation.StyleController = this.layoutControl;
            this.lblExplanation.Text = "此設定僅控制應用程式的登入失敗鎖定機制，不會變更或管理公司網域密碼。";
            //
            // Root
            //
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { this.layoutMaximumFailedAttempts, this.layoutLockoutMinutes, this.layoutExplanation, this.emptySpace });
            this.Root.Size = new System.Drawing.Size(620, 231);
            //
            // layout items
            //
            this.layoutMaximumFailedAttempts.Control = this.spinMaximumFailedAttempts;
            this.layoutMaximumFailedAttempts.Location = new System.Drawing.Point(0, 0);
            this.layoutMaximumFailedAttempts.Size = new System.Drawing.Size(600, 36);
            this.layoutMaximumFailedAttempts.Text = "登入失敗次數上限";
            this.layoutMaximumFailedAttempts.TextSize = new System.Drawing.Size(220, 24);
            this.layoutMaximumFailedAttempts.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.layoutMaximumFailedAttempts.AppearanceItemCaption.Options.UseFont = true;
            this.layoutLockoutMinutes.Control = this.spinLockoutMinutes;
            this.layoutLockoutMinutes.Location = new System.Drawing.Point(0, 36);
            this.layoutLockoutMinutes.Size = new System.Drawing.Size(600, 36);
            this.layoutLockoutMinutes.Text = "帳號鎖定時間（分鐘）";
            this.layoutLockoutMinutes.TextSize = new System.Drawing.Size(220, 24);
            this.layoutLockoutMinutes.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.layoutLockoutMinutes.AppearanceItemCaption.Options.UseFont = true;
            this.layoutExplanation.Control = this.lblExplanation;
            this.layoutExplanation.Location = new System.Drawing.Point(0, 72);
            this.layoutExplanation.Size = new System.Drawing.Size(600, 50);
            this.layoutExplanation.TextVisible = false;
            this.emptySpace.AllowHotTrack = false;
            this.emptySpace.Location = new System.Drawing.Point(0, 122);
            this.emptySpace.Size = new System.Drawing.Size(600, 89);
            //
            // AuthenticationPolicyForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(620, 280);
            this.Controls.Add(this.layoutControl);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.IconOptions.Icon = global::Winform4System.Properties.Resources.AppIcon;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AuthenticationPolicyForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "帳號安全設定";
            ((System.ComponentModel.ISupportInitialize)(this.barManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
            this.layoutControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spinMaximumFailedAttempts.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinLockoutMinutes.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutMaximumFailedAttempts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutLockoutMinutes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutExplanation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpace)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
