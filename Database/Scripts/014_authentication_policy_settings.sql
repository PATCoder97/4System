SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID(N'dbo.sys_SystemSetting', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.sys_SystemSetting
        (
            SettingKey      varchar(100) NOT NULL CONSTRAINT PK_sys_SystemSetting PRIMARY KEY,
            SettingValue    nvarchar(1000) NOT NULL,
            DisplayName     nvarchar(200) NOT NULL,
            Description     nvarchar(500) NULL,
            ValueType       varchar(20) NOT NULL CONSTRAINT DF_sys_SystemSetting_ValueType DEFAULT ('STRING'),
            MinimumValue    int NULL,
            MaximumValue    int NULL,
            IsSensitive     bit NOT NULL CONSTRAINT DF_sys_SystemSetting_IsSensitive DEFAULT (0),
            UpdatedByUserId varchar(10) NULL,
            UpdatedAt       datetime2(0) NOT NULL CONSTRAINT DF_sys_SystemSetting_UpdatedAt DEFAULT (SYSUTCDATETIME()),
            RowVersion      rowversion NOT NULL,
            CONSTRAINT CK_sys_SystemSetting_ValueType CHECK (ValueType IN ('STRING', 'INTEGER', 'BOOLEAN')),
            CONSTRAINT FK_sys_SystemSetting_UpdatedBy FOREIGN KEY (UpdatedByUserId) REFERENCES dbo.auth_UserAccount(UserId)
        );
    END;

    MERGE dbo.sys_SystemSetting AS target
    USING (VALUES
        ('AUTH.MAX_FAILED_ATTEMPTS', N'5',  N'登入失敗次數上限', N'達到此失敗次數後暫時鎖定帳號。', 'INTEGER', 1, 20),
        ('AUTH.LOCKOUT_MINUTES',     N'15', N'帳號鎖定時間（分鐘）', N'登入失敗次數達上限後的鎖定時間。', 'INTEGER', 1, 1440)
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

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
