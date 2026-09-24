SET NOCOUNT ON;
SET XACT_ABORT ON;
BEGIN TRY
    BEGIN TRANSACTION;
    UPDATE dbo.app_Function SET DisplayName=N'角色與權限管理', Description=N'管理角色、權限矩陣及影響範圍', NavigationTarget='RolePermissionManagementView', IsVisible=0, IsActive=1, DevelopmentStatus='COMPLETED', UpdatedAt=SYSUTCDATETIME() WHERE FunctionCode='SYSTEM.ROLE';
    UPDATE dbo.auth_Permission SET DisplayName=CASE PermissionCode WHEN 'SYSTEM.ROLE.VIEW' THEN N'檢視角色與權限' WHEN 'SYSTEM.ROLE.ADMIN' THEN N'管理角色與權限' ELSE DisplayName END, IsActive=1 WHERE PermissionCode IN ('SYSTEM.ROLE.VIEW','SYSTEM.ROLE.ADMIN');
    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
