SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    UPDATE dbo.app_Function
       SET DisplayName = N'功能卡管理',
           Description = N'管理主畫面功能卡與群組設定',
           NavigationTarget = 'FunctionManagementView',
           IsVisible = 0,
           IsActive = 1,
           DevelopmentStatus = 'COMPLETED',
           UpdatedAt = SYSUTCDATETIME()
     WHERE FunctionCode = 'SYSTEM.FUNCTION';

    UPDATE dbo.app_Function
       SET DisplayName = N'系統設定',
           Description = N'系統參數與功能卡設定',
           NavigationTarget = 'SystemSettingsForm',
           IsVisible = 1,
           IsActive = 1,
           DevelopmentStatus = 'COMPLETED',
           UpdatedAt = SYSUTCDATETIME()
     WHERE FunctionCode = 'SYSTEM.SETTINGS';

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
