using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using Winform4System.Core.Security;
using Winform4System.DataAccess;
using Winform4System.DataAccess.Configuration;
using Winform4System.DataAccess.Entities;

namespace Winform4System.Business.Services
{
    public sealed class SystemHealthService
    {
        private readonly string _connectionString;

        public SystemHealthService(ConnectionStringProvider provider)
        {
            _connectionString = (provider ?? throw new ArgumentNullException(nameof(provider))).Get();
        }

        public IReadOnlyList<SystemHealthItem> Check(string applicationVersion)
        {
            CurrentAuthorization.Demand("SYSTEM.HEALTH.VIEW");
            var result = new List<SystemHealthItem>();
            DateTime checkedAt = DateTime.Now;
            result.Add(Item("版本", "應用程式版本", HealthState.Information,
                string.IsNullOrWhiteSpace(applicationVersion) ? "無法取得" : applicationVersion, checkedAt));
            try
            {
                using (var context = new Winform4SystemDbContext(_connectionString))
                {
                    bool databaseExists = context.Database.Exists();
                    result.Add(Item("連線", "資料庫連線", databaseExists ? HealthState.Healthy : HealthState.Error,
                        databaseExists ? context.Database.Connection.DataSource + " / " + context.Database.Connection.Database : "無法連線至主要資料庫。", checkedAt));
                    if (!databaseExists) return result;

                    List<SystemSetting> settings = context.SystemSettings.AsNoTracking().Where(x => !x.IsSensitive).ToList();
                    AddMetadata(result, settings, applicationVersion, checkedAt);
                    AddConfigurationValidation(result, settings, checkedAt);
                    AddPathChecks(result, settings.Where(x => x.ValueType == "PATH").ToList(), checkedAt);
                }
            }
            catch (Exception ex)
            {
                result.Add(Item("連線", "資料庫連線", HealthState.Error, ex.GetBaseException().Message, checkedAt));
            }
            return result;
        }

        private static void AddMetadata(ICollection<SystemHealthItem> result, IList<SystemSetting> settings, string applicationVersion, DateTime checkedAt)
        {
            AddRequiredSetting(result, settings, "SYSTEM.SCHEMA_VERSION", "版本", "資料庫結構版本", checkedAt);
            AddRequiredSetting(result, settings, "SYSTEM.ENVIRONMENT", "環境", "執行環境", checkedAt);
        }

        private static void AddRequiredSetting(ICollection<SystemHealthItem> result, IEnumerable<SystemSetting> settings, string key, string category, string name, DateTime checkedAt)
        {
            SystemSetting setting = settings.FirstOrDefault(x => x.SettingKey == key);
            bool valid = setting != null && !string.IsNullOrWhiteSpace(setting.SettingValue);
            result.Add(Item(category, name, valid ? HealthState.Information : HealthState.Error,
                valid ? setting.SettingValue : "缺少必要設定。", checkedAt));
        }

        private static void AddConfigurationValidation(ICollection<SystemHealthItem> result, IEnumerable<SystemSetting> settings, DateTime checkedAt)
        {
            var errors = new List<string>();
            foreach (SystemSetting setting in settings)
            {
                try { SystemSettingService.ValidateAndNormalize(setting, setting.SettingValue); }
                catch (Exception ex) { errors.Add(setting.DisplayName + "：" + ex.Message); }
            }
            result.Add(Item("設定", "重要設定檢查", errors.Count == 0 ? HealthState.Healthy : HealthState.Error,
                errors.Count == 0 ? "所有非敏感參數皆符合格式限制。" : string.Join("；", errors), checkedAt));
        }

        private static void AddPathChecks(ICollection<SystemHealthItem> result, IEnumerable<SystemSetting> pathSettings, DateTime checkedAt)
        {
            foreach (SystemSetting setting in pathSettings)
            {
                string path = Environment.ExpandEnvironmentVariables(setting.SettingValue ?? string.Empty);
                if (string.IsNullOrWhiteSpace(path))
                {
                    result.Add(Item("共用路徑", setting.DisplayName, HealthState.Warning, "尚未設定路徑。", checkedAt));
                    continue;
                }
                bool exists;
                try { exists = Directory.Exists(path); }
                catch (Exception ex)
                {
                    result.Add(Item("共用路徑", setting.DisplayName, HealthState.Error, ex.Message, checkedAt));
                    continue;
                }
                result.Add(Item("共用路徑", setting.DisplayName, exists ? HealthState.Healthy : HealthState.Error,
                    exists ? path : "無法存取：" + path, checkedAt));
                if (exists) AddStorageCheck(result, setting.DisplayName, path, checkedAt);
            }
        }

        private static void AddStorageCheck(ICollection<SystemHealthItem> result, string displayName, string path, DateTime checkedAt)
        {
            try
            {
                string root = Path.GetPathRoot(Path.GetFullPath(path));
                if (string.IsNullOrWhiteSpace(root)) return;
                var drive = new DriveInfo(root);
                if (!drive.IsReady) return;
                double freeGb = drive.AvailableFreeSpace / 1024d / 1024d / 1024d;
                double totalGb = drive.TotalSize / 1024d / 1024d / 1024d;
                HealthState state = totalGb > 0 && drive.AvailableFreeSpace * 100d / drive.TotalSize < 10d ? HealthState.Warning : HealthState.Healthy;
                result.Add(Item("儲存空間", displayName, state, string.Format("可用 {0:N1} GB／總計 {1:N1} GB", freeGb, totalGb), checkedAt));
            }
            catch (Exception ex)
            {
                result.Add(Item("儲存空間", displayName, HealthState.Warning, "無法取得容量資訊：" + ex.Message, checkedAt));
            }
        }

        private static SystemHealthItem Item(string category, string name, HealthState state, string detail, DateTime checkedAt)
        {
            return new SystemHealthItem { Category = category, CheckName = name, State = state, Detail = detail, CheckedAt = checkedAt };
        }
    }

    public enum HealthState { Healthy, Warning, Error, Information }

    public sealed class SystemHealthItem
    {
        public string Category { get; set; }
        public string CheckName { get; set; }
        public HealthState State { get; set; }
        public string Detail { get; set; }
        public DateTime CheckedAt { get; set; }
        public string StateDisplay => State == HealthState.Healthy ? "正常" : State == HealthState.Warning ? "警告" : State == HealthState.Error ? "錯誤" : "資訊";
    }
}
