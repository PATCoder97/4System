/*
    Standardize employee names as explicit Traditional Chinese and Vietnamese fields.
    Existing values are preserved:
      PreferredName -> DisplayNameTW
      FullName      -> DisplayNameVN
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID(N'dbo.hr_EmployeeProfile', N'U') IS NULL
        THROW 51030, N'找不到人員資料表 dbo.hr_EmployeeProfile。', 1;

    IF COL_LENGTH(N'dbo.hr_EmployeeProfile', N'DisplayNameTW') IS NULL
       AND COL_LENGTH(N'dbo.hr_EmployeeProfile', N'PreferredName') IS NOT NULL
        EXEC sys.sp_rename
            N'dbo.hr_EmployeeProfile.PreferredName',
            N'DisplayNameTW',
            N'COLUMN';

    IF COL_LENGTH(N'dbo.hr_EmployeeProfile', N'DisplayNameVN') IS NULL
       AND COL_LENGTH(N'dbo.hr_EmployeeProfile', N'FullName') IS NOT NULL
        EXEC sys.sp_rename
            N'dbo.hr_EmployeeProfile.FullName',
            N'DisplayNameVN',
            N'COLUMN';

    IF COL_LENGTH(N'dbo.hr_EmployeeProfile', N'DisplayNameTW') IS NULL
       OR COL_LENGTH(N'dbo.hr_EmployeeProfile', N'DisplayNameVN') IS NULL
        THROW 51031, N'無法建立完整的中越文姓名欄位。', 1;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    THROW;
END CATCH;
