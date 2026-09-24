SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    UPDATE dbo.app_Function
       SET DisplayName = N'人員管理',
           Description = N'管理人員資料與登入帳號',
           NavigationTarget = 'EmployeeManagementView',
           IsVisible = 0,
           IsActive = 1,
           DevelopmentStatus = 'COMPLETED',
           UpdatedAt = SYSUTCDATETIME()
     WHERE FunctionCode = 'SYSTEM.USER';

    UPDATE dbo.app_Function
       SET DisplayName = N'系統管理',
           Description = N'管理人員、權限與系統功能設定',
           NavigationTarget = 'SystemSettingsForm',
           IsVisible = 1,
           IsActive = 1,
           DevelopmentStatus = 'COMPLETED',
           UpdatedAt = SYSUTCDATETIME()
     WHERE FunctionCode = 'SYSTEM.SETTINGS';

    DECLARE @SystemFunctionId int = (SELECT FunctionId FROM dbo.app_Function WHERE FunctionCode = 'SYSTEM');

    MERGE dbo.app_Function AS target
    USING (VALUES ('SYSTEM.USER.PERMISSION', @SystemFunctionId, N'人員權限', N'管理人員所屬安全性群組及檢視有效權限', 'EmployeePermissionView', 20))
       AS source(FunctionCode, ParentFunctionId, DisplayName, Description, NavigationTarget, SortOrder)
       ON target.FunctionCode = source.FunctionCode
    WHEN MATCHED THEN UPDATE SET
        ParentFunctionId = source.ParentFunctionId, DisplayName = source.DisplayName,
        Description = source.Description, NavigationTarget = source.NavigationTarget,
        SortOrder = source.SortOrder, IsVisible = 0, IsActive = 1,
        DevelopmentStatus = 'COMPLETED', UpdatedAt = SYSUTCDATETIME()
    WHEN NOT MATCHED THEN INSERT
        (ParentFunctionId, FunctionCode, DisplayName, Description, NavigationTarget, SortOrder, IsVisible, IsActive, DevelopmentStatus, IsWide)
        VALUES (source.ParentFunctionId, source.FunctionCode, source.DisplayName, source.Description, source.NavigationTarget, source.SortOrder, 0, 1, 'COMPLETED', 0);

    MERGE dbo.auth_Permission AS target
    USING
    (
        SELECT f.FunctionId, seed.ActionCode, seed.PermissionCode, seed.DisplayName
        FROM (VALUES
            ('VIEW',  'SYSTEM.USER.PERMISSION.VIEW',  N'檢視人員權限'),
            ('ADMIN', 'SYSTEM.USER.PERMISSION.ADMIN', N'管理人員權限')
        ) seed(ActionCode, PermissionCode, DisplayName)
        CROSS JOIN (SELECT FunctionId FROM dbo.app_Function WHERE FunctionCode = 'SYSTEM.USER.PERMISSION') f
    ) AS source
       ON target.PermissionCode = source.PermissionCode
    WHEN MATCHED THEN UPDATE SET DisplayName = source.DisplayName, IsActive = 1
    WHEN NOT MATCHED THEN INSERT (FunctionId, ActionCode, PermissionCode, DisplayName, IsActive)
        VALUES (source.FunctionId, source.ActionCode, source.PermissionCode, source.DisplayName, 1);

    MERGE dbo.auth_RolePermission AS target
    USING
    (
        SELECT role.RoleId, permission.PermissionId
        FROM dbo.auth_Role role
        CROSS JOIN dbo.auth_Permission permission
        WHERE role.RoleCode = 'SECURITY_ADMIN'
          AND permission.PermissionCode IN ('SYSTEM.USER.PERMISSION.VIEW', 'SYSTEM.USER.PERMISSION.ADMIN')
    ) source
       ON target.RoleId = source.RoleId AND target.PermissionId = source.PermissionId
    WHEN MATCHED THEN UPDATE SET IsActive = 1
    WHEN NOT MATCHED THEN INSERT (RoleId, PermissionId) VALUES (source.RoleId, source.PermissionId);

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
