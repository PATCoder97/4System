SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    UPDATE dbo.app_Function
       SET DisplayName = N'安全性群組管理',
           Description = N'管理安全性群組、群組角色與群組成員',
           NavigationTarget = 'SecurityGroupManagementView',
           IsVisible = 0,
           IsActive = 1,
           DevelopmentStatus = 'COMPLETED',
           UpdatedAt = SYSUTCDATETIME()
     WHERE FunctionCode = 'SYSTEM.GROUP';

    UPDATE dbo.auth_Permission
       SET DisplayName = CASE PermissionCode
           WHEN 'SYSTEM.GROUP.VIEW' THEN N'檢視安全性群組'
           WHEN 'SYSTEM.GROUP.ADMIN' THEN N'管理安全性群組'
           ELSE DisplayName END,
           IsActive = 1
     WHERE PermissionCode IN ('SYSTEM.GROUP.VIEW', 'SYSTEM.GROUP.ADMIN');

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
