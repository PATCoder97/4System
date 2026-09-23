SET NOCOUNT ON;
SET XACT_ABORT ON;

IF COL_LENGTH('dbo.auth_UserAccount', 'LoginName') IS NOT NULL
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;

        IF EXISTS
        (
            SELECT 1
            FROM dbo.hr_EmployeeProfile
            WHERE EmployeeCode NOT LIKE 'VNW[0-9][0-9][0-9][0-9][0-9][0-9][0-9]'
        )
            THROW 51020, N'存在不符合 VNW0000000 格式的人員代碼，無法執行移轉。', 1;

        CREATE TABLE #UserIdMap
        (
            LegacyUserId bigint NOT NULL PRIMARY KEY,
            NewUserId varchar(10) NOT NULL UNIQUE
        );

        EXEC sys.sp_executesql N'
            INSERT #UserIdMap(LegacyUserId, NewUserId)
            SELECT UserId,
                   CASE
                       WHEN UPPER(LoginName) LIKE ''VNW[0-9][0-9][0-9][0-9][0-9][0-9][0-9]''
                           THEN UPPER(LoginName)
                       WHEN UPPER(LoginName) = ''ADMIN''
                           THEN ''VNW0014732''
                   END
            FROM dbo.auth_UserAccount;

            IF EXISTS
            (
                SELECT 1
                FROM dbo.auth_UserAccount
                WHERE UPPER(LoginName) <> ''ADMIN''
                  AND UPPER(LoginName) NOT LIKE ''VNW[0-9][0-9][0-9][0-9][0-9][0-9][0-9]''
            )
                THROW 51021, N''存在不符合 VNW0000000 格式的登入帳號，無法執行移轉。'', 1;
        ';

        IF OBJECT_ID(N'dbo.vw_auth_UserEffectivePermission', N'V') IS NOT NULL
            DROP VIEW dbo.vw_auth_UserEffectivePermission;
        IF OBJECT_ID(N'dbo.usp_auth_UserHasPermission', N'P') IS NOT NULL
            DROP PROCEDURE dbo.usp_auth_UserHasPermission;
        IF OBJECT_ID(N'dbo.usp_auth_AddUserToGroup', N'P') IS NOT NULL
            DROP PROCEDURE dbo.usp_auth_AddUserToGroup;
        IF OBJECT_ID(N'dbo.usp_auth_RemoveUserFromGroup', N'P') IS NOT NULL
            DROP PROCEDURE dbo.usp_auth_RemoveUserFromGroup;

        ALTER TABLE dbo.audit_AuditLog DROP CONSTRAINT FK_audit_AuditLog_User;
        ALTER TABLE dbo.auth_GroupRole DROP CONSTRAINT FK_auth_GroupRole_AssignedBy;
        ALTER TABLE dbo.auth_RolePermission DROP CONSTRAINT FK_auth_RolePermission_AssignedBy;
        ALTER TABLE dbo.auth_UserGroup DROP CONSTRAINT FK_auth_UserGroup_AssignedBy;
        ALTER TABLE dbo.auth_UserGroup DROP CONSTRAINT FK_auth_UserGroup_User;
        ALTER TABLE dbo.sys_Setting DROP CONSTRAINT FK_sys_Setting_UpdatedBy;

        DROP INDEX IX_audit_AuditLog_User_CreatedAt ON dbo.audit_AuditLog;
        DROP INDEX IX_auth_UserGroup_Group_Active ON dbo.auth_UserGroup;
        ALTER TABLE dbo.auth_UserGroup DROP CONSTRAINT PK_auth_UserGroup;

        CREATE TABLE dbo.auth_UserAccount_New
        (
            UserId             varchar(10) NOT NULL,
            EmployeeProfileId  bigint NULL,
            DomainAccount      varchar(200) NULL,
            AuthenticationType varchar(20) NOT NULL,
            PasswordHash       varchar(500) NULL,
            SecurityStamp      uniqueidentifier NOT NULL,
            FailedLoginCount   int NOT NULL,
            LockoutEndUtc      datetime2(0) NULL,
            LastLoginAt        datetime2(0) NULL,
            IsActive           bit NOT NULL,
            CreatedAt          datetime2(0) NOT NULL,
            UpdatedAt          datetime2(0) NULL,
            RowVersion         rowversion NOT NULL
        );

        INSERT dbo.auth_UserAccount_New
            (UserId, EmployeeProfileId, DomainAccount, AuthenticationType, PasswordHash,
             SecurityStamp, FailedLoginCount, LockoutEndUtc, LastLoginAt, IsActive, CreatedAt, UpdatedAt)
        SELECT map.NewUserId, account.EmployeeProfileId, account.DomainAccount,
               account.AuthenticationType, account.PasswordHash, account.SecurityStamp,
               account.FailedLoginCount, account.LockoutEndUtc, account.LastLoginAt,
               account.IsActive, account.CreatedAt, account.UpdatedAt
        FROM dbo.auth_UserAccount AS account
        INNER JOIN #UserIdMap AS map ON map.LegacyUserId = account.UserId;

        ALTER TABLE dbo.auth_UserGroup ALTER COLUMN UserId varchar(10) NOT NULL;
        ALTER TABLE dbo.auth_UserGroup ALTER COLUMN AssignedByUserId varchar(10) NULL;
        ALTER TABLE dbo.auth_GroupRole ALTER COLUMN AssignedByUserId varchar(10) NULL;
        ALTER TABLE dbo.auth_RolePermission ALTER COLUMN AssignedByUserId varchar(10) NULL;
        ALTER TABLE dbo.sys_Setting ALTER COLUMN UpdatedByUserId varchar(10) NULL;
        ALTER TABLE dbo.audit_AuditLog ALTER COLUMN UserId varchar(10) NULL;

        UPDATE child SET UserId = map.NewUserId
        FROM dbo.auth_UserGroup AS child
        INNER JOIN #UserIdMap AS map ON map.LegacyUserId = TRY_CONVERT(bigint, child.UserId);

        UPDATE child SET AssignedByUserId = map.NewUserId
        FROM dbo.auth_UserGroup AS child
        INNER JOIN #UserIdMap AS map ON map.LegacyUserId = TRY_CONVERT(bigint, child.AssignedByUserId);

        UPDATE child SET AssignedByUserId = map.NewUserId
        FROM dbo.auth_GroupRole AS child
        INNER JOIN #UserIdMap AS map ON map.LegacyUserId = TRY_CONVERT(bigint, child.AssignedByUserId);

        UPDATE child SET AssignedByUserId = map.NewUserId
        FROM dbo.auth_RolePermission AS child
        INNER JOIN #UserIdMap AS map ON map.LegacyUserId = TRY_CONVERT(bigint, child.AssignedByUserId);

        UPDATE child SET UpdatedByUserId = map.NewUserId
        FROM dbo.sys_Setting AS child
        INNER JOIN #UserIdMap AS map ON map.LegacyUserId = TRY_CONVERT(bigint, child.UpdatedByUserId);

        UPDATE child SET UserId = map.NewUserId
        FROM dbo.audit_AuditLog AS child
        INNER JOIN #UserIdMap AS map ON map.LegacyUserId = TRY_CONVERT(bigint, child.UserId);

        EXEC sys.sp_executesql N'
            UPDATE dbo.audit_AuditLog
               SET LoginName = ''VNW0014732''
             WHERE UPPER(LoginName) = ''ADMIN'';
        ';
        EXEC sys.sp_rename N'dbo.audit_AuditLog.LoginName', N'AttemptedUserId', N'COLUMN';

        DROP TABLE dbo.auth_UserAccount;
        EXEC sys.sp_rename N'dbo.auth_UserAccount_New', N'auth_UserAccount';

        ALTER TABLE dbo.auth_UserAccount ADD CONSTRAINT PK_auth_UserAccount
            PRIMARY KEY CLUSTERED (UserId);
        ALTER TABLE dbo.auth_UserAccount ADD CONSTRAINT UQ_auth_UserAccount_Employee
            UNIQUE (EmployeeProfileId);
        ALTER TABLE dbo.auth_UserAccount ADD CONSTRAINT FK_auth_UserAccount_Employee
            FOREIGN KEY (EmployeeProfileId) REFERENCES dbo.hr_EmployeeProfile(EmployeeProfileId);
        ALTER TABLE dbo.auth_UserAccount ADD CONSTRAINT CK_auth_UserAccount_AuthType
            CHECK (AuthenticationType IN ('WINDOWS', 'LOCAL'));
        ALTER TABLE dbo.auth_UserAccount ADD CONSTRAINT CK_auth_UserAccount_UserId
            CHECK (UserId = UPPER(UserId)
                   AND UserId LIKE 'VNW[0-9][0-9][0-9][0-9][0-9][0-9][0-9]');
        ALTER TABLE dbo.auth_UserAccount ADD CONSTRAINT CK_auth_UserAccount_Password
            CHECK (AuthenticationType = 'WINDOWS' OR (AuthenticationType = 'LOCAL' AND PasswordHash IS NOT NULL));

        CREATE UNIQUE INDEX UX_auth_UserAccount_DomainAccount
            ON dbo.auth_UserAccount(DomainAccount) WHERE DomainAccount IS NOT NULL;
        CREATE INDEX IX_auth_UserAccount_Active
            ON dbo.auth_UserAccount(IsActive, UserId) INCLUDE (EmployeeProfileId, LockoutEndUtc);

        ALTER TABLE dbo.auth_UserGroup ADD CONSTRAINT PK_auth_UserGroup
            PRIMARY KEY CLUSTERED (UserId, GroupId);
        ALTER TABLE dbo.auth_UserGroup ADD CONSTRAINT FK_auth_UserGroup_User
            FOREIGN KEY (UserId) REFERENCES dbo.auth_UserAccount(UserId);
        ALTER TABLE dbo.auth_UserGroup ADD CONSTRAINT FK_auth_UserGroup_AssignedBy
            FOREIGN KEY (AssignedByUserId) REFERENCES dbo.auth_UserAccount(UserId);
        CREATE INDEX IX_auth_UserGroup_Group_Active
            ON dbo.auth_UserGroup(GroupId, IsActive, ExpiresAt) INCLUDE (UserId);

        ALTER TABLE dbo.auth_GroupRole ADD CONSTRAINT FK_auth_GroupRole_AssignedBy
            FOREIGN KEY (AssignedByUserId) REFERENCES dbo.auth_UserAccount(UserId);
        ALTER TABLE dbo.auth_RolePermission ADD CONSTRAINT FK_auth_RolePermission_AssignedBy
            FOREIGN KEY (AssignedByUserId) REFERENCES dbo.auth_UserAccount(UserId);
        ALTER TABLE dbo.sys_Setting ADD CONSTRAINT FK_sys_Setting_UpdatedBy
            FOREIGN KEY (UpdatedByUserId) REFERENCES dbo.auth_UserAccount(UserId);
        ALTER TABLE dbo.audit_AuditLog ADD CONSTRAINT FK_audit_AuditLog_User
            FOREIGN KEY (UserId) REFERENCES dbo.auth_UserAccount(UserId);
        CREATE INDEX IX_audit_AuditLog_User_CreatedAt
            ON dbo.audit_AuditLog(UserId, CreatedAt DESC)
            INCLUDE (ActionCode, EntityName, EntityId);

        DROP INDEX IX_hr_EmployeeProfile_Department_Status ON dbo.hr_EmployeeProfile;
        ALTER TABLE dbo.hr_EmployeeProfile DROP CONSTRAINT UQ_hr_EmployeeProfile_EmployeeCode;
        ALTER TABLE dbo.hr_EmployeeProfile ALTER COLUMN EmployeeCode varchar(10) NOT NULL;
        ALTER TABLE dbo.hr_EmployeeProfile ADD CONSTRAINT UQ_hr_EmployeeProfile_EmployeeCode
            UNIQUE (EmployeeCode);
        ALTER TABLE dbo.hr_EmployeeProfile ADD CONSTRAINT CK_hr_EmployeeProfile_EmployeeCode
            CHECK (EmployeeCode = UPPER(EmployeeCode)
                   AND EmployeeCode LIKE 'VNW[0-9][0-9][0-9][0-9][0-9][0-9][0-9]');
        CREATE INDEX IX_hr_EmployeeProfile_Department_Status
            ON dbo.hr_EmployeeProfile(DepartmentId, EmploymentStatus)
            INCLUDE (EmployeeCode, FullName, JobTitleId);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;

