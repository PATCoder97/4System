SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    IF COL_LENGTH(N'dbo.auth_UserAccount', N'LastDomainValidatedAt') IS NULL
        ALTER TABLE dbo.auth_UserAccount ADD LastDomainValidatedAt datetime2(0) NULL;

    UPDATE dbo.auth_UserAccount
       SET DomainAccount = UserId,
           AuthenticationType = 'WINDOWS',
           UpdatedAt = SYSUTCDATETIME()
     WHERE DomainAccount IS NULL
        OR DomainAccount <> UserId
        OR AuthenticationType <> 'WINDOWS';

    IF EXISTS (SELECT 1 FROM sys.check_constraints WHERE parent_object_id = OBJECT_ID(N'dbo.auth_UserAccount') AND name = N'CK_auth_UserAccount_AuthType')
        ALTER TABLE dbo.auth_UserAccount DROP CONSTRAINT CK_auth_UserAccount_AuthType;

    IF EXISTS (SELECT 1 FROM sys.check_constraints WHERE parent_object_id = OBJECT_ID(N'dbo.auth_UserAccount') AND name = N'CK_auth_UserAccount_Password')
        ALTER TABLE dbo.auth_UserAccount DROP CONSTRAINT CK_auth_UserAccount_Password;

    IF EXISTS (SELECT 1 FROM sys.check_constraints WHERE parent_object_id = OBJECT_ID(N'dbo.auth_UserAccount') AND name = N'CK_auth_UserAccount_DomainOnly')
        ALTER TABLE dbo.auth_UserAccount DROP CONSTRAINT CK_auth_UserAccount_DomainOnly;

    IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.auth_UserAccount') AND name = N'UX_auth_UserAccount_DomainAccount')
        DROP INDEX UX_auth_UserAccount_DomainAccount ON dbo.auth_UserAccount;

    ALTER TABLE dbo.auth_UserAccount ALTER COLUMN DomainAccount varchar(200) NOT NULL;

    CREATE UNIQUE INDEX UX_auth_UserAccount_DomainAccount
        ON dbo.auth_UserAccount(DomainAccount);

    ALTER TABLE dbo.auth_UserAccount WITH CHECK ADD CONSTRAINT CK_auth_UserAccount_DomainOnly
        CHECK (AuthenticationType = 'WINDOWS');

    IF NOT EXISTS
    (
        SELECT 1 FROM sys.default_constraints dc
        INNER JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id
        WHERE dc.parent_object_id = OBJECT_ID(N'dbo.auth_UserAccount') AND c.name = N'AuthenticationType'
    )
        ALTER TABLE dbo.auth_UserAccount ADD CONSTRAINT DF_auth_UserAccount_AuthenticationType_DomainOnly DEFAULT ('WINDOWS') FOR AuthenticationType;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
