using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Winform4System.Core.Security;
using Winform4System.DataAccess;
using Winform4System.DataAccess.Configuration;
using Winform4System.DataAccess.Entities;

namespace Winform4System.Business.Services
{
    public sealed class SystemSettingService
    {
        private readonly string _connectionString;

        public SystemSettingService(ConnectionStringProvider provider)
        {
            _connectionString = (provider ?? throw new ArgumentNullException(nameof(provider))).Get();
        }

        public SystemSettingService(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException("Connection string is required.", nameof(connectionString));
            _connectionString = connectionString;
        }

        public IReadOnlyList<SystemSettingListItem> GetSettings()
        {
            CurrentAuthorization.Demand("SYSTEM.SETTING.VIEW");
            using (var context = new Winform4SystemDbContext(_connectionString))
            {
                return context.SystemSettings.AsNoTracking()
                    .Where(x => !x.IsSensitive)
                    .OrderBy(x => x.SettingKey)
                    .Select(x => new SystemSettingListItem
                    {
                        SettingKey = x.SettingKey,
                        SettingValue = x.SettingValue,
                        DisplayName = x.DisplayName,
                        Description = x.Description,
                        ValueType = x.ValueType,
                        MinimumValue = x.MinimumValue,
                        MaximumValue = x.MaximumValue,
                        UpdatedByUserId = x.UpdatedByUserId,
                        UpdatedAt = x.UpdatedAt,
                        RowVersion = x.RowVersion
                    }).ToList();
            }
        }

        public void Save(SystemSettingEditModel model)
        {
            CurrentAuthorization.Demand("SYSTEM.SETTING.ADMIN");
            if (model == null) throw new ArgumentNullException(nameof(model));
            using (var context = new Winform4SystemDbContext(_connectionString))
            {
                SystemSetting setting = context.SystemSettings.SingleOrDefault(x => x.SettingKey == model.SettingKey);
                if (setting == null || setting.IsSensitive) throw new InvalidOperationException("找不到可編輯的系統參數。");
                if (string.Equals(setting.SettingKey, "SYSTEM.SCHEMA_VERSION", StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("資料庫結構版本只能由部署指令碼更新。");
                if (!RowVersionMatches(setting.RowVersion, model.RowVersion))
                    throw new InvalidOperationException("系統參數已由其他使用者更新，請重新載入後再試。");

                string normalizedValue = ValidateAndNormalize(setting, model.SettingValue);
                string previousValue = setting.SettingValue;
                setting.SettingValue = normalizedValue;
                setting.UpdatedByUserId = CurrentAuthorization.UserId;
                setting.UpdatedAt = DateTime.UtcNow;
                context.AuditLogs.Add(new AuditLog
                {
                    UserId = CurrentAuthorization.UserId,
                    ActionCode = "SYSTEM.SETTING.UPDATED",
                    EntityName = "sys_SystemSetting",
                    EntityId = setting.SettingKey,
                    Description = "已更新系統參數「" + setting.DisplayName + "」。",
                    MachineName = Environment.MachineName,
                    DataJson = AuditDataJson.Change(
                        new Dictionary<string, string> { { "value", previousValue } },
                        new Dictionary<string, string> { { "value", normalizedValue } })
                });
                try { context.SaveChanges(); }
                catch (DbUpdateConcurrencyException)
                {
                    throw new InvalidOperationException("系統參數已由其他使用者更新，請重新載入後再試。");
                }
            }
        }

        internal static string ValidateAndNormalize(SystemSetting setting, string value)
        {
            string normalized = (value ?? string.Empty).Trim();
            if (normalized.Length > 1000) throw new InvalidOperationException("參數值不可超過 1000 個字元。");
            switch ((setting.ValueType ?? string.Empty).ToUpperInvariant())
            {
                case "INTEGER":
                    if (!int.TryParse(normalized, out int integerValue)) throw new InvalidOperationException("參數值必須是整數。");
                    if (setting.MinimumValue.HasValue && integerValue < setting.MinimumValue.Value)
                        throw new InvalidOperationException("參數值不可小於 " + setting.MinimumValue.Value + "。");
                    if (setting.MaximumValue.HasValue && integerValue > setting.MaximumValue.Value)
                        throw new InvalidOperationException("參數值不可大於 " + setting.MaximumValue.Value + "。");
                    return integerValue.ToString();
                case "BOOLEAN":
                    if (!bool.TryParse(normalized, out bool booleanValue)) throw new InvalidOperationException("參數值必須是布林值。");
                    return booleanValue ? "true" : "false";
                case "PATH":
                    return normalized;
                case "STRING":
                    return normalized;
                default:
                    throw new InvalidOperationException("不支援的參數類型：" + setting.ValueType);
            }
        }

        private static bool RowVersionMatches(byte[] current, byte[] original)
        {
            return current != null && original != null && current.SequenceEqual(original);
        }
    }

    public sealed class SystemSettingListItem
    {
        public string SettingKey { get; set; }
        public string SettingValue { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string ValueType { get; set; }
        public int? MinimumValue { get; set; }
        public int? MaximumValue { get; set; }
        public string UpdatedByUserId { get; set; }
        public DateTime UpdatedAt { get; set; }
        public byte[] RowVersion { get; set; }
        public string ValueTypeDisplay => ValueType == "INTEGER" ? "整數" : ValueType == "BOOLEAN" ? "是／否" : ValueType == "PATH" ? "路徑" : "文字";
        public string SettingValueDisplay => ValueType == "BOOLEAN" ? (string.Equals(SettingValue, "true", StringComparison.OrdinalIgnoreCase) ? "是" : "否") : SettingValue;
        public bool CanEdit => !string.Equals(SettingKey, "SYSTEM.SCHEMA_VERSION", StringComparison.OrdinalIgnoreCase);
    }

    public sealed class SystemSettingEditModel
    {
        public string SettingKey { get; set; }
        public string SettingValue { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
