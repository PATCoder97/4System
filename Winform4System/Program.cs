using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.UserSkins;
using DevExpress.XtraEditors;
using System;
using System.Drawing;
using System.Windows.Forms;
using Winform4System.Business.Security;
using Winform4System.Business.Services;
using Winform4System.DataAccess.Configuration;
using Winform4System.DataAccess.Repositories;
using Winform4System.Forms.Login;
using Winform4System.Forms.Main;
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

                using (var loginForm = new LoginForm(authenticationService, logger))
                {
                    if (loginForm.ShowDialog() != DialogResult.OK)
                    {
                        logger.Info("Program", "Login canceled. Application closing.");
                        return;
                    }

                    IMainMenuService menuService = new EfMainMenuService(
                        new EfMainMenuRepository(new ConnectionStringProvider()),
                        loginForm.Session.AccountId);
                    Application.Run(new MainForm(menuService, logger, loginForm.Session));
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
