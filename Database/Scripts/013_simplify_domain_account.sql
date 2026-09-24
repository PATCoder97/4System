SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.auth_UserAccount') AND name = N'UX_auth_UserAccount_DomainAccount')
        DROP INDEX UX_auth_UserAccount_DomainAccount ON dbo.auth_UserAccount;

    IF EXISTS (SELECT 1 FROM sys.check_constraints WHERE parent_object_id = OBJECT_ID(N'dbo.auth_UserAccount') AND name = N'CK_auth_UserAccount_AuthType')
        ALTER TABLE dbo.auth_UserAccount DROP CONSTRAINT CK_auth_UserAccount_AuthType;
    IF EXISTS (SELECT 1 FROM sys.check_constraints WHERE parent_object_id = OBJECT_ID(N'dbo.auth_UserAccount') AND name = N'CK_auth_UserAccount_Password')
        ALTER TABLE dbo.auth_UserAccount DROP CONSTRAINT CK_auth_UserAccount_Password;
    IF EXISTS (SELECT 1 FROM sys.check_constraints WHERE parent_object_id = OBJECT_ID(N'dbo.auth_UserAccount') AND name = N'CK_auth_UserAccount_DomainOnly')
        ALTER TABLE dbo.auth_UserAccount DROP CONSTRAINT CK_auth_UserAccount_DomainOnly;

    DECLARE @dropDefaults nvarchar(max) = N'';
    SELECT @dropDefaults = @dropDefaults + N'ALTER TABLE dbo.auth_UserAccount DROP CONSTRAINT ' + QUOTENAME(dc.name) + N';'
      FROM sys.default_constraints dc
      JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id
     WHERE dc.parent_object_id = OBJECT_ID(N'dbo.auth_UserAccount')
       AND c.name IN (N'AuthenticationType', N'DomainAccount');
    IF @dropDefaults <> N'' EXEC sys.sp_executesql @dropDefaults;

    IF COL_LENGTH(N'dbo.auth_UserAccount', N'AuthenticationType') IS NOT NULL
        ALTER TABLE dbo.auth_UserAccount DROP COLUMN AuthenticationType;
    IF COL_LENGTH(N'dbo.auth_UserAccount', N'DomainAccount') IS NOT NULL
        ALTER TABLE dbo.auth_UserAccount DROP COLUMN DomainAccount;

    IF COL_LENGTH(N'dbo.auth_UserAccount', N'PasswordHash') IS NOT NULL
       AND COL_LENGTH(N'dbo.auth_UserAccount', N'CachedDomainPasswordHash') IS NULL
        EXEC sys.sp_rename N'dbo.auth_UserAccount.PasswordHash', N'CachedDomainPasswordHash', N'COLUMN';

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