IF OBJECT_ID(N'dbo.CK_auth_UserAccount_UserId', N'C') IS NOT NULL
    ALTER TABLE dbo.auth_UserAccount DROP CONSTRAINT CK_auth_UserAccount_UserId;
ALTER TABLE dbo.auth_UserAccount WITH CHECK ADD CONSTRAINT CK_auth_UserAccount_UserId
    CHECK (UserId = UPPER(UserId)
           AND UserId LIKE 'VNW[0-9][0-9][0-9][0-9][0-9][0-9][0-9]');

IF OBJECT_ID(N'dbo.CK_hr_EmployeeProfile_EmployeeCode', N'C') IS NOT NULL
    ALTER TABLE dbo.hr_EmployeeProfile DROP CONSTRAINT CK_hr_EmployeeProfile_EmployeeCode;
ALTER TABLE dbo.hr_EmployeeProfile WITH CHECK ADD CONSTRAINT CK_hr_EmployeeProfile_EmployeeCode
    CHECK (EmployeeCode = UPPER(EmployeeCode)
           AND EmployeeCode LIKE 'VNW[0-9][0-9][0-9][0-9][0-9][0-9][0-9]');
GO

IF OBJECT_ID(N'dbo.vw_auth_UserEffectivePermission', N'V') IS NOT NULL
    DROP VIEW dbo.vw_auth_UserEffectivePermission;
