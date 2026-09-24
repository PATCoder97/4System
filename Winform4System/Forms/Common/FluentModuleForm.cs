using DevExpress.Utils;
using DevExpress.Utils.Svg;
using DevExpress.XtraBars.FluentDesignSystem;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraTab;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Winform4System.Forms.Common
{
    public abstract class FluentModuleForm : FluentDesignForm
    {
        private readonly AccordionControl _navigation;
        private readonly XtraTabControl _tabs;
        private readonly Dictionary<string, XtraTabPage> _openPages =
            new Dictionary<string, XtraTabPage>(StringComparer.OrdinalIgnoreCase);

        protected FluentModuleForm(string title, string componentName)
        {
            if (string.IsNullOrWhiteSpace(componentName))
                throw new ArgumentException("Component name is required.", nameof(componentName));

            Text = title;
            IconOptions.Icon = Properties.Resources.AppIcon;
            StartPosition = FormStartPosition.CenterParent;
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(1100, 700);

            var titleBar = new FluentDesignFormControl
            {
                Dock = DockStyle.Top,
                FluentDesignForm = this,
                Name = componentName + "TitleBar"
            };
            _navigation = new AccordionControl
            {
                AllowItemSelection = true,
                Dock = DockStyle.Left,
                Name = componentName + "Navigation",
                ScrollBarMode = ScrollBarMode.Touch,
                ShowFilterControl = ShowFilterControl.Always,
                ViewType = AccordionControlViewType.HamburgerMenu,
                Width = 235
            };
            var container = new FluentDesignFormContainer
            {
                Dock = DockStyle.Fill,
                Name = componentName + "Container"
            };
            _tabs = new XtraTabControl
            {
                Dock = DockStyle.Fill,
                Name = componentName + "Tabs",
                ClosePageButtonShowMode = ClosePageButtonShowMode.InAllTabPageHeaders,
                ShowTabHeader = DefaultBoolean.True
            };
            _tabs.AppearancePage.Header.Font = new Font("Microsoft JhengHei UI", 12F);
            _tabs.AppearancePage.Header.Options.UseFont = true;
            _tabs.AppearancePage.HeaderActive.Font = new Font("Microsoft JhengHei UI", 12F);
            _tabs.AppearancePage.HeaderActive.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Critical;
            _tabs.AppearancePage.HeaderActive.Options.UseFont = true;
            _tabs.AppearancePage.HeaderActive.Options.UseForeColor = true;
            _tabs.CloseButtonClick += Tabs_CloseButtonClick;
            container.Controls.Add(_tabs);

            Controls.Add(container);
            Controls.Add(_navigation);
            Controls.Add(titleBar);
            ControlContainer = container;
            FluentDesignFormControl = titleBar;
            NavigationControl = _navigation;
        }

        protected AccordionControlElement AddNavigationGroup(string caption, string hint = null)
        {
            var group = new AccordionControlElement
            {
                Text = caption,
                Hint = string.IsNullOrWhiteSpace(hint) ? caption : hint,
                Style = ElementStyle.Group,
                Expanded = true
            };
            group.Appearance.Default.Font = new Font("Microsoft JhengHei UI", 14.25F);
            _navigation.Elements.Add(group);
            return group;
        }

        protected void AddNavigationItem(
            AccordionControlElement parent,
            string key,
            string caption,
            SvgImage icon,
            Func<Control> contentFactory)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            var item = new AccordionControlElement
            {
                Name = "nav_" + key,
                Text = caption,
                Hint = caption,
                Style = ElementStyle.Item,
                Tag = new NavigationItem(key, caption, contentFactory)
            };
            item.Appearance.Default.Font = new Font("Microsoft JhengHei UI", 14.25F);
            item.Appearance.Normal.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Hyperlink;
            item.Appearance.Hovered.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Critical;
            item.Appearance.Pressed.ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Critical;
            item.ImageOptions.SvgImage = icon;
            item.Click += NavigationItem_Click;
            parent.Elements.Add(item);
        }

        protected void OpenTab(string key, string caption, Func<Control> contentFactory)
        {
            if (_openPages.TryGetValue(key, out XtraTabPage existingPage))
            {
                _tabs.SelectedTabPage = existingPage;
                return;
            }

            using (SplashScreenManager.ShowOverlayForm(this))
            {
                Control content = contentFactory();
                content.Dock = DockStyle.Fill;
                var page = new XtraTabPage { Name = "tab_" + key, Text = caption, Tag = key };
                page.Controls.Add(content);
                _tabs.TabPages.Add(page);
                _openPages.Add(key, page);
                _tabs.SelectedTabPage = page;
            }
        }

        private void NavigationItem_Click(object sender, EventArgs e)
        {
            var element = sender as AccordionControlElement;
            var navigationItem = element?.Tag as NavigationItem;
            if (navigationItem != null)
                OpenTab(navigationItem.Key, navigationItem.Caption, navigationItem.ContentFactory);
        }

        private void Tabs_CloseButtonClick(object sender, EventArgs e)
        {
            var pageProperty = e?.GetType().GetProperty("Page");
            var page = pageProperty?.GetValue(e, null) as XtraTabPage ?? _tabs.SelectedTabPage;
            if (page == null)
                return;

            string key = page.Tag as string;
            if (!string.IsNullOrWhiteSpace(key))
                _openPages.Remove(key);
            _tabs.TabPages.Remove(page);
            page.Dispose();
        }

        private sealed class NavigationItem
        {
            public NavigationItem(string key, string caption, Func<Control> contentFactory)
            {
                Key = key;
                Caption = caption;
                ContentFactory = contentFactory ?? throw new ArgumentNullException(nameof(contentFactory));
            }

            public string Key { get; }
            public string Caption { get; }
            public Func<Control> ContentFactory { get; }
        }
    }
}
