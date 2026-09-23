using System;
using System.Configuration;

namespace Winform4System.DataAccess.Configuration
{
    public sealed class ConnectionStringProvider
    {
        public string Get(string name = "MainDatabase")
        {
            ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings[name];
            if (setting == null || string.IsNullOrWhiteSpace(setting.ConnectionString))
                throw new InvalidOperationException($"在 App.config 中找不到連線字串「{name}」。");

            return setting.ConnectionString;
        }
    }
}
