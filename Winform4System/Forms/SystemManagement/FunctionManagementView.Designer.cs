namespace Winform4System.Forms.SystemManagement
{
    partial class FunctionManagementView
    {
        private System.ComponentModel.IContainer components = null;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutTree;
        private DevExpress.XtraBars.BarManager barManagerTP;
        private DevExpress.XtraBars.Bar barTools;
        private DevExpress.XtraBars.BarButtonItem btnReload;
        private DevExpress.XtraTreeList.TreeList treeFunctions;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colDisplayName;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colFunctionCode;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colDescription;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colNavigationTarget;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colSortOrder;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colDevelopmentStatus;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colIsWide;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colIsVisible;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colIsActive;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FunctionManagementView));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.treeFunctions = new DevExpress.XtraTreeList.TreeList();
            this.colDisplayName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colFunctionCode = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colDescription = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colNavigationTarget = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colSortOrder = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colDevelopmentStatus = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colIsWide = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colIsVisible = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colIsActive = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.barManagerTP = new DevExpress.XtraBars.BarManager(this.components);
            this.barTools = new DevExpress.XtraBars.Bar();
            this.btnReload = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutTree = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeFunctions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutTree)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.treeFunctions);
            this.layoutControl1.AllowCustomization = false;
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 49);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(1200, 671);
            this.layoutControl1.TabIndex = 0;
            // 
            // treeFunctions
            // 
            this.treeFunctions.Appearance.HeaderPanel.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.treeFunctions.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.Black;
            this.treeFunctions.Appearance.HeaderPanel.Options.UseFont = true;
            this.treeFunctions.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.treeFunctions.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.treeFunctions.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.treeFunctions.Appearance.Row.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.treeFunctions.Appearance.Row.ForeColor = System.Drawing.Color.Black;
            this.treeFunctions.Appearance.Row.Options.UseFont = true;
            this.treeFunctions.Appearance.Row.Options.UseForeColor = true;
            this.treeFunctions.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.colDisplayName,
            this.colFunctionCode,
            this.colDescription,
            this.colNavigationTarget,
            this.colSortOrder,
            this.colDevelopmentStatus,
            this.colIsWide,
            this.colIsVisible,
            this.colIsActive});
            this.treeFunctions.KeyFieldName = "FunctionId";
            this.treeFunctions.Location = new System.Drawing.Point(12, 12);
            this.treeFunctions.Name = "treeFunctions";
            this.treeFunctions.OptionsBehavior.Editable = false;
            this.treeFunctions.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.treeFunctions.OptionsSelection.EnableAppearanceHotTrackedRow = DevExpress.Utils.DefaultBoolean.True;
            this.treeFunctions.OptionsView.AutoWidth = false;
            this.treeFunctions.OptionsView.EnableAppearanceOddRow = true;
            this.treeFunctions.OptionsView.ShowAutoFilterRow = true;
            this.treeFunctions.ParentFieldName = "ParentFunctionId";
            this.treeFunctions.Size = new System.Drawing.Size(1176, 647);
            this.treeFunctions.TabIndex = 1;
            this.treeFunctions.PopupMenuShowing += new DevExpress.XtraTreeList.PopupMenuShowingEventHandler(this.treeFunctions_PopupMenuShowing);
            // 
            // colDisplayName
            // 
            this.colDisplayName.Caption = "功能階層";
            this.colDisplayName.FieldName = "DisplayName";
            this.colDisplayName.Name = "colDisplayName";
            this.colDisplayName.Visible = true;
            this.colDisplayName.VisibleIndex = 0;
            this.colDisplayName.Width = 210;
            // 
            // colFunctionCode
            // 
            this.colFunctionCode.Caption = "功能代碼";
            this.colFunctionCode.FieldName = "FunctionCode";
            this.colFunctionCode.Name = "colFunctionCode";
            this.colFunctionCode.Visible = true;
            this.colFunctionCode.VisibleIndex = 1;
            this.colFunctionCode.Width = 200;
            // 
            // colDescription
            // 
            this.colDescription.Caption = "功能說明";
            this.colDescription.FieldName = "Description";
            this.colDescription.Name = "colDescription";
            this.colDescription.Visible = true;
            this.colDescription.VisibleIndex = 2;
            this.colDescription.Width = 260;
            // 
            // colNavigationTarget
            // 
            this.colNavigationTarget.Caption = "導覽目標";
            this.colNavigationTarget.FieldName = "NavigationTarget";
            this.colNavigationTarget.Name = "colNavigationTarget";
            this.colNavigationTarget.Visible = true;
            this.colNavigationTarget.VisibleIndex = 3;
            this.colNavigationTarget.Width = 190;
            // 
            // colSortOrder
            // 
            this.colSortOrder.Caption = "排序";
            this.colSortOrder.FieldName = "SortOrder";
            this.colSortOrder.Name = "colSortOrder";
            this.colSortOrder.Visible = true;
            this.colSortOrder.VisibleIndex = 4;
            this.colSortOrder.Width = 65;
            // 
            // colDevelopmentStatus
            // 
            this.colDevelopmentStatus.Caption = "開發狀態";
            this.colDevelopmentStatus.FieldName = "DevelopmentStatus";
            this.colDevelopmentStatus.Name = "colDevelopmentStatus";
            this.colDevelopmentStatus.Visible = true;
            this.colDevelopmentStatus.VisibleIndex = 5;
            this.colDevelopmentStatus.Width = 100;
            // 
            // colIsWide
            // 
            this.colIsWide.Caption = "寬卡";
            this.colIsWide.FieldName = "IsWide";
            this.colIsWide.Name = "colIsWide";
            this.colIsWide.Visible = true;
            this.colIsWide.VisibleIndex = 6;
            this.colIsWide.Width = 65;
            // 
            // colIsVisible
            // 
            this.colIsVisible.Caption = "顯示";
            this.colIsVisible.FieldName = "IsVisible";
            this.colIsVisible.Name = "colIsVisible";
            this.colIsVisible.Visible = true;
            this.colIsVisible.VisibleIndex = 7;
            this.colIsVisible.Width = 65;
            // 
            // colIsActive
            // 
            this.colIsActive.Caption = "啟用";
            this.colIsActive.FieldName = "IsActive";
            this.colIsActive.Name = "colIsActive";
            this.colIsActive.Visible = true;
            this.colIsActive.VisibleIndex = 8;
            this.colIsActive.Width = 65;
            // 
            // barManagerTP
            // 
            this.barManagerTP.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.barTools});
            this.barManagerTP.DockControls.Add(this.barDockControlTop);
            this.barManagerTP.DockControls.Add(this.barDockControlBottom);
            this.barManagerTP.DockControls.Add(this.barDockControlLeft);
            this.barManagerTP.DockControls.Add(this.barDockControlRight);
            this.barManagerTP.Form = this;
            this.barManagerTP.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.btnReload});
            this.barManagerTP.MainMenu = this.barTools;
            this.barManagerTP.MaxItemId = 1;
            // 
            // barTools
            // 
            this.barTools.BarAppearance.Disabled.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.barTools.BarAppearance.Disabled.Options.UseFont = true;
            this.barTools.BarAppearance.Hovered.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.barTools.BarAppearance.Hovered.ForeColor = System.Drawing.Color.Black;
            this.barTools.BarAppearance.Hovered.Options.UseFont = true;
            this.barTools.BarAppearance.Hovered.Options.UseForeColor = true;
            this.barTools.BarAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.barTools.BarAppearance.Normal.ForeColor = System.Drawing.Color.Black;
            this.barTools.BarAppearance.Normal.Options.UseFont = true;
            this.barTools.BarAppearance.Normal.Options.UseForeColor = true;
            this.barTools.BarAppearance.Pressed.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.barTools.BarAppearance.Pressed.ForeColor = System.Drawing.Color.Black;
            this.barTools.BarAppearance.Pressed.Options.UseFont = true;
            this.barTools.BarAppearance.Pressed.Options.UseForeColor = true;
            this.barTools.BarName = "Main menu";
            this.barTools.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Top;
            this.barTools.DockCol = 0;
            this.barTools.DockRow = 0;
            this.barTools.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.barTools.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.btnReload, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph)});
            this.barTools.OptionsBar.AllowQuickCustomization = false;
            this.barTools.OptionsBar.DrawDragBorder = false;
            this.barTools.OptionsBar.MultiLine = true;
            this.barTools.OptionsBar.UseWholeRow = true;
            this.barTools.Text = "主選單";
            // 
            // btnReload
            // 
            this.btnReload.Caption = "重新整理";
            this.btnReload.Id = 0;
            this.btnReload.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnReload.ImageOptions.SvgImage")));
            this.btnReload.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32);
            this.btnReload.ItemAppearance.Hovered.ForeColor = System.Drawing.Color.Blue;
            this.btnReload.ItemAppearance.Hovered.Options.UseForeColor = true;
            this.btnReload.ItemAppearance.Normal.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F);
            this.btnReload.ItemAppearance.Normal.Options.UseFont = true;
            this.btnReload.Name = "btnReload";
            this.btnReload.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnReload_ItemClick);
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManagerTP;
            this.barDockControlTop.Size = new System.Drawing.Size(1200, 49);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 720);
            this.barDockControlBottom.Manager = this.barManagerTP;
            this.barDockControlBottom.Size = new System.Drawing.Size(1200, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 49);
            this.barDockControlLeft.Manager = this.barManagerTP;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 671);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1200, 49);
            this.barDockControlRight.Manager = this.barManagerTP;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 671);
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutTree});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(1200, 671);
            this.Root.TextVisible = false;
            // 
            // layoutTree
            // 
            this.layoutTree.Control = this.treeFunctions;
            this.layoutTree.Location = new System.Drawing.Point(0, 0);
            this.layoutTree.Name = "layoutTree";
            this.layoutTree.Size = new System.Drawing.Size(1180, 651);
            this.layoutTree.TextSize = new System.Drawing.Size(0, 0);
            this.layoutTree.TextVisible = false;
            // 
            // FunctionManagementView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "FunctionManagementView";
            this.Size = new System.Drawing.Size(1200, 720);
            this.Load += new System.EventHandler(this.FunctionManagementView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.treeFunctions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManagerTP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutTree)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
    }
}