GO

CREATE VIEW dbo.vw_auth_UserEffectivePermission
AS
    SELECT DISTINCT
        u.UserId,
        g.GroupId,
        g.GroupCode,
        r.RoleId,
        r.RoleCode,
        f.FunctionId,
        f.FunctionCode,
        p.PermissionId,
        p.PermissionCode,
        p.ActionCode
    FROM dbo.auth_UserAccount AS u
    INNER JOIN dbo.auth_UserGroup AS ug ON ug.UserId = u.UserId
        AND ug.IsActive = 1
        AND (ug.ExpiresAt IS NULL OR ug.ExpiresAt > SYSUTCDATETIME())
    INNER JOIN dbo.auth_SecurityGroup AS g ON g.GroupId = ug.GroupId AND g.IsActive = 1
    INNER JOIN dbo.auth_GroupRole AS gr ON gr.GroupId = g.GroupId AND gr.IsActive = 1
    INNER JOIN dbo.auth_Role AS r ON r.RoleId = gr.RoleId AND r.IsActive = 1
    INNER JOIN dbo.auth_RolePermission AS rp ON rp.RoleId = r.RoleId AND rp.IsActive = 1
    INNER JOIN dbo.auth_Permission AS p ON p.PermissionId = rp.PermissionId AND p.IsActive = 1
    INNER JOIN dbo.app_Function AS f ON f.FunctionId = p.FunctionId AND f.IsActive = 1
    WHERE u.IsActive = 1;
