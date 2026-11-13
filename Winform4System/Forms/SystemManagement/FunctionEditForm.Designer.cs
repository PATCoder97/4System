namespace Winform4System.Forms.SystemManagement
{
    partial class FunctionEditForm
    {
        private System.ComponentModel.IContainer components = null;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraEditors.TextEdit txbCode;
        private DevExpress.XtraEditors.TextEdit txbDisplayName;
        private DevExpress.XtraEditors.LookUpEdit cbbParent;
        private DevExpress.XtraEditors.LookUpEdit txbNavigationTarget;
        private DevExpress.XtraEditors.ImageComboBoxEdit cbbIcon;
        private DevExpress.Utils.SvgImageCollection iconImages;
        private DevExpress.XtraLayout.LayoutControlItem layoutIcon;
        private DevExpress.XtraEditors.SpinEdit speSortOrder;
        private DevExpress.XtraEditors.ComboBoxEdit cbbDevelopmentStatus;
        private DevExpress.XtraEditors.MemoEdit memoDescription;
        private DevExpress.XtraEditors.CheckEdit chkWide;
        private DevExpress.XtraEditors.CheckEdit chkVisible;
        private DevExpress.XtraEditors.CheckEdit chkActive;
        private DevExpress.XtraBars.BarManager barManagerTP;
        private DevExpress.XtraBars.Bar barCommands;
        private DevExpress.XtraBars.BarButtonItem btnSave;
        private DevExpress.XtraBars.BarButtonItem btnCancel;
        private DevExpress.XtraBars.BarButtonItem btnEdit;
        private DevExpress.XtraBars.BarButtonItem btnDelete;
        private DevExpress.XtraLayout.LayoutControlItem layoutCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutDisplayName;
        private DevExpress.XtraLayout.LayoutControlItem layoutParent;
        private DevExpress.XtraLayout.LayoutControlItem layoutNavigationTarget;
        private DevExpress.XtraLayout.LayoutControlItem layoutSortOrder;
        private DevExpress.XtraLayout.LayoutControlItem layoutDevelopmentStatus;
        private DevExpress.XtraLayout.LayoutControlItem layoutDescription;
        private DevExpress.XtraLayout.LayoutControlItem layoutWide;
        private DevExpress.XtraLayout.LayoutControlItem layoutVisible;
        private DevExpress.XtraLayout.LayoutControlItem layoutActive;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpace;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FunctionEditForm));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.txbCode = new DevExpress.XtraEditors.TextEdit();
            this.txbDisplayName = new DevExpress.XtraEditors.TextEdit();
            this.cbbParent = new DevExpress.XtraEditors.LookUpEdit();
            this.txbNavigationTarget = new DevExpress.XtraEditors.LookUpEdit();
            this.cbbIcon = new DevExpress.XtraEditors.ImageComboBoxEdit();
            this.iconImages = new DevExpress.Utils.SvgImageCollection(this.components);
            this.layoutIcon = new DevExpress.XtraLayout.LayoutControlItem();
            this.speSortOrder = new DevExpress.XtraEditors.SpinEdit();
            this.cbbDevelopmentStatus = new DevExpress.XtraEditors.ComboBoxEdit();
            this.memoDescription = new DevExpress.XtraEditors.MemoEdit();
            this.chkWide = new DevExpress.XtraEditors.CheckEdit();
            this.chkVisible = new DevExpress.XtraEditors.CheckEdit();
            this.chkActive = new DevExpress.XtraEditors.CheckEdit();
            this.barManagerTP = new DevExpress.XtraBars.BarManager(this.components);
            this.barCommands = new DevExpress.XtraBars.Bar();
            this.btnSave = new DevExpress.XtraBars.BarButtonItem();
            this.btnCancel = new DevExpress.XtraBars.BarButtonItem();
            this.btnEdit = new DevExpress.XtraBars.BarButtonItem();
            this.btnDelete = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutDisplayName = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutParent = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutNavigationTarget = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutSortOrder = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutDevelopmentStatus = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutDescription = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutWide = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutVisible = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutActive = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpace = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbbIcon.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImages)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbDisplayName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbParent.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbNavigationTarget.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.speSortOrder.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbDevelopmentStatus.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.memoDescription.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkWide.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkVisible.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkActive.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutDisplayName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutParent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutNavigationTarget)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutSortOrder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutDevelopmentStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutDescription)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutWide)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutVisible)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutActive)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpace)).BeginInit();
            this.SuspendLayout();
            //
            // layoutControl1
            //
            this.layoutControl1.Controls.Add(this.txbCode);
            this.layoutControl1.Controls.Add(this.txbDisplayName);
            this.layoutControl1.Controls.Add(this.cbbParent);
            this.layoutControl1.Controls.Add(this.txbNavigationTarget);
            this.layoutControl1.Controls.Add(this.cbbIcon);
            this.layoutControl1.Controls.Add(this.speSortOrder);
            this.layoutControl1.Controls.Add(this.cbbDevelopmentStatus);
            this.layoutControl1.Controls.Add(this.memoDescription);
            this.layoutControl1.Controls.Add(this.chkWide);
            this.layoutControl1.Controls.Add(this.chkVisible);
            this.layoutControl1.Controls.Add(this.chkActive);
            this.layoutControl1.AllowCustomization = false;
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 49);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(720, 473);
            this.layoutControl1.TabIndex = 0;
            //
            // txbCode
            //
            this.txbCode.Location = new System.Drawing.Point(100, 12);
            this.txbCode.Name = "txbCode";
            this.txbCode.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.txbCode.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txbCode.Properties.Appearance.Options.UseFont = true;
            this.txbCode.Properties.Appearance.Options.UseForeColor = true;
            this.txbCode.Properties.MaxLength = 100;
            this.txbCode.Properties.ReadOnly = true;
            this.txbCode.Size = new System.Drawing.Size(608, 32);
            this.txbCode.StyleController = this.layoutControl1;
            this.txbCode.TabIndex = 0;
            //
            // txbDisplayName
            //
            this.txbDisplayName.Location = new System.Drawing.Point(100, 48);
            this.txbDisplayName.Name = "txbDisplayName";
            this.txbDisplayName.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.txbDisplayName.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txbDisplayName.Properties.Appearance.Options.UseFont = true;
            this.txbDisplayName.Properties.Appearance.Options.UseForeColor = true;
            this.txbDisplayName.Properties.MaxLength = 200;
            this.txbDisplayName.Size = new System.Drawing.Size(608, 32);
            this.txbDisplayName.StyleController = this.layoutControl1;
            this.txbDisplayName.TabIndex = 1;
            //
            // cbbParent
            //
            this.cbbParent.Location = new System.Drawing.Point(100, 84);
            this.cbbParent.Name = "cbbParent";
            this.cbbParent.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.cbbParent.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.cbbParent.Properties.Appearance.Options.UseFont = true;
            this.cbbParent.Properties.Appearance.Options.UseForeColor = true;
            this.cbbParent.Properties.AppearanceDropDown.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.cbbParent.Properties.AppearanceDropDown.Options.UseFont = true;
            this.cbbParent.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Clear),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbbParent.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo("DisplayName", "上層功能")});
            this.cbbParent.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cbbParent_ButtonClick);
            this.cbbParent.Properties.ShowHeader = false;
            this.cbbParent.Properties.NullText = "（頂層群組）";
            this.cbbParent.Size = new System.Drawing.Size(608, 32);
            this.cbbParent.StyleController = this.layoutControl1;
            this.cbbParent.TabIndex = 2;
            //
            // txbNavigationTarget
            //
            this.txbNavigationTarget.Location = new System.Drawing.Point(100, 120);
            this.txbNavigationTarget.Name = "txbNavigationTarget";
            this.txbNavigationTarget.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.txbNavigationTarget.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txbNavigationTarget.Properties.Appearance.Options.UseFont = true;
            this.txbNavigationTarget.Properties.Appearance.Options.UseForeColor = true;
            this.txbNavigationTarget.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            this.txbNavigationTarget.Properties.NullText = "群組（不開啟畫面）";
            this.txbNavigationTarget.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.txbNavigationTarget.Properties.ShowHeader = false;
            this.txbNavigationTarget.Properties.AppearanceDropDown.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.txbNavigationTarget.Properties.AppearanceDropDown.Options.UseFont = true;
            this.txbNavigationTarget.Size = new System.Drawing.Size(608, 32);
            this.txbNavigationTarget.StyleController = this.layoutControl1;
            this.txbNavigationTarget.TabIndex = 3;
            this.cbbIcon.Name = "cbbIcon";
            this.cbbIcon.StyleController = this.layoutControl1;
            this.cbbIcon.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.cbbIcon.Properties.Appearance.Options.UseFont = true;
            this.cbbIcon.Properties.AppearanceDropDown.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.cbbIcon.Properties.AppearanceDropDown.Options.UseFont = true;
            this.cbbIcon.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            this.cbbIcon.Properties.SmallImages = this.iconImages;
            this.cbbIcon.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.layoutIcon.Control = this.cbbIcon;
            this.layoutIcon.Name = "layoutIcon";
            this.layoutIcon.Text = "功能圖示";
            this.layoutIcon.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.layoutIcon.AppearanceItemCaption.Options.UseFont = true;
            this.layoutIcon.Location = new System.Drawing.Point(0, 144);
            this.layoutIcon.Size = new System.Drawing.Size(700, 36);
            //
            // speSortOrder
            //
            this.speSortOrder.EditValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.speSortOrder.Location = new System.Drawing.Point(100, 192);
            this.speSortOrder.Name = "speSortOrder";
            this.speSortOrder.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.speSortOrder.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.speSortOrder.Properties.Appearance.Options.UseFont = true;
            this.speSortOrder.Properties.Appearance.Options.UseForeColor = true;
            this.speSortOrder.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.speSortOrder.Properties.IsFloatValue = false;
            this.speSortOrder.Properties.MaskSettings.Set("mask", "d");
            this.speSortOrder.Properties.MaxValue = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.speSortOrder.Size = new System.Drawing.Size(608, 32);
            this.speSortOrder.StyleController = this.layoutControl1;
            this.speSortOrder.TabIndex = 4;
            //
            // cbbDevelopmentStatus
            //
            this.cbbDevelopmentStatus.Location = new System.Drawing.Point(100, 228);
            this.cbbDevelopmentStatus.Name = "cbbDevelopmentStatus";
            this.cbbDevelopmentStatus.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.cbbDevelopmentStatus.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.cbbDevelopmentStatus.Properties.Appearance.Options.UseFont = true;
            this.cbbDevelopmentStatus.Properties.Appearance.Options.UseForeColor = true;
            this.cbbDevelopmentStatus.Properties.AppearanceDropDown.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.cbbDevelopmentStatus.Properties.AppearanceDropDown.Options.UseFont = true;
            this.cbbDevelopmentStatus.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbbDevelopmentStatus.Properties.Items.AddRange(new object[] {
            "尚未開始",
            "開發中",
            "已完成"});
            this.cbbDevelopmentStatus.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cbbDevelopmentStatus.Size = new System.Drawing.Size(608, 32);
            this.cbbDevelopmentStatus.StyleController = this.layoutControl1;
            this.cbbDevelopmentStatus.TabIndex = 5;
            //
            // memoDescription
            //
            this.memoDescription.Location = new System.Drawing.Point(100, 264);
            this.memoDescription.Name = "memoDescription";
            this.memoDescription.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.memoDescription.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.memoDescription.Properties.Appearance.Options.UseFont = true;
            this.memoDescription.Properties.Appearance.Options.UseForeColor = true;
            this.memoDescription.Properties.MaxLength = 500;
            this.memoDescription.Size = new System.Drawing.Size(608, 112);
            this.memoDescription.StyleController = this.layoutControl1;
            this.memoDescription.TabIndex = 6;
            //
            // chkWide
            //
            this.chkWide.Location = new System.Drawing.Point(12, 380);
            this.chkWide.Name = "chkWide";
            this.chkWide.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.chkWide.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.chkWide.Properties.Appearance.Options.UseFont = true;
            this.chkWide.Properties.Appearance.Options.UseForeColor = true;
            this.chkWide.Properties.Caption = "使用寬版卡片";
            this.chkWide.Size = new System.Drawing.Size(696, 32);
            this.chkWide.StyleController = this.layoutControl1;
            this.chkWide.TabIndex = 7;
            //
            // chkVisible
            //
            this.chkVisible.Location = new System.Drawing.Point(12, 416);
            this.chkVisible.Name = "chkVisible";
            this.chkVisible.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.chkVisible.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.chkVisible.Properties.Appearance.Options.UseFont = true;
            this.chkVisible.Properties.Appearance.Options.UseForeColor = true;
            this.chkVisible.Properties.Caption = "顯示於主畫面或模組選單";
            this.chkVisible.Size = new System.Drawing.Size(696, 32);
            this.chkVisible.StyleController = this.layoutControl1;
            this.chkVisible.TabIndex = 8;
            //
            // chkActive
            //
            this.chkActive.Location = new System.Drawing.Point(12, 452);
            this.chkActive.Name = "chkActive";
            this.chkActive.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.chkActive.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.chkActive.Properties.Appearance.Options.UseFont = true;
            this.chkActive.Properties.Appearance.Options.UseForeColor = true;
            this.chkActive.Properties.Caption = "啟用此功能";
            this.chkActive.Size = new System.Drawing.Size(696, 32);
            this.chkActive.StyleController = this.layoutControl1;
            this.chkActive.TabIndex = 9;
            //
            // barManagerTP
            //
            this.barManagerTP.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.barCommands});
            this.barManagerTP.DockControls.Add(this.barDockControlTop);
            this.barManagerTP.DockControls.Add(this.barDockControlBottom);
            this.barManagerTP.DockControls.Add(this.barDockControlLeft);
            this.barManagerTP.DockControls.Add(this.barDockControlRight);
            this.barManagerTP.Form = this;
            this.barManagerTP.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.btnSave,
            this.btnCancel, this.btnEdit, this.btnDelete});
            this.barManagerTP.MainMenu = this.barCommands;
            this.barManagerTP.MaxItemId = 4;
            //
            // barCommands
            //
            this.barCommands.BarAppearance.Disabled.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.barCommands.BarAppearance.Disabled.Options.UseFont = true;
            this.barCommands.BarAppearance.Hovered.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.barCommands.BarAppearance.Hovered.ForeColor = System.Drawing.Color.Black;
            this.barCommands.BarAppearance.Hovered.Options.UseFont = true;
            this.barCommands.BarAppearance.Hovered.Options.UseForeColor = true;
            this.barCommands.BarAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.barCommands.BarAppearance.Normal.ForeColor = System.Drawing.Color.Black;
            this.barCommands.BarAppearance.Normal.Options.UseFont = true;
            this.barCommands.BarAppearance.Normal.Options.UseForeColor = true;
            this.barCommands.BarAppearance.Pressed.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.barCommands.BarAppearance.Pressed.ForeColor = System.Drawing.Color.Black;
            this.barCommands.BarAppearance.Pressed.Options.UseFont = true;
            this.barCommands.BarAppearance.Pressed.Options.UseForeColor = true;
            this.barCommands.BarName = "Main menu";
            this.barCommands.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Top;
            this.barCommands.DockCol = 0;
            this.barCommands.DockRow = 0;
            this.barCommands.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.barCommands.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnEdit, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnDelete, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnSave, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnCancel, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph)});
            this.barCommands.OptionsBar.AllowQuickCustomization = false;
            this.barCommands.OptionsBar.DrawDragBorder = false;
            this.barCommands.OptionsBar.MultiLine = true;
            this.barCommands.OptionsBar.UseWholeRow = true;
            this.barCommands.Text = "主選單";
            //
            // btnSave
            //
            this.btnSave.Caption = "儲存";
            this.btnSave.Id = 0;
            this.btnSave.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnSave.ImageOptions.SvgImage")));
            this.btnSave.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnSave.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Blue;
            this.btnSave.ItemAppearance.Hovered.Options.UseForeColor = true;
            this.btnSave.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.btnSave.ItemAppearance.Normal.Options.UseFont = true;
            this.btnSave.Name = "btnSave";
            this.btnSave.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnSave_ItemClick);
            //
            // btnCancel
            //
            this.btnCancel.Caption = "取消";
            this.btnCancel.Id = 1;
            this.btnCancel.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnCancel.ImageOptions.SvgImage")));
            this.btnCancel.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnCancel.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Blue;
            this.btnCancel.ItemAppearance.Hovered.Options.UseForeColor = true;
            this.btnCancel.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.btnCancel.ItemAppearance.Normal.Options.UseFont = true;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnCancel_ItemClick);
            this.btnEdit.Caption = "編輯";
            this.btnEdit.Id = 2;
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.ImageOptions.SvgImage = Winform4System.Helpers.SvgIconCatalog.Edit;
            this.btnEdit.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnEdit.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnEdit_ItemClick);
            this.btnDelete.Caption = "刪除";
            this.btnDelete.Id = 3;
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.ImageOptions.SvgImage = Winform4System.Helpers.SvgIconCatalog.Delete;
            this.btnDelete.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnDelete.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnDelete_ItemClick);
            this.btnEdit.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.btnEdit.ItemAppearance.Normal.Options.UseFont = true;
            this.btnEdit.ItemAppearance.Normal.ForeColor = System.Drawing.Color.Black;
            this.btnEdit.ItemAppearance.Normal.Options.UseForeColor = true;
            this.btnEdit.ItemAppearance.Hovered.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.btnEdit.ItemAppearance.Hovered.Options.UseFont = true;
            this.btnEdit.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Black;
            this.btnEdit.ItemAppearance.Hovered.Options.UseForeColor = true;
            this.btnEdit.ItemAppearance.Pressed.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.btnEdit.ItemAppearance.Pressed.Options.UseFont = true;
            this.btnEdit.ItemAppearance.Pressed.ForeColor = System.Drawing.Color.Black;
            this.btnEdit.ItemAppearance.Pressed.Options.UseForeColor = true;
            this.btnEdit.ItemAppearance.Disabled.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.btnEdit.ItemAppearance.Disabled.Options.UseFont = true;
            this.btnDelete.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.btnDelete.ItemAppearance.Normal.Options.UseFont = true;
            this.btnDelete.ItemAppearance.Normal.ForeColor = System.Drawing.Color.Black;
            this.btnDelete.ItemAppearance.Normal.Options.UseForeColor = true;
            this.btnDelete.ItemAppearance.Hovered.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.btnDelete.ItemAppearance.Hovered.Options.UseFont = true;
            this.btnDelete.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Black;
            this.btnDelete.ItemAppearance.Hovered.Options.UseForeColor = true;
            this.btnDelete.ItemAppearance.Pressed.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.btnDelete.ItemAppearance.Pressed.Options.UseFont = true;
            this.btnDelete.ItemAppearance.Pressed.ForeColor = System.Drawing.Color.Black;
            this.btnDelete.ItemAppearance.Pressed.Options.UseForeColor = true;
            this.btnDelete.ItemAppearance.Disabled.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.btnDelete.ItemAppearance.Disabled.Options.UseFont = true;
            this.btnSave.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.btnSave.ItemAppearance.Normal.Options.UseFont = true;
            this.btnSave.ItemAppearance.Normal.ForeColor = System.Drawing.Color.Black;
            this.btnSave.ItemAppearance.Normal.Options.UseForeColor = true;
            this.btnSave.ItemAppearance.Hovered.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.btnSave.ItemAppearance.Hovered.Options.UseFont = true;
            this.btnSave.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Black;
            this.btnSave.ItemAppearance.Hovered.Options.UseForeColor = true;
            this.btnSave.ItemAppearance.Pressed.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.btnSave.ItemAppearance.Pressed.Options.UseFont = true;
            this.btnSave.ItemAppearance.Pressed.ForeColor = System.Drawing.Color.Black;
            this.btnSave.ItemAppearance.Pressed.Options.UseForeColor = true;
            this.btnSave.ItemAppearance.Disabled.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.btnSave.ItemAppearance.Disabled.Options.UseFont = true;
            this.btnCancel.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.btnCancel.ItemAppearance.Normal.Options.UseFont = true;
            this.btnCancel.ItemAppearance.Normal.ForeColor = System.Drawing.Color.Black;
            this.btnCancel.ItemAppearance.Normal.Options.UseForeColor = true;
            this.btnCancel.ItemAppearance.Hovered.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.btnCancel.ItemAppearance.Hovered.Options.UseFont = true;
            this.btnCancel.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Black;
            this.btnCancel.ItemAppearance.Hovered.Options.UseForeColor = true;
            this.btnCancel.ItemAppearance.Pressed.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.btnCancel.ItemAppearance.Pressed.Options.UseFont = true;
            this.btnCancel.ItemAppearance.Pressed.ForeColor = System.Drawing.Color.Black;
            this.btnCancel.ItemAppearance.Pressed.Options.UseForeColor = true;
            this.btnCancel.ItemAppearance.Disabled.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.btnCancel.ItemAppearance.Disabled.Options.UseFont = true;
            //
            // barDockControlTop
            //
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManagerTP;
            this.barDockControlTop.Size = new System.Drawing.Size(720, 49);
            //
            // barDockControlBottom
            //
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 571);
            this.barDockControlBottom.Manager = this.barManagerTP;
            this.barDockControlBottom.Size = new System.Drawing.Size(720, 0);
            //
            // barDockControlLeft
            //
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 49);
            this.barDockControlLeft.Manager = this.barManagerTP;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 522);
            //
            // barDockControlRight
            //
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(720, 49);
            this.barDockControlRight.Manager = this.barManagerTP;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 522);
            //
            // Root
            //
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutCode,
            this.layoutDisplayName,
            this.layoutParent,
            this.layoutNavigationTarget,
            this.layoutIcon,
            this.layoutSortOrder,
            this.layoutDevelopmentStatus,
            this.layoutDescription,
            this.layoutWide,
            this.layoutVisible,
            this.layoutActive,
            this.emptySpace});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(720, 473);
            this.Root.TextVisible = false;
            //
            // layoutCode
            //
            this.layoutCode.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.layoutCode.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.layoutCode.AppearanceItemCaption.Options.UseFont = true;
            this.layoutCode.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutCode.Control = this.txbCode;
            this.layoutCode.Location = new System.Drawing.Point(0, 0);
            this.layoutCode.Name = "layoutCode";
            this.layoutCode.Size = new System.Drawing.Size(700, 36);
            this.layoutCode.Text = "功能代碼";
            this.layoutCode.TextSize = new System.Drawing.Size(76, 24);
            //
            // layoutDisplayName
            //
            this.layoutDisplayName.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.layoutDisplayName.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.layoutDisplayName.AppearanceItemCaption.Options.UseFont = true;
            this.layoutDisplayName.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutDisplayName.Control = this.txbDisplayName;
            this.layoutDisplayName.Location = new System.Drawing.Point(0, 36);
            this.layoutDisplayName.Name = "layoutDisplayName";
            this.layoutDisplayName.Size = new System.Drawing.Size(700, 36);
            this.layoutDisplayName.Text = "顯示名稱";
            this.layoutDisplayName.TextSize = new System.Drawing.Size(76, 24);
            //
            // layoutParent
            //
            this.layoutParent.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.layoutParent.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.layoutParent.AppearanceItemCaption.Options.UseFont = true;
            this.layoutParent.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutParent.Control = this.cbbParent;
            this.layoutParent.Location = new System.Drawing.Point(0, 72);
            this.layoutParent.Name = "layoutParent";
            this.layoutParent.Size = new System.Drawing.Size(700, 36);
            this.layoutParent.Text = "所屬群組";
            this.layoutParent.TextSize = new System.Drawing.Size(76, 24);
            //
            // layoutNavigationTarget
            //
            this.layoutNavigationTarget.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.layoutNavigationTarget.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.layoutNavigationTarget.AppearanceItemCaption.Options.UseFont = true;
            this.layoutNavigationTarget.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutNavigationTarget.Control = this.txbNavigationTarget;
            this.layoutNavigationTarget.Location = new System.Drawing.Point(0, 108);
            this.layoutNavigationTarget.Name = "layoutNavigationTarget";
            this.layoutNavigationTarget.Size = new System.Drawing.Size(700, 36);
            this.layoutNavigationTarget.Text = "導覽目標";
            this.layoutNavigationTarget.TextSize = new System.Drawing.Size(76, 24);
            //
            // layoutSortOrder
            //
            this.layoutSortOrder.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.layoutSortOrder.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.layoutSortOrder.AppearanceItemCaption.Options.UseFont = true;
            this.layoutSortOrder.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutSortOrder.Control = this.speSortOrder;
            this.layoutSortOrder.Location = new System.Drawing.Point(0, 180);
            this.layoutSortOrder.Name = "layoutSortOrder";
            this.layoutSortOrder.Size = new System.Drawing.Size(700, 36);
            this.layoutSortOrder.Text = "排序";
            this.layoutSortOrder.TextSize = new System.Drawing.Size(76, 24);
            //
            // layoutDevelopmentStatus
            //
            this.layoutDevelopmentStatus.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.layoutDevelopmentStatus.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.layoutDevelopmentStatus.AppearanceItemCaption.Options.UseFont = true;
            this.layoutDevelopmentStatus.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutDevelopmentStatus.Control = this.cbbDevelopmentStatus;
            this.layoutDevelopmentStatus.Location = new System.Drawing.Point(0, 216);
            this.layoutDevelopmentStatus.Name = "layoutDevelopmentStatus";
            this.layoutDevelopmentStatus.Size = new System.Drawing.Size(700, 36);
            this.layoutDevelopmentStatus.Text = "開發狀態";
            this.layoutDevelopmentStatus.TextSize = new System.Drawing.Size(76, 24);
            //
            // layoutDescription
            //
            this.layoutDescription.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.layoutDescription.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.layoutDescription.AppearanceItemCaption.Options.UseFont = true;
            this.layoutDescription.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutDescription.Control = this.memoDescription;
            this.layoutDescription.Location = new System.Drawing.Point(0, 252);
            this.layoutDescription.MaxSize = new System.Drawing.Size(0, 116);
            this.layoutDescription.MinSize = new System.Drawing.Size(149, 116);
            this.layoutDescription.Name = "layoutDescription";
            this.layoutDescription.Size = new System.Drawing.Size(700, 116);
            this.layoutDescription.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutDescription.Text = "功能說明";
            this.layoutDescription.TextSize = new System.Drawing.Size(76, 24);
            //
            // layoutWide
            //
            this.layoutWide.Control = this.chkWide;
            this.layoutWide.Location = new System.Drawing.Point(0, 368);
            this.layoutWide.Name = "layoutWide";
            this.layoutWide.Size = new System.Drawing.Size(700, 36);
            this.layoutWide.TextSize = new System.Drawing.Size(0, 0);
            this.layoutWide.TextVisible = false;
            //
            // layoutVisible
            //
            this.layoutVisible.Control = this.chkVisible;
            this.layoutVisible.Location = new System.Drawing.Point(0, 404);
            this.layoutVisible.Name = "layoutVisible";
            this.layoutVisible.Size = new System.Drawing.Size(700, 36);
            this.layoutVisible.TextSize = new System.Drawing.Size(0, 0);
            this.layoutVisible.TextVisible = false;
            //
            // layoutActive
            //
            this.layoutActive.Control = this.chkActive;
            this.layoutActive.Location = new System.Drawing.Point(0, 440);
            this.layoutActive.Name = "layoutActive";
            this.layoutActive.Size = new System.Drawing.Size(700, 36);
            this.layoutActive.TextSize = new System.Drawing.Size(0, 0);
            this.layoutActive.TextVisible = false;
            //
            // emptySpace
            //
            this.emptySpace.AllowHotTrack = false;
            this.emptySpace.Location = new System.Drawing.Point(0, 476);
            this.emptySpace.Name = "emptySpace";
            this.emptySpace.Size = new System.Drawing.Size(700, 12);
            this.emptySpace.TextSize = new System.Drawing.Size(0, 0);
            //
            // FunctionEditForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(720, 522);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.IconOptions.Icon = global::Winform4System.Properties.Resources.AppIcon;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FunctionEditForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "編輯功能卡";
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cbbIcon.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iconImages)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbDisplayName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbParent.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txbNavigationTarget.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.speSortOrder.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbbDevelopmentStatus.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.memoDescription.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkWide.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkVisible.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkActive.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutDisplayName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutParent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutNavigationTarget)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutSortOrder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutDevelopmentStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutDescription)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutWide)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutVisible)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutActive)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpace)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
    }
}
