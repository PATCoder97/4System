using System;
using System.Data.Entity;
using System.Linq;
using Winform4System.DataAccess;
using Winform4System.DataAccess.Configuration;

namespace Winform4System.Business.Services
{
    public sealed class SessionSecurityService
    {
        private readonly string _connectionString;

        public SessionSecurityService(ConnectionStringProvider provider)
        {
            _connectionString = (provider ?? throw new ArgumentNullException(nameof(provider))).Get();
        }

        public SessionSecurityService(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException("Connection string is required.", nameof(connectionString));
            _connectionString = connectionString;
        }

        public bool IsSessionValid(string userId, Guid securityStamp)
        {
            if (string.IsNullOrWhiteSpace(userId) || securityStamp == Guid.Empty) return false;
            using (var context = new Winform4SystemDbContext(_connectionString))
            {
                return context.UserAccounts.AsNoTracking().Any(x =>
                    x.UserId == userId
                    && x.IsActive
                    && x.SecurityStamp == securityStamp);
            }
        }
    }
}
