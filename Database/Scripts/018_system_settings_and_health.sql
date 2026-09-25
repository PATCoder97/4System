SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID(N'dbo.sys_SystemSetting', N'U') IS NULL
        THROW 50000, N'找不到 sys_SystemSetting，請先執行 014_authentication_policy_settings.sql。', 1;

    IF EXISTS
    (
        SELECT 1 FROM sys.check_constraints
        WHERE parent_object_id = OBJECT_ID(N'dbo.sys_SystemSetting')
          AND name = N'CK_sys_SystemSetting_ValueType'
    )
        ALTER TABLE dbo.sys_SystemSetting DROP CONSTRAINT CK_sys_SystemSetting_ValueType;

    ALTER TABLE dbo.sys_SystemSetting WITH CHECK ADD CONSTRAINT CK_sys_SystemSetting_ValueType
        CHECK (ValueType IN ('STRING', 'INTEGER', 'BOOLEAN', 'PATH'));

    MERGE dbo.sys_SystemSetting AS target
    USING (VALUES
        ('SYSTEM.UPLOAD_MAX_MB',      N'50',          N'上傳大小上限（MB）', N'單一檔案允許上傳的最大容量。',                   'INTEGER', 1, 1024),
        ('SYSTEM.BUSINESS_DATA_PATH', N'',            N'業務資料共用路徑',   N'存放業務附件及共用檔案的根目錄。',             'PATH',    NULL, NULL),
        ('SYSTEM.ALERTS_ENABLED',     N'true',        N'啟用系統警示',       N'控制系統是否顯示重要設定與運作狀態警示。',       'BOOLEAN', NULL, NULL),
        ('SYSTEM.ENVIRONMENT',        N'正式',        N'執行環境',           N'目前部署環境，例如正式、測試或開發環境。',             'STRING',  NULL, NULL),
        ('SYSTEM.SCHEMA_VERSION',     N'018',         N'資料庫結構版本',     N'目前已套用的資料庫結構版本。',                 'STRING',  NULL, NULL)
    ) AS source(SettingKey, DefaultValue, DisplayName, Description, ValueType, MinimumValue, MaximumValue)
       ON target.SettingKey = source.SettingKey
    WHEN MATCHED THEN UPDATE SET
        DisplayName = source.DisplayName,
        Description = source.Description,
        ValueType = source.ValueType,
        MinimumValue = source.MinimumValue,
        MaximumValue = source.MaximumValue,
        IsSensitive = 0
    WHEN NOT MATCHED THEN INSERT
        (SettingKey, SettingValue, DisplayName, Description, ValueType, MinimumValue, MaximumValue, IsSensitive)
        VALUES
        (source.SettingKey, source.DefaultValue, source.DisplayName, source.Description, source.ValueType, source.MinimumValue, source.MaximumValue, 0);

    DECLARE @SystemFunctionId int = (SELECT FunctionId FROM dbo.app_Function WHERE FunctionCode = 'SYSTEM');

    MERGE dbo.app_Function AS target
    USING (VALUES
        ('SYSTEM.SETTING', @SystemFunctionId, N'系統參數', N'維護非敏感的系統參數與業務路徑', 'SystemSettingManagementView', 70),
        ('SYSTEM.HEALTH',  @SystemFunctionId, N'系統狀態', N'檢查資料庫、共用路徑、儲存空間及重要設定', 'SystemHealthView', 80)
    ) AS source(FunctionCode, ParentFunctionId, DisplayName, Description, NavigationTarget, SortOrder)
       ON target.FunctionCode = source.FunctionCode
    WHEN MATCHED THEN UPDATE SET
        ParentFunctionId = source.ParentFunctionId,
        DisplayName = source.DisplayName,
        Description = source.Description,
        NavigationTarget = source.NavigationTarget,
        SortOrder = source.SortOrder,
        IsVisible = 0,
        IsActive = 1,
        DevelopmentStatus = 'COMPLETED',
        UpdatedAt = SYSUTCDATETIME()
    WHEN NOT MATCHED THEN INSERT
        (FunctionCode, ParentFunctionId, DisplayName, Description, NavigationTarget, SortOrder, IsVisible, IsActive, DevelopmentStatus)
        VALUES
        (source.FunctionCode, source.ParentFunctionId, source.DisplayName, source.Description, source.NavigationTarget, source.SortOrder, 0, 1, 'COMPLETED');

    MERGE dbo.auth_Permission AS target
    USING
    (
        SELECT functionRow.FunctionId, seed.ActionCode, seed.PermissionCode, seed.DisplayName
        FROM (VALUES
            ('SYSTEM.SETTING', 'VIEW',  'SYSTEM.SETTING.VIEW',  N'檢視系統參數'),
            ('SYSTEM.SETTING', 'ADMIN', 'SYSTEM.SETTING.ADMIN', N'管理系統參數'),
            ('SYSTEM.HEALTH',  'VIEW',  'SYSTEM.HEALTH.VIEW',   N'檢視系統狀態')
        ) seed(FunctionCode, ActionCode, PermissionCode, DisplayName)
        INNER JOIN dbo.app_Function functionRow ON functionRow.FunctionCode = seed.FunctionCode
    ) AS source ON target.PermissionCode = source.PermissionCode
    WHEN MATCHED THEN UPDATE SET
        FunctionId = source.FunctionId,
        ActionCode = source.ActionCode,
        DisplayName = source.DisplayName,
        IsActive = 1
    WHEN NOT MATCHED THEN INSERT (FunctionId, ActionCode, PermissionCode, DisplayName, IsActive)
        VALUES (source.FunctionId, source.ActionCode, source.PermissionCode, source.DisplayName, 1);

    MERGE dbo.auth_RolePermission AS target
    USING
    (
        SELECT role.RoleId, permission.PermissionId
        FROM dbo.auth_Role role
        CROSS JOIN dbo.auth_Permission permission
        WHERE role.RoleCode = 'SECURITY_ADMIN'
          AND permission.PermissionCode IN ('SYSTEM.SETTING.VIEW', 'SYSTEM.SETTING.ADMIN', 'SYSTEM.HEALTH.VIEW')
    ) source ON target.RoleId = source.RoleId AND target.PermissionId = source.PermissionId
    WHEN MATCHED THEN UPDATE SET IsActive = 1
    WHEN NOT MATCHED THEN INSERT (RoleId, PermissionId, AssignedAt, IsActive)
        VALUES (source.RoleId, source.PermissionId, SYSUTCDATETIME(), 1);

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
