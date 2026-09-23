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
                throw new InvalidOperationException($"Không tìm thấy connection string '{name}' trong App.config.");

            return setting.ConnectionString;
        }
    }
}
