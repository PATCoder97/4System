SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    IF COL_LENGTH(N'dbo.auth_UserAccount', N'CreatedAt') IS NULL
        THROW 50001, N'找不到 auth_UserAccount.CreatedAt 欄位。請先依序執行前置資料庫腳本。', 1;

    IF NOT EXISTS
    (
        SELECT 1
        FROM sys.default_constraints AS defaultConstraint
        INNER JOIN sys.columns AS columnRow
            ON columnRow.object_id = defaultConstraint.parent_object_id
           AND columnRow.column_id = defaultConstraint.parent_column_id
        WHERE defaultConstraint.parent_object_id = OBJECT_ID(N'dbo.auth_UserAccount')
          AND columnRow.name = N'CreatedAt'
    )
    BEGIN
        ALTER TABLE dbo.auth_UserAccount
            ADD CONSTRAINT DF_auth_UserAccount_CreatedAt
                DEFAULT (SYSUTCDATETIME()) FOR CreatedAt;
    END;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