GO

IF OBJECT_ID(N'dbo.usp_auth_UserHasPermission', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_auth_UserHasPermission;
GO

CREATE PROCEDURE dbo.usp_auth_UserHasPermission
    @UserId varchar(10),
    @PermissionCode varchar(150)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CAST(CASE WHEN EXISTS
    (
        SELECT 1 FROM dbo.vw_auth_UserEffectivePermission
        WHERE UserId = @UserId AND PermissionCode = @PermissionCode
    ) THEN 1 ELSE 0 END AS bit) AS IsGranted;
END;
GO

IF OBJECT_ID(N'dbo.usp_auth_AddUserToGroup', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_auth_AddUserToGroup;
GO

CREATE PROCEDURE dbo.usp_auth_AddUserToGroup
    @UserId varchar(10),
    @GroupCode varchar(80),
    @AssignedByUserId varchar(10) = NULL,
    @ExpiresAt datetime2(0) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    DECLARE @GroupId int =
    (
        SELECT GroupId FROM dbo.auth_SecurityGroup
        WHERE GroupCode = @GroupCode AND IsActive = 1
    );
    IF @GroupId IS NULL THROW 51001, N'安全性群組不存在或已停用。', 1;
    IF NOT EXISTS (SELECT 1 FROM dbo.auth_UserAccount WHERE UserId = @UserId AND IsActive = 1)
        THROW 51002, N'使用者不存在或已停用。', 1;
    UPDATE dbo.auth_UserGroup
       SET IsActive = 1, AssignedAt = SYSUTCDATETIME(),
           AssignedByUserId = @AssignedByUserId, ExpiresAt = @ExpiresAt
     WHERE UserId = @UserId AND GroupId = @GroupId;
    IF @@ROWCOUNT = 0
        INSERT dbo.auth_UserGroup(UserId, GroupId, AssignedByUserId, ExpiresAt)
        VALUES (@UserId, @GroupId, @AssignedByUserId, @ExpiresAt);
    INSERT dbo.audit_AuditLog(UserId, ActionCode, EntityName, EntityId, Description)
    VALUES (@AssignedByUserId, 'SECURITY.USER_GROUP.ADD', 'auth_UserGroup',
            CONCAT(@UserId, ':', @GroupId), CONCAT('Added user to group ', @GroupCode));
END;
GO

IF OBJECT_ID(N'dbo.usp_auth_RemoveUserFromGroup', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_auth_RemoveUserFromGroup;
GO

CREATE PROCEDURE dbo.usp_auth_RemoveUserFromGroup
    @UserId varchar(10),
    @GroupCode varchar(80),
    @RemovedByUserId varchar(10) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    DECLARE @GroupId int =
    (
        SELECT GroupId FROM dbo.auth_SecurityGroup WHERE GroupCode = @GroupCode
    );
    IF @GroupId IS NULL THROW 51003, N'安全性群組不存在。', 1;
    UPDATE dbo.auth_UserGroup SET IsActive = 0
     WHERE UserId = @UserId AND GroupId = @GroupId AND IsActive = 1;
    IF @@ROWCOUNT = 0 THROW 51004, N'找不到有效的使用者群組成員資格。', 1;
    INSERT dbo.audit_AuditLog(UserId, ActionCode, EntityName, EntityId, Description)
    VALUES (@RemovedByUserId, 'SECURITY.USER_GROUP.REMOVE', 'auth_UserGroup',
            CONCAT(@UserId, ':', @GroupId), CONCAT('Removed user from group ', @GroupCode));
END;
GO
