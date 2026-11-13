using System;
using System.Configuration;

namespace Winform4System.DataAccess.Configuration
{
    public sealed class ConnectionStringProvider
    {
        public string Get(string name = "MainDatabase")
        {
            if (name == "MainDatabase")
            {
                string environment = ConfigurationManager.AppSettings["AppEnvironment"] ?? "Test";
                if (environment != "Test" && environment != "Prod")
                    throw new InvalidOperationException("執行環境必須為 Test 或 Prod。");
                name = environment + "Database";
            }
            ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings[name];
            if (setting == null || string.IsNullOrWhiteSpace(setting.ConnectionString))
                throw new InvalidOperationException($"在 App.config 中找不到連線字串「{name}」。");

            return setting.ConnectionString;
        }
    }
}
