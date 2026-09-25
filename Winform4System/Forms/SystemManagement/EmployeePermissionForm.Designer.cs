namespace Winform4System.Forms.SystemManagement
{
    partial class EmployeePermissionForm
    {
        private System.ComponentModel.IContainer components;
        private DevExpress.XtraBars.BarManager barManager;
        private DevExpress.XtraBars.Bar barTools;
        private DevExpress.XtraBars.BarButtonItem btnSave, btnCancel;
        private DevExpress.XtraBars.BarDockControl barDockControlTop, barDockControlBottom, barDockControlLeft, barDockControlRight;
        private DevExpress.XtraLayout.LayoutControl layoutControl;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutTabs;
        private DevExpress.XtraTab.XtraTabControl tabControl;
        private DevExpress.XtraTab.XtraTabPage tabGroups, tabRoles, tabPermissions;
        private DevExpress.XtraLayout.LayoutControl groupsLayout, rolesLayout, permissionsLayout;
        private DevExpress.XtraLayout.LayoutControlGroup groupsRoot, rolesRoot, permissionsRoot;
        private DevExpress.XtraLayout.LayoutControlItem layoutGroups, layoutRoles, layoutPermissions;
        private DevExpress.XtraEditors.CheckedListBoxControl checkedGroups;
        private DevExpress.XtraEditors.MemoEdit memoRoles;
        private DevExpress.XtraGrid.GridControl gridPermissions;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewPermissions;
        private DevExpress.XtraGrid.Columns.GridColumn colPermissionCode, colPermissionName;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            barManager = new DevExpress.XtraBars.BarManager(components); barTools = new DevExpress.XtraBars.Bar(); btnSave = new DevExpress.XtraBars.BarButtonItem(); btnCancel = new DevExpress.XtraBars.BarButtonItem(); barDockControlTop = new DevExpress.XtraBars.BarDockControl(); barDockControlBottom = new DevExpress.XtraBars.BarDockControl(); barDockControlLeft = new DevExpress.XtraBars.BarDockControl(); barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            layoutControl = new DevExpress.XtraLayout.LayoutControl(); Root = new DevExpress.XtraLayout.LayoutControlGroup(); layoutTabs = new DevExpress.XtraLayout.LayoutControlItem(); tabControl = new DevExpress.XtraTab.XtraTabControl(); tabGroups = new DevExpress.XtraTab.XtraTabPage(); tabRoles = new DevExpress.XtraTab.XtraTabPage(); tabPermissions = new DevExpress.XtraTab.XtraTabPage();
            groupsLayout = new DevExpress.XtraLayout.LayoutControl(); rolesLayout = new DevExpress.XtraLayout.LayoutControl(); permissionsLayout = new DevExpress.XtraLayout.LayoutControl(); groupsRoot = new DevExpress.XtraLayout.LayoutControlGroup(); rolesRoot = new DevExpress.XtraLayout.LayoutControlGroup(); permissionsRoot = new DevExpress.XtraLayout.LayoutControlGroup(); layoutGroups = new DevExpress.XtraLayout.LayoutControlItem(); layoutRoles = new DevExpress.XtraLayout.LayoutControlItem(); layoutPermissions = new DevExpress.XtraLayout.LayoutControlItem(); checkedGroups = new DevExpress.XtraEditors.CheckedListBoxControl(); memoRoles = new DevExpress.XtraEditors.MemoEdit(); gridPermissions = new DevExpress.XtraGrid.GridControl(); gridViewPermissions = new DevExpress.XtraGrid.Views.Grid.GridView(); colPermissionCode = new DevExpress.XtraGrid.Columns.GridColumn(); colPermissionName = new DevExpress.XtraGrid.Columns.GridColumn();
            BeginInitialize();

            barManager.Bars.AddRange(new[] { barTools }); barManager.DockControls.Add(barDockControlTop); barManager.DockControls.Add(barDockControlBottom); barManager.DockControls.Add(barDockControlLeft); barManager.DockControls.Add(barDockControlRight); barManager.Form = this; barManager.Items.AddRange(new DevExpress.XtraBars.BarItem[] { btnSave, btnCancel }); barManager.MainMenu = barTools; barManager.MaxItemId = 2;
            barTools.BarName = "人員權限工具列"; barTools.DockStyle = DevExpress.XtraBars.BarDockStyle.Top; barTools.LinksPersistInfo.AddRange(new[] { new DevExpress.XtraBars.LinkPersistInfo(btnSave), new DevExpress.XtraBars.LinkPersistInfo(btnCancel) }); barTools.OptionsBar.AllowQuickCustomization = false; barTools.OptionsBar.DrawDragBorder = false; barTools.OptionsBar.UseWholeRow = true; ConfigureBar(barTools);
            btnSave.Caption = "儲存群組設定"; btnSave.Id = 0; btnSave.ImageOptions.SvgImage = Winform4System.Helpers.SvgIconCatalog.Confirm; btnSave.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32); btnSave.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph; btnSave.ItemClick += btnSave_ItemClick;
            btnCancel.Caption = "取消"; btnCancel.Id = 1; btnCancel.ImageOptions.SvgImage = Winform4System.Helpers.SvgIconCatalog.Cancel; btnCancel.ImageOptions.SvgImageSize = new System.Drawing.Size(32, 32); btnCancel.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph; btnCancel.ItemClick += btnCancel_ItemClick;
            barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top; barDockControlTop.Manager = barManager; barDockControlTop.Size = new System.Drawing.Size(900, 49); barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom; barDockControlBottom.Manager = barManager; barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left; barDockControlLeft.Manager = barManager; barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right; barDockControlRight.Manager = barManager;

            layoutControl.AllowCustomization = false; layoutControl.Controls.Add(tabControl); layoutControl.Dock = System.Windows.Forms.DockStyle.Fill; layoutControl.Location = new System.Drawing.Point(0, 49); layoutControl.Root = Root; layoutControl.Size = new System.Drawing.Size(900, 551); Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True; Root.GroupBordersVisible = false; Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { layoutTabs }); Root.Size = new System.Drawing.Size(900, 551); layoutTabs.Control = tabControl; layoutTabs.Location = new System.Drawing.Point(0, 0); layoutTabs.Size = new System.Drawing.Size(880, 531); layoutTabs.TextVisible = false;

            tabControl.AppearancePage.Header.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F); tabControl.AppearancePage.Header.ForeColor = System.Drawing.Color.Black; tabControl.AppearancePage.Header.Options.UseFont = true; tabControl.AppearancePage.Header.Options.UseForeColor = true; tabControl.AppearancePage.HeaderActive.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F, System.Drawing.FontStyle.Bold); tabControl.AppearancePage.HeaderActive.Options.UseFont = true; tabControl.Location = new System.Drawing.Point(12, 12); tabControl.SelectedTabPage = tabGroups; tabControl.Size = new System.Drawing.Size(876, 527); tabControl.TabPages.AddRange(new[] { tabGroups, tabRoles, tabPermissions });
            ConfigureTab(tabGroups, groupsLayout, "安全性群組", 874, 486); ConfigureTab(tabRoles, rolesLayout, "有效角色", 874, 486); ConfigureTab(tabPermissions, permissionsLayout, "有效權限", 874, 486);

            ConfigureLayout(groupsLayout, groupsRoot, layoutGroups, checkedGroups, 874, 486); checkedGroups.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F); checkedGroups.Appearance.ForeColor = System.Drawing.Color.Black; checkedGroups.Appearance.Options.UseFont = true; checkedGroups.Appearance.Options.UseForeColor = true;
            ConfigureLayout(rolesLayout, rolesRoot, layoutRoles, memoRoles, 874, 486); memoRoles.Properties.ReadOnly = true; memoRoles.Properties.Appearance.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F); memoRoles.Properties.Appearance.ForeColor = System.Drawing.Color.Black; memoRoles.Properties.Appearance.Options.UseFont = true; memoRoles.Properties.Appearance.Options.UseForeColor = true;
            ConfigureLayout(permissionsLayout, permissionsRoot, layoutPermissions, gridPermissions, 874, 486);

            gridPermissions.MainView = gridViewPermissions; gridPermissions.ViewCollection.Add(gridViewPermissions); gridViewPermissions.GridControl = gridPermissions; gridViewPermissions.OptionsBehavior.Editable = false; gridViewPermissions.OptionsSelection.EnableAppearanceFocusedCell = false; gridViewPermissions.OptionsSelection.EnableAppearanceHotTrackedRow = DevExpress.Utils.DefaultBoolean.True; gridViewPermissions.OptionsView.ColumnAutoWidth = false; gridViewPermissions.OptionsView.EnableAppearanceOddRow = true; gridViewPermissions.OptionsView.ShowGroupPanel = false; gridViewPermissions.OptionsView.ShowAutoFilterRow = true; gridViewPermissions.Appearance.HeaderPanel.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F); gridViewPermissions.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.Black; gridViewPermissions.Appearance.HeaderPanel.Options.UseFont = true; gridViewPermissions.Appearance.HeaderPanel.Options.UseForeColor = true; gridViewPermissions.Appearance.HeaderPanel.Options.UseTextOptions = true; gridViewPermissions.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center; gridViewPermissions.Appearance.Row.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F); gridViewPermissions.Appearance.Row.ForeColor = System.Drawing.Color.Black; gridViewPermissions.Appearance.Row.Options.UseFont = true; gridViewPermissions.Appearance.Row.Options.UseForeColor = true; gridViewPermissions.Columns.AddRange(new[] { colPermissionCode, colPermissionName });
            colPermissionCode.FieldName = "PermissionCode"; colPermissionCode.Caption = "權限代碼"; colPermissionCode.Width = 330; colPermissionCode.Visible = true; colPermissionCode.VisibleIndex = 0; colPermissionName.FieldName = "DisplayName"; colPermissionName.Caption = "權限名稱"; colPermissionName.Width = 360; colPermissionName.Visible = true; colPermissionName.VisibleIndex = 1;

            Controls.Add(layoutControl); Controls.Add(barDockControlLeft); Controls.Add(barDockControlRight); Controls.Add(barDockControlBottom); Controls.Add(barDockControlTop); AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F); AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font; ClientSize = new System.Drawing.Size(900, 600); Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F); MinimumSize = new System.Drawing.Size(800, 550); StartPosition = System.Windows.Forms.FormStartPosition.CenterParent; Text = "人員權限"; IconOptions.Icon = Winform4System.Properties.Resources.AppIcon;
            EndInitialize();
        }

        private void BeginInitialize()
        {
            ((System.ComponentModel.ISupportInitialize)barManager).BeginInit(); ((System.ComponentModel.ISupportInitialize)layoutControl).BeginInit(); layoutControl.SuspendLayout(); ((System.ComponentModel.ISupportInitialize)Root).BeginInit(); ((System.ComponentModel.ISupportInitialize)layoutTabs).BeginInit(); ((System.ComponentModel.ISupportInitialize)tabControl).BeginInit(); tabControl.SuspendLayout(); tabGroups.SuspendLayout(); tabRoles.SuspendLayout(); tabPermissions.SuspendLayout();
            foreach (DevExpress.XtraLayout.LayoutControl layout in new[] { groupsLayout, rolesLayout, permissionsLayout }) { ((System.ComponentModel.ISupportInitialize)layout).BeginInit(); layout.SuspendLayout(); }
            foreach (DevExpress.XtraLayout.LayoutControlGroup group in new[] { groupsRoot, rolesRoot, permissionsRoot }) ((System.ComponentModel.ISupportInitialize)group).BeginInit();
            foreach (DevExpress.XtraLayout.LayoutControlItem item in new[] { layoutGroups, layoutRoles, layoutPermissions }) ((System.ComponentModel.ISupportInitialize)item).BeginInit();
            ((System.ComponentModel.ISupportInitialize)checkedGroups).BeginInit(); ((System.ComponentModel.ISupportInitialize)memoRoles.Properties).BeginInit(); ((System.ComponentModel.ISupportInitialize)gridPermissions).BeginInit(); ((System.ComponentModel.ISupportInitialize)gridViewPermissions).BeginInit(); SuspendLayout();
        }

        private void EndInitialize()
        {
            ((System.ComponentModel.ISupportInitialize)barManager).EndInit(); ((System.ComponentModel.ISupportInitialize)layoutControl).EndInit(); layoutControl.ResumeLayout(false); ((System.ComponentModel.ISupportInitialize)Root).EndInit(); ((System.ComponentModel.ISupportInitialize)layoutTabs).EndInit(); ((System.ComponentModel.ISupportInitialize)tabControl).EndInit(); tabControl.ResumeLayout(false); tabGroups.ResumeLayout(false); tabRoles.ResumeLayout(false); tabPermissions.ResumeLayout(false);
            foreach (DevExpress.XtraLayout.LayoutControl layout in new[] { groupsLayout, rolesLayout, permissionsLayout }) { ((System.ComponentModel.ISupportInitialize)layout).EndInit(); layout.ResumeLayout(false); }
            foreach (DevExpress.XtraLayout.LayoutControlGroup group in new[] { groupsRoot, rolesRoot, permissionsRoot }) ((System.ComponentModel.ISupportInitialize)group).EndInit();
            foreach (DevExpress.XtraLayout.LayoutControlItem item in new[] { layoutGroups, layoutRoles, layoutPermissions }) ((System.ComponentModel.ISupportInitialize)item).EndInit();
            ((System.ComponentModel.ISupportInitialize)checkedGroups).EndInit(); ((System.ComponentModel.ISupportInitialize)memoRoles.Properties).EndInit(); ((System.ComponentModel.ISupportInitialize)gridPermissions).EndInit(); ((System.ComponentModel.ISupportInitialize)gridViewPermissions).EndInit(); ResumeLayout(false); PerformLayout();
        }

        private static void ConfigureTab(DevExpress.XtraTab.XtraTabPage page, System.Windows.Forms.Control content, string caption, int width, int height) { page.Controls.Add(content); page.Text = caption; page.Size = new System.Drawing.Size(width, height); }
        private static void ConfigureLayout(DevExpress.XtraLayout.LayoutControl layout, DevExpress.XtraLayout.LayoutControlGroup root, DevExpress.XtraLayout.LayoutControlItem item, System.Windows.Forms.Control control, int width, int height) { layout.AllowCustomization = false; layout.Controls.Add(control); layout.Dock = System.Windows.Forms.DockStyle.Fill; layout.Root = root; layout.Size = new System.Drawing.Size(width, height); root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True; root.GroupBordersVisible = false; root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { item }); root.Size = new System.Drawing.Size(width, height); item.Control = control; item.Location = new System.Drawing.Point(0, 0); item.Size = new System.Drawing.Size(width - 20, height - 20); item.TextVisible = false; }
        private static void ConfigureBar(DevExpress.XtraBars.Bar bar) { bar.BarAppearance.Disabled.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F); bar.BarAppearance.Disabled.Options.UseFont = true; bar.BarAppearance.Hovered.Font = bar.BarAppearance.Normal.Font = bar.BarAppearance.Pressed.Font = new System.Drawing.Font("Microsoft JhengHei UI", 14.25F); bar.BarAppearance.Hovered.ForeColor = bar.BarAppearance.Normal.ForeColor = bar.BarAppearance.Pressed.ForeColor = System.Drawing.Color.Black; bar.BarAppearance.Hovered.Options.UseFont = bar.BarAppearance.Normal.Options.UseFont = bar.BarAppearance.Pressed.Options.UseFont = true; bar.BarAppearance.Hovered.Options.UseForeColor = bar.BarAppearance.Normal.Options.UseForeColor = bar.BarAppearance.Pressed.Options.UseForeColor = true; }
    }
}
