SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    IF COL_LENGTH('dbo.app_Function', 'Description') IS NULL
        ALTER TABLE dbo.app_Function ADD Description nvarchar(500) NULL;

    IF COL_LENGTH('dbo.app_Function', 'DevelopmentStatus') IS NULL
        ALTER TABLE dbo.app_Function ADD DevelopmentStatus varchar(20) NOT NULL
            CONSTRAINT DF_app_Function_DevelopmentStatus DEFAULT ('NOT_STARTED');

    IF COL_LENGTH('dbo.app_Function', 'IsWide') IS NULL
        ALTER TABLE dbo.app_Function ADD IsWide bit NOT NULL
            CONSTRAINT DF_app_Function_IsWide DEFAULT (1);

    IF NOT EXISTS
    (
        SELECT 1
        FROM sys.check_constraints
        WHERE parent_object_id = OBJECT_ID(N'dbo.app_Function')
          AND name = N'CK_app_Function_DevelopmentStatus'
    )
    BEGIN
        EXEC
        (
            N'ALTER TABLE dbo.app_Function WITH CHECK ADD CONSTRAINT CK_app_Function_DevelopmentStatus
              CHECK (DevelopmentStatus IN (''NOT_STARTED'', ''IN_PROGRESS'', ''COMPLETED''));'
        );
    END;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    MERGE dbo.app_Function AS target
    USING (VALUES
        ('MAIN',                    NULL,       N'主頁',       N'系統主頁',                 NULL,                         0,  0, 'COMPLETED',   1),
        ('ASSET',                   NULL,       N'部門管理',   N'部門功能',                 NULL,                        10,  1, 'IN_PROGRESS', 1),
        ('ASSET.MACHINE_WAREHOUSE', 'ASSET',    N'機邊庫',     N'設備、物料與固定資產',     'MachineWarehouseForm',     10,  1, 'IN_PROGRESS', 1),
        ('SYSTEM',                  NULL,       N'系統管理',   N'系統管理功能',             NULL,                        90,  1, 'IN_PROGRESS', 1),
        ('SYSTEM.USER',             'SYSTEM',   N'使用者管理', N'帳號與部門管理',           'UserManagementForm',       10,  1, 'NOT_STARTED', 1),
        ('SYSTEM.GROUP',            'SYSTEM',   N'安全性群組', N'使用者群組管理',           'SecurityGroupManagementForm', 20, 0, 'NOT_STARTED', 1),
        ('SYSTEM.ROLE',             'SYSTEM',   N'角色與權限', N'功能存取權限管理',         'RoleManagementForm',       30,  1, 'NOT_STARTED', 1),
        ('SYSTEM.FUNCTION',         'SYSTEM',   N'功能清單',   N'系統功能與導覽設定',       'FunctionManagementForm',   40,  0, 'NOT_STARTED', 1),
        ('SYSTEM.SETTINGS',         'SYSTEM',   N'系統設定',   N'系統參數與一般設定',       'SystemSettingsForm',       50,  1, 'NOT_STARTED', 1),
        ('SYSTEM.MANUAL',           'SYSTEM',   N'操作手冊',   N'使用者操作說明',           'UserManualForm',           60,  1, 'NOT_STARTED', 1)
    ) AS source
        (FunctionCode, ParentCode, DisplayName, Description, NavigationTarget, SortOrder, IsVisible, DevelopmentStatus, IsWide)
    ON target.FunctionCode = source.FunctionCode
    WHEN MATCHED THEN UPDATE SET
        target.DisplayName = source.DisplayName,
        target.Description = source.Description,
        target.NavigationTarget = source.NavigationTarget,
        target.SortOrder = source.SortOrder,
        target.IsVisible = source.IsVisible,
        target.DevelopmentStatus = source.DevelopmentStatus,
        target.IsWide = source.IsWide,
        target.IsActive = 1,
        target.UpdatedAt = SYSUTCDATETIME()
    WHEN NOT MATCHED THEN INSERT
        (ParentFunctionId, FunctionCode, DisplayName, Description, NavigationTarget,
         SortOrder, IsVisible, IsActive, DevelopmentStatus, IsWide)
        VALUES
        (NULL, source.FunctionCode, source.DisplayName, source.Description, source.NavigationTarget,
         source.SortOrder, source.IsVisible, 1, source.DevelopmentStatus, source.IsWide);

    UPDATE child
       SET ParentFunctionId = parent.FunctionId
      FROM dbo.app_Function AS child
      INNER JOIN (VALUES
          ('ASSET.MACHINE_WAREHOUSE', 'ASSET'),
          ('SYSTEM.USER', 'SYSTEM'),
          ('SYSTEM.GROUP', 'SYSTEM'),
          ('SYSTEM.ROLE', 'SYSTEM'),
          ('SYSTEM.FUNCTION', 'SYSTEM'),
          ('SYSTEM.SETTINGS', 'SYSTEM'),
          ('SYSTEM.MANUAL', 'SYSTEM')
      ) AS relation(ChildCode, ParentCode) ON relation.ChildCode = child.FunctionCode
      INNER JOIN dbo.app_Function AS parent ON parent.FunctionCode = relation.ParentCode;

    MERGE dbo.auth_Permission AS target
    USING
    (
        SELECT functionData.FunctionId,
               seed.ActionCode,
               seed.PermissionCode,
               seed.DisplayName
        FROM (VALUES
            ('ASSET',                   'ACCESS', 'ASSET.ACCESS',                   N'存取部門管理'),
            ('ASSET.MACHINE_WAREHOUSE', 'VIEW',   'ASSET.MACHINE_WAREHOUSE.VIEW',  N'檢視機邊庫'),
            ('SYSTEM.SETTINGS',         'VIEW',   'SYSTEM.SETTINGS.VIEW',          N'檢視系統設定'),
            ('SYSTEM.MANUAL',           'VIEW',   'SYSTEM.MANUAL.VIEW',            N'檢視操作手冊')
        ) AS seed(FunctionCode, ActionCode, PermissionCode, DisplayName)
        INNER JOIN dbo.app_Function AS functionData
            ON functionData.FunctionCode = seed.FunctionCode
    ) AS source
    ON target.PermissionCode = source.PermissionCode
    WHEN MATCHED THEN UPDATE SET
        target.DisplayName = source.DisplayName,
        target.IsActive = 1
    WHEN NOT MATCHED THEN INSERT
        (FunctionId, ActionCode, PermissionCode, DisplayName)
        VALUES
        (source.FunctionId, source.ActionCode, source.PermissionCode, source.DisplayName);

    MERGE dbo.auth_RolePermission AS target
    USING
    (
        SELECT roleData.RoleId, permissionData.PermissionId
        FROM dbo.auth_Role AS roleData
        CROSS JOIN dbo.auth_Permission AS permissionData
        WHERE roleData.RoleCode = 'SECURITY_ADMIN'
          AND roleData.IsActive = 1
          AND permissionData.IsActive = 1
    ) AS source
    ON target.RoleId = source.RoleId AND target.PermissionId = source.PermissionId
    WHEN MATCHED THEN UPDATE SET target.IsActive = 1
    WHEN NOT MATCHED THEN INSERT (RoleId, PermissionId)
        VALUES (source.RoleId, source.PermissionId);

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
