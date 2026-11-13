using System.Data.Entity;
using Npgsql;

namespace Winform4System.DataAccess.Configuration
{
    public sealed class PostgresConfiguration : DbConfiguration
    {
        public PostgresConfiguration()
        {
            SetProviderFactory("Npgsql", NpgsqlFactory.Instance);
            SetProviderServices("Npgsql", NpgsqlServices.Instance);
            SetDefaultConnectionFactory(new NpgsqlConnectionFactory());
        }
    }
}
