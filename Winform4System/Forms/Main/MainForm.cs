using DevExpress.Utils;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Winform4System.Business.Services;
using Winform4System.Core.Models;
using Winform4System.Logging;

namespace Winform4System.Forms.Main
{
    public partial class MainForm : XtraForm
    {
        private readonly IMainMenuService _menuService;
        private readonly IAppLogger _logger;
        private readonly UserSession _session;
        private readonly Dictionary<TileItem, MenuItemDefinition> _menuByTile = new Dictionary<TileItem, MenuItemDefinition>();

        public MainForm()
            : this(new DemoMainMenuService(), new FileAppLogger(), UserSession.CreateDemo())
        {
        }

        public MainForm(IMainMenuService menuService, IAppLogger logger, UserSession session)
        {
            _menuService = menuService ?? throw new ArgumentNullException(nameof(menuService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _session = session ?? throw new ArgumentNullException(nameof(session));

            InitializeComponent();
            InitializeDashboard();
        }

        private void InitializeDashboard()
        {
            lblWelcome.Text = $"Xin chào, {_session.DisplayName}";
            lblSession.Text = $"{_session.UserId}  •  {_session.Department}  •  {_session.Role}";

            IReadOnlyList<MenuItemDefinition> menuItems = _menuService.GetMenuItems();
            foreach (IGrouping<string, MenuItemDefinition> groupData in menuItems.GroupBy(item => item.Group))
            {
                var group = new TileGroup { Text = groupData.Key };
                foreach (MenuItemDefinition definition in groupData)
                    group.Items.Add(CreateMenuTile(definition));

                tileMain.Groups.Add(group);
            }

            lblStatus.Text = $"Sẵn sàng  •  {menuItems.Count} chức năng  •  {DateTime.Now:dd/MM/yyyy HH:mm}";
            _logger.Info(nameof(MainForm), "Main dashboard initialized.");
        }

        private TileItem CreateMenuTile(MenuItemDefinition definition)
        {
            var titleElement = new TileItemElement
            {
                Text = definition.Title,
                TextAlignment = TileItemContentAlignment.TopLeft
            };
            titleElement.Appearance.Normal.Font = new Font("Segoe UI Semibold", 15F, FontStyle.Bold);
            titleElement.Appearance.Normal.Options.UseFont = true;

            var descriptionElement = new TileItemElement
            {
                Text = definition.Description,
                TextAlignment = TileItemContentAlignment.BottomLeft,
                TextLocation = new Point(0, -2)
            };
            descriptionElement.Appearance.Normal.Font = new Font("Segoe UI", 9F);
            descriptionElement.Appearance.Normal.ForeColor = Color.FromArgb(235, 245, 250);
            descriptionElement.Appearance.Normal.Options.UseFont = true;
            descriptionElement.Appearance.Normal.Options.UseForeColor = true;

            var tile = new TileItem
            {
                Name = "tile" + definition.Code,
                ItemSize = definition.IsWide ? TileItemSize.Wide : TileItemSize.Medium
            };
            tile.AppearanceItem.Normal.BackColor = definition.AccentColor;
            tile.AppearanceItem.Normal.BorderColor = Color.FromArgb(45, 255, 255, 255);
            tile.AppearanceItem.Normal.Options.UseBackColor = true;
            tile.AppearanceItem.Normal.Options.UseBorderColor = true;
            tile.Elements.Add(titleElement);
            tile.Elements.Add(descriptionElement);
            tile.ItemClick += MenuTile_ItemClick;

            _menuByTile[tile] = definition;
            return tile;
        }

        private void MenuTile_ItemClick(object sender, TileItemEventArgs e)
        {
            if (!_menuByTile.TryGetValue(e.Item, out MenuItemDefinition definition))
                return;

            _logger.Info(nameof(MainForm), $"Menu selected: {definition.Code} - {definition.Title}");
            XtraMessageBox.Show(
                $"Chức năng “{definition.Title}” đã được chuẩn bị vị trí.\n\nPhần nghiệp vụ sẽ được kết nối ở giai đoạn tiếp theo.",
                "Winform4System",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void btnUser_Click(object sender, EventArgs e)
        {
            XtraMessageBox.Show(
                $"{_session.DisplayName}\nMã người dùng: {_session.UserId}\nĐơn vị: {_session.Department}\nTrạng thái: {_session.Role}",
                "Thông tin người dùng",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void btnExit_Click(object sender, EventArgs e) => Close();

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _logger.Info(nameof(MainForm), "Application closing.");
        }
    }
}
