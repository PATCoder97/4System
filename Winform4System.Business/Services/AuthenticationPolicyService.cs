using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using Winform4System.Core.Security;
using Winform4System.DataAccess;
using Winform4System.DataAccess.Configuration;
using Winform4System.DataAccess.Entities;

namespace Winform4System.Business.Services
{
    public sealed class AuthenticationPolicyService
    {
        private const string MaximumFailedAttemptsKey = "AUTH.MAX_FAILED_ATTEMPTS";
        private const string LockoutMinutesKey = "AUTH.LOCKOUT_MINUTES";
        private readonly string _connectionString;

        public AuthenticationPolicyService(ConnectionStringProvider provider)
        {
            _connectionString = (provider ?? throw new ArgumentNullException(nameof(provider))).Get();
        }

        public AuthenticationPolicyService(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException("Connection string is required.", nameof(connectionString));
            _connectionString = connectionString;
        }

        public AuthenticationPolicy GetForAuthentication()
        {
            using (var context = new Winform4SystemDbContext(_connectionString))
            {
                return ReadPolicy(context, false);
            }
        }

        public AuthenticationPolicy GetForManagement()
        {
            CurrentAuthorization.Demand("SYSTEM.USER.ADMIN");
            using (var context = new Winform4SystemDbContext(_connectionString))
            {
                return ReadPolicy(context, true);
            }
        }

        public void Save(AuthenticationPolicy policy)
        {
            CurrentAuthorization.Demand("SYSTEM.USER.ADMIN");
            Validate(policy);
            using (var context = new Winform4SystemDbContext(_connectionString))
            using (var transaction = context.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                SystemSetting attempts = GetSetting(context, MaximumFailedAttemptsKey);
                SystemSetting minutes = GetSetting(context, LockoutMinutesKey);
                if (!RowVersionMatches(attempts.RowVersion, policy.MaximumFailedAttemptsRowVersion)
                    || !RowVersionMatches(minutes.RowVersion, policy.LockoutMinutesRowVersion))
                    throw new InvalidOperationException("帳號安全設定已由其他使用者更新，請重新開啟後再試。");

                var before = new Dictionary<string, string>
                {
                    { "maximumFailedAttempts", attempts.SettingValue },
                    { "lockoutMinutes", minutes.SettingValue }
                };
                attempts.SettingValue = policy.MaximumFailedAttempts.ToString();
                minutes.SettingValue = policy.LockoutMinutes.ToString();
                attempts.UpdatedByUserId = CurrentAuthorization.UserId;
                minutes.UpdatedByUserId = CurrentAuthorization.UserId;
                attempts.UpdatedAt = DateTime.UtcNow;
                minutes.UpdatedAt = DateTime.UtcNow;
                context.AuditLogs.Add(new AuditLog
                {
                    UserId = CurrentAuthorization.UserId,
                    ActionCode = "SYSTEM.SETTING.AUTH_POLICY.UPDATED",
                    EntityName = "sys_SystemSetting",
                    EntityId = "AUTH",
                    Description = "已更新帳號登入失敗與鎖定政策。",
                    MachineName = Environment.MachineName,
                    DataJson = AuditDataJson.Change(before, new Dictionary<string, string>
                    {
                        { "maximumFailedAttempts", attempts.SettingValue },
                        { "lockoutMinutes", minutes.SettingValue }
                    })
                });
                context.SaveChanges();
                transaction.Commit();
            }
        }

        private static AuthenticationPolicy ReadPolicy(Winform4SystemDbContext context, bool includeRowVersions)
        {
            SystemSetting attempts = GetSetting(context, MaximumFailedAttemptsKey, true);
            SystemSetting minutes = GetSetting(context, LockoutMinutesKey, true);
            var policy = new AuthenticationPolicy
            {
                MaximumFailedAttempts = ParseValue(attempts, 1, 20),
                LockoutMinutes = ParseValue(minutes, 1, 1440)
            };
            if (includeRowVersions)
            {
                policy.MaximumFailedAttemptsRowVersion = attempts.RowVersion;
                policy.LockoutMinutesRowVersion = minutes.RowVersion;
            }
            return policy;
        }

        private static SystemSetting GetSetting(Winform4SystemDbContext context, string key, bool noTracking = false)
        {
            IQueryable<SystemSetting> query = noTracking ? context.SystemSettings.AsNoTracking() : context.SystemSettings;
            SystemSetting setting = query.SingleOrDefault(x => x.SettingKey == key);
            if (setting == null) throw new InvalidOperationException("找不到必要的帳號安全設定：" + key);
            return setting;
        }

        private static int ParseValue(SystemSetting setting, int minimum, int maximum)
        {
            if (!int.TryParse(setting.SettingValue, out int value) || value < minimum || value > maximum)
                throw new InvalidOperationException("帳號安全設定值無效：" + setting.SettingKey);
            return value;
        }

        private static void Validate(AuthenticationPolicy policy)
        {
            if (policy == null) throw new ArgumentNullException(nameof(policy));
            if (policy.MaximumFailedAttempts < 1 || policy.MaximumFailedAttempts > 20)
                throw new InvalidOperationException("登入失敗次數上限必須介於 1 到 20。");
            if (policy.LockoutMinutes < 1 || policy.LockoutMinutes > 1440)
                throw new InvalidOperationException("帳號鎖定時間必須介於 1 到 1440 分鐘。");
        }

        private static bool RowVersionMatches(byte[] current, byte[] original)
        {
            return current != null && original != null && current.SequenceEqual(original);
        }
    }

    public sealed class AuthenticationPolicy
    {
        public int MaximumFailedAttempts { get; set; }
        public int LockoutMinutes { get; set; }
        public byte[] MaximumFailedAttemptsRowVersion { get; set; }
        public byte[] LockoutMinutesRowVersion { get; set; }
    }
}
