SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @SystemFunctionId int = (SELECT FunctionId FROM dbo.app_Function WHERE FunctionCode = 'SYSTEM');

    MERGE dbo.app_Function AS target
    USING (VALUES ('SYSTEM.AUDIT', @SystemFunctionId, N'稽核記錄', N'查詢、檢視及匯出系統稽核記錄', 'AuditLogView', 50))
       AS source(FunctionCode, ParentFunctionId, DisplayName, Description, NavigationTarget, SortOrder)
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
            ('VIEW',   'SYSTEM.AUDIT.VIEW',   N'檢視稽核記錄'),
            ('EXPORT', 'SYSTEM.AUDIT.EXPORT', N'匯出稽核記錄')
        ) seed(ActionCode, PermissionCode, DisplayName)
        CROSS JOIN (SELECT FunctionId FROM dbo.app_Function WHERE FunctionCode = 'SYSTEM.AUDIT') functionRow
    ) AS source
       ON target.PermissionCode = source.PermissionCode
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
          AND permission.PermissionCode IN ('SYSTEM.AUDIT.VIEW', 'SYSTEM.AUDIT.EXPORT')
    ) source
       ON target.RoleId = source.RoleId AND target.PermissionId = source.PermissionId
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
