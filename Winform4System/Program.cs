using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.UserSkins;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList.Localization;
using System;
using System.Drawing;
using System.Windows.Forms;
using Winform4System.Business.Security;
using Winform4System.Business.Services;
using Winform4System.Core.Models;
using Winform4System.Core.Security;
using Winform4System.DataAccess.Configuration;
using Winform4System.DataAccess.Repositories;
using Winform4System.Forms.Login;
using Winform4System.Forms.Main;
using Winform4System.Helpers;
using Winform4System.Logging;

namespace Winform4System
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            BonusSkins.Register();
            UserLookAndFeel.Default.SetSkinStyle("WXI");
            WindowsFormsSettings.DefaultFont = new Font("Microsoft JhengHei UI", 9F, FontStyle.Regular);
            AppearanceObject.DefaultMenuFont = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular);
            TreeListLocalizer.Active = new TraditionalChineseTreeListLocalizer();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            IAppLogger logger = new FileAppLogger();
            Application.ThreadException += (sender, args) => HandleUiException(logger, args.Exception);
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
                logger.Error("UnhandledException", "Unhandled application exception.", args.ExceptionObject as Exception);

            logger.Info("Program", "Application starting.");

            try
            {
                var authenticationService = new AuthenticationService(
                    new EfUserAccountRepository(new ConnectionStringProvider()),
                    new PasswordHasher());

                while (true)
                {
                    UserSession session;
                    using (var loginForm = new LoginForm(authenticationService, logger))
                    {
                        if (loginForm.ShowDialog() != DialogResult.OK)
                        {
                            logger.Info("Program", "Login canceled. Application closing.");
                            return;
                        }

                        session = loginForm.Session;
                    }

                    CurrentAuthorization.SetPermissions(session.PermissionCodes);

                    IMainMenuService menuService = new EfMainMenuService(
                        new EfMainMenuRepository(new ConnectionStringProvider()),
                        session.UserId);
                    using (var mainForm = new MainForm(menuService, logger, session))
                    {
                        Application.Run(mainForm);
                        if (!mainForm.LogoutRequested)
                            return;
                    }

                    logger.Info("Program", "Logout completed. Returning to login screen.");
                }
            }
            catch (Exception exception)
            {
                logger.Error("Program", "Application failed to start.", exception);
                XtraMessageBox.Show(
                    "無法啟動應用程式，詳細資訊已記錄於 Logs 資料夾。",
                    ApplicationMetadata.DisplayName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private static void HandleUiException(IAppLogger logger, Exception exception)
        {
            logger.Error("UI", "Unhandled UI exception.", exception);
            XtraMessageBox.Show(
                "系統發生錯誤，詳細資訊已記錄於 Logs 資料夾。",
                ApplicationMetadata.DisplayName,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
