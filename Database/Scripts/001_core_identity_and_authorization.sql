/*
    Winform4System - Core identity and authorization schema
    Model: User -> SecurityGroup -> Role -> Permission -> Function
    Target: Microsoft SQL Server

    Run this script once on a new database. It is intentionally database-name agnostic.
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID(N'dbo.dm_Department', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.dm_Department
        (
            DepartmentId       int IDENTITY(1,1) NOT NULL,
            DepartmentCode     varchar(30) NOT NULL,
            DepartmentName     nvarchar(200) NOT NULL,
            ParentDepartmentId int NULL,
            SortOrder          int NOT NULL CONSTRAINT DF_dm_Department_SortOrder DEFAULT (0),
            IsActive           bit NOT NULL CONSTRAINT DF_dm_Department_IsActive DEFAULT (1),
            CreatedAt          datetime2(0) NOT NULL CONSTRAINT DF_dm_Department_CreatedAt DEFAULT (SYSUTCDATETIME()),
            UpdatedAt          datetime2(0) NULL,
            RowVersion         rowversion NOT NULL,
            CONSTRAINT PK_dm_Department PRIMARY KEY CLUSTERED (DepartmentId),
            CONSTRAINT UQ_dm_Department_Code UNIQUE (DepartmentCode),
            CONSTRAINT FK_dm_Department_Parent FOREIGN KEY (ParentDepartmentId)
                REFERENCES dbo.dm_Department(DepartmentId),
            CONSTRAINT CK_dm_Department_NotSelf CHECK (ParentDepartmentId IS NULL OR ParentDepartmentId <> DepartmentId)
        );

        CREATE INDEX IX_dm_Department_Parent_Active
            ON dbo.dm_Department(ParentDepartmentId, IsActive, SortOrder);
    END;

    IF OBJECT_ID(N'dbo.dm_JobTitle', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.dm_JobTitle
        (
            JobTitleId    int IDENTITY(1,1) NOT NULL,
            JobTitleCode  varchar(30) NOT NULL,
            JobTitleName  nvarchar(200) NOT NULL,
            SortOrder     int NOT NULL CONSTRAINT DF_dm_JobTitle_SortOrder DEFAULT (0),
            IsActive      bit NOT NULL CONSTRAINT DF_dm_JobTitle_IsActive DEFAULT (1),
            CreatedAt     datetime2(0) NOT NULL CONSTRAINT DF_dm_JobTitle_CreatedAt DEFAULT (SYSUTCDATETIME()),
            UpdatedAt     datetime2(0) NULL,
            RowVersion    rowversion NOT NULL,
            CONSTRAINT PK_dm_JobTitle PRIMARY KEY CLUSTERED (JobTitleId),
            CONSTRAINT UQ_dm_JobTitle_Code UNIQUE (JobTitleCode)
        );
    END;

    IF OBJECT_ID(N'dbo.hr_EmployeeProfile', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.hr_EmployeeProfile
        (
            EmployeeProfileId  bigint IDENTITY(1,1) NOT NULL,
            EmployeeCode       varchar(30) NOT NULL,
            FullName           nvarchar(200) NOT NULL,
            PreferredName      nvarchar(100) NULL,
            DepartmentId       int NOT NULL,
            JobTitleId         int NULL,
            WorkEmail          varchar(200) NULL,
            WorkPhone          varchar(30) NULL,
            HireDate           date NULL,
            JobEffectiveDate   date NULL,
            ResignDate         date NULL,
            EmploymentStatus   tinyint NOT NULL CONSTRAINT DF_hr_EmployeeProfile_Status DEFAULT (1),
            CreatedAt          datetime2(0) NOT NULL CONSTRAINT DF_hr_EmployeeProfile_CreatedAt DEFAULT (SYSUTCDATETIME()),
            UpdatedAt          datetime2(0) NULL,
            RowVersion         rowversion NOT NULL,
            CONSTRAINT PK_hr_EmployeeProfile PRIMARY KEY CLUSTERED (EmployeeProfileId),
            CONSTRAINT UQ_hr_EmployeeProfile_EmployeeCode UNIQUE (EmployeeCode),
            CONSTRAINT FK_hr_EmployeeProfile_Department FOREIGN KEY (DepartmentId)
                REFERENCES dbo.dm_Department(DepartmentId),
            CONSTRAINT FK_hr_EmployeeProfile_JobTitle FOREIGN KEY (JobTitleId)
                REFERENCES dbo.dm_JobTitle(JobTitleId),
            CONSTRAINT CK_hr_EmployeeProfile_Status CHECK (EmploymentStatus IN (0, 1, 2, 3)),
            CONSTRAINT CK_hr_EmployeeProfile_Dates CHECK (ResignDate IS NULL OR HireDate IS NULL OR ResignDate >= HireDate)
        );

        CREATE INDEX IX_hr_EmployeeProfile_Department_Status
            ON dbo.hr_EmployeeProfile(DepartmentId, EmploymentStatus)
            INCLUDE (EmployeeCode, FullName, JobTitleId);
    END;

    IF OBJECT_ID(N'dbo.hr_EmployeePrivate', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.hr_EmployeePrivate
        (
            EmployeeProfileId bigint NOT NULL,
            DateOfBirth       date NULL,
            GenderCode        varchar(20) NULL,
            CitizenId         varchar(50) NULL,
            NationalityCode   varchar(10) NULL,
            PersonalEmail     varchar(200) NULL,
            PersonalPhone     varchar(30) NULL,
            Address           nvarchar(500) NULL,
            EducationLevel    nvarchar(100) NULL,
            EducationName     nvarchar(200) NULL,
            UpdatedAt         datetime2(0) NOT NULL CONSTRAINT DF_hr_EmployeePrivate_UpdatedAt DEFAULT (SYSUTCDATETIME()),
            RowVersion        rowversion NOT NULL,
            CONSTRAINT PK_hr_EmployeePrivate PRIMARY KEY CLUSTERED (EmployeeProfileId),
            CONSTRAINT FK_hr_EmployeePrivate_Profile FOREIGN KEY (EmployeeProfileId)
                REFERENCES dbo.hr_EmployeeProfile(EmployeeProfileId)
        );

        CREATE UNIQUE INDEX UX_hr_EmployeePrivate_CitizenId
            ON dbo.hr_EmployeePrivate(CitizenId)
            WHERE CitizenId IS NOT NULL;
    END;

    IF OBJECT_ID(N'dbo.auth_UserAccount', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.auth_UserAccount
        (
            UserId              bigint IDENTITY(1,1) NOT NULL,
            EmployeeProfileId   bigint NULL,
            LoginName           varchar(100) NOT NULL,
            NormalizedLoginName AS UPPER(LoginName) PERSISTED,
            DomainAccount       varchar(200) NULL,
            AuthenticationType  varchar(20) NOT NULL CONSTRAINT DF_auth_UserAccount_AuthType DEFAULT ('WINDOWS'),
            PasswordHash        varchar(500) NULL,
            SecurityStamp       uniqueidentifier NOT NULL CONSTRAINT DF_auth_UserAccount_Stamp DEFAULT (NEWID()),
            FailedLoginCount    int NOT NULL CONSTRAINT DF_auth_UserAccount_Failed DEFAULT (0),
            LockoutEndUtc       datetime2(0) NULL,
            LastLoginAt         datetime2(0) NULL,
            IsActive            bit NOT NULL CONSTRAINT DF_auth_UserAccount_IsActive DEFAULT (1),
            CreatedAt           datetime2(0) NOT NULL CONSTRAINT DF_auth_UserAccount_CreatedAt DEFAULT (SYSUTCDATETIME()),
            UpdatedAt           datetime2(0) NULL,
            RowVersion          rowversion NOT NULL,
            CONSTRAINT PK_auth_UserAccount PRIMARY KEY CLUSTERED (UserId),
            CONSTRAINT UQ_auth_UserAccount_Login UNIQUE (NormalizedLoginName),
            CONSTRAINT UQ_auth_UserAccount_Employee UNIQUE (EmployeeProfileId),
            CONSTRAINT FK_auth_UserAccount_Employee FOREIGN KEY (EmployeeProfileId)
                REFERENCES dbo.hr_EmployeeProfile(EmployeeProfileId),
            CONSTRAINT CK_auth_UserAccount_AuthType CHECK (AuthenticationType IN ('WINDOWS', 'LOCAL')),
            CONSTRAINT CK_auth_UserAccount_Password CHECK
            (
                AuthenticationType = 'WINDOWS'
                OR (AuthenticationType = 'LOCAL' AND PasswordHash IS NOT NULL)
            )
        );

        CREATE UNIQUE INDEX UX_auth_UserAccount_DomainAccount
            ON dbo.auth_UserAccount(DomainAccount)
            WHERE DomainAccount IS NOT NULL;

        CREATE INDEX IX_auth_UserAccount_Active
            ON dbo.auth_UserAccount(IsActive, UserId)
            INCLUDE (LoginName, EmployeeProfileId, LockoutEndUtc);
    END;

    IF OBJECT_ID(N'dbo.auth_SecurityGroup', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.auth_SecurityGroup
        (
            GroupId        int IDENTITY(1,1) NOT NULL,
            GroupCode      varchar(80) NOT NULL,
            GroupName      nvarchar(200) NOT NULL,
            Description    nvarchar(500) NULL,
            IsSystemGroup  bit NOT NULL CONSTRAINT DF_auth_SecurityGroup_System DEFAULT (0),
            IsActive       bit NOT NULL CONSTRAINT DF_auth_SecurityGroup_Active DEFAULT (1),
            CreatedAt      datetime2(0) NOT NULL CONSTRAINT DF_auth_SecurityGroup_CreatedAt DEFAULT (SYSUTCDATETIME()),
            UpdatedAt      datetime2(0) NULL,
            RowVersion     rowversion NOT NULL,
            CONSTRAINT PK_auth_SecurityGroup PRIMARY KEY CLUSTERED (GroupId),
            CONSTRAINT UQ_auth_SecurityGroup_Code UNIQUE (GroupCode)
        );
    END;

    IF OBJECT_ID(N'dbo.auth_Role', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.auth_Role
        (
            RoleId        int IDENTITY(1,1) NOT NULL,
            RoleCode      varchar(80) NOT NULL,
            RoleName      nvarchar(200) NOT NULL,
            Description   nvarchar(500) NULL,
            IsSystemRole  bit NOT NULL CONSTRAINT DF_auth_Role_System DEFAULT (0),
            IsActive      bit NOT NULL CONSTRAINT DF_auth_Role_Active DEFAULT (1),
            CreatedAt     datetime2(0) NOT NULL CONSTRAINT DF_auth_Role_CreatedAt DEFAULT (SYSUTCDATETIME()),
            UpdatedAt     datetime2(0) NULL,
            RowVersion    rowversion NOT NULL,
            CONSTRAINT PK_auth_Role PRIMARY KEY CLUSTERED (RoleId),
            CONSTRAINT UQ_auth_Role_Code UNIQUE (RoleCode)
        );
    END;

    IF OBJECT_ID(N'dbo.app_Function', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.app_Function
        (
            FunctionId        int IDENTITY(1,1) NOT NULL,
            ParentFunctionId  int NULL,
            FunctionCode      varchar(100) NOT NULL,
            DisplayName       nvarchar(200) NOT NULL,
            NavigationTarget  varchar(300) NULL,
            IconName          varchar(100) NULL,
            SortOrder         int NOT NULL CONSTRAINT DF_app_Function_Sort DEFAULT (0),
            IsVisible         bit NOT NULL CONSTRAINT DF_app_Function_Visible DEFAULT (1),
            IsActive          bit NOT NULL CONSTRAINT DF_app_Function_Active DEFAULT (1),
            CreatedAt         datetime2(0) NOT NULL CONSTRAINT DF_app_Function_CreatedAt DEFAULT (SYSUTCDATETIME()),
            UpdatedAt         datetime2(0) NULL,
            RowVersion        rowversion NOT NULL,
            CONSTRAINT PK_app_Function PRIMARY KEY CLUSTERED (FunctionId),
            CONSTRAINT UQ_app_Function_Code UNIQUE (FunctionCode),
            CONSTRAINT FK_app_Function_Parent FOREIGN KEY (ParentFunctionId)
                REFERENCES dbo.app_Function(FunctionId),
            CONSTRAINT CK_app_Function_NotSelf CHECK (ParentFunctionId IS NULL OR ParentFunctionId <> FunctionId)
        );

        CREATE INDEX IX_app_Function_Parent_Active
            ON dbo.app_Function(ParentFunctionId, IsActive, IsVisible, SortOrder);
    END;

    IF OBJECT_ID(N'dbo.auth_Permission', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.auth_Permission
        (
            PermissionId    int IDENTITY(1,1) NOT NULL,
            FunctionId      int NOT NULL,
            ActionCode      varchar(30) NOT NULL,
            PermissionCode  varchar(150) NOT NULL,
            DisplayName     nvarchar(200) NOT NULL,
            Description     nvarchar(500) NULL,
            IsActive        bit NOT NULL CONSTRAINT DF_auth_Permission_Active DEFAULT (1),
            CONSTRAINT PK_auth_Permission PRIMARY KEY CLUSTERED (PermissionId),
            CONSTRAINT UQ_auth_Permission_Code UNIQUE (PermissionCode),
            CONSTRAINT UQ_auth_Permission_FunctionAction UNIQUE (FunctionId, ActionCode),
            CONSTRAINT FK_auth_Permission_Function FOREIGN KEY (FunctionId)
                REFERENCES dbo.app_Function(FunctionId),
            CONSTRAINT CK_auth_Permission_Action CHECK (ActionCode IN ('ACCESS', 'VIEW', 'CREATE', 'UPDATE', 'DELETE', 'APPROVE', 'EXPORT', 'ADMIN'))
        );
    END;

    IF OBJECT_ID(N'dbo.auth_UserGroup', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.auth_UserGroup
        (
            UserId            bigint NOT NULL,
            GroupId           int NOT NULL,
            AssignedAt        datetime2(0) NOT NULL CONSTRAINT DF_auth_UserGroup_AssignedAt DEFAULT (SYSUTCDATETIME()),
            AssignedByUserId  bigint NULL,
            ExpiresAt         datetime2(0) NULL,
            IsActive          bit NOT NULL CONSTRAINT DF_auth_UserGroup_Active DEFAULT (1),
            CONSTRAINT PK_auth_UserGroup PRIMARY KEY CLUSTERED (UserId, GroupId),
            CONSTRAINT FK_auth_UserGroup_User FOREIGN KEY (UserId)
                REFERENCES dbo.auth_UserAccount(UserId),
            CONSTRAINT FK_auth_UserGroup_Group FOREIGN KEY (GroupId)
                REFERENCES dbo.auth_SecurityGroup(GroupId),
            CONSTRAINT FK_auth_UserGroup_AssignedBy FOREIGN KEY (AssignedByUserId)
                REFERENCES dbo.auth_UserAccount(UserId),
            CONSTRAINT CK_auth_UserGroup_Expiry CHECK (ExpiresAt IS NULL OR ExpiresAt > AssignedAt)
        );

        CREATE INDEX IX_auth_UserGroup_Group_Active
            ON dbo.auth_UserGroup(GroupId, IsActive, ExpiresAt)
            INCLUDE (UserId);
    END;

    IF OBJECT_ID(N'dbo.auth_GroupRole', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.auth_GroupRole
        (
            GroupId          int NOT NULL,
            RoleId           int NOT NULL,
            AssignedAt       datetime2(0) NOT NULL CONSTRAINT DF_auth_GroupRole_AssignedAt DEFAULT (SYSUTCDATETIME()),
            AssignedByUserId bigint NULL,
            IsActive         bit NOT NULL CONSTRAINT DF_auth_GroupRole_Active DEFAULT (1),
            CONSTRAINT PK_auth_GroupRole PRIMARY KEY CLUSTERED (GroupId, RoleId),
            CONSTRAINT FK_auth_GroupRole_Group FOREIGN KEY (GroupId)
                REFERENCES dbo.auth_SecurityGroup(GroupId),
            CONSTRAINT FK_auth_GroupRole_Role FOREIGN KEY (RoleId)
                REFERENCES dbo.auth_Role(RoleId),
            CONSTRAINT FK_auth_GroupRole_AssignedBy FOREIGN KEY (AssignedByUserId)
                REFERENCES dbo.auth_UserAccount(UserId)
        );

        CREATE INDEX IX_auth_GroupRole_Role_Active
            ON dbo.auth_GroupRole(RoleId, IsActive)
            INCLUDE (GroupId);
    END;

    IF OBJECT_ID(N'dbo.auth_RolePermission', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.auth_RolePermission
        (
            RoleId           int NOT NULL,
            PermissionId     int NOT NULL,
            AssignedAt       datetime2(0) NOT NULL CONSTRAINT DF_auth_RolePermission_AssignedAt DEFAULT (SYSUTCDATETIME()),
            AssignedByUserId bigint NULL,
            IsActive         bit NOT NULL CONSTRAINT DF_auth_RolePermission_Active DEFAULT (1),
            CONSTRAINT PK_auth_RolePermission PRIMARY KEY CLUSTERED (RoleId, PermissionId),
            CONSTRAINT FK_auth_RolePermission_Role FOREIGN KEY (RoleId)
                REFERENCES dbo.auth_Role(RoleId),
            CONSTRAINT FK_auth_RolePermission_Permission FOREIGN KEY (PermissionId)
                REFERENCES dbo.auth_Permission(PermissionId),
            CONSTRAINT FK_auth_RolePermission_AssignedBy FOREIGN KEY (AssignedByUserId)
                REFERENCES dbo.auth_UserAccount(UserId)
        );

        CREATE INDEX IX_auth_RolePermission_Permission_Active
            ON dbo.auth_RolePermission(PermissionId, IsActive)
            INCLUDE (RoleId);
    END;

    IF OBJECT_ID(N'dbo.sys_Setting', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.sys_Setting
        (
            SettingKey      varchar(120) NOT NULL,
            SettingValue    nvarchar(max) NULL,
            ValueType       varchar(30) NOT NULL CONSTRAINT DF_sys_Setting_Type DEFAULT ('STRING'),
            SettingGroup    varchar(80) NOT NULL CONSTRAINT DF_sys_Setting_Group DEFAULT ('GENERAL'),
            Description     nvarchar(500) NULL,
            IsSensitive     bit NOT NULL CONSTRAINT DF_sys_Setting_Sensitive DEFAULT (0),
            UpdatedAt       datetime2(0) NOT NULL CONSTRAINT DF_sys_Setting_UpdatedAt DEFAULT (SYSUTCDATETIME()),
            UpdatedByUserId bigint NULL,
            RowVersion      rowversion NOT NULL,
            CONSTRAINT PK_sys_Setting PRIMARY KEY CLUSTERED (SettingKey),
            CONSTRAINT FK_sys_Setting_UpdatedBy FOREIGN KEY (UpdatedByUserId)
                REFERENCES dbo.auth_UserAccount(UserId),
            CONSTRAINT CK_sys_Setting_Type CHECK (ValueType IN ('STRING', 'INT', 'DECIMAL', 'BOOL', 'DATE', 'JSON', 'PATH', 'URL'))
        );
    END;

    IF OBJECT_ID(N'dbo.audit_AuditLog', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.audit_AuditLog
        (
            AuditLogId    bigint IDENTITY(1,1) NOT NULL,
            CreatedAt     datetime2(3) NOT NULL CONSTRAINT DF_audit_AuditLog_CreatedAt DEFAULT (SYSUTCDATETIME()),
            UserId        bigint NULL,
            LoginName     varchar(100) NULL,
            ActionCode    varchar(100) NOT NULL,
            EntityName    varchar(128) NULL,
            EntityId      varchar(100) NULL,
            Description   nvarchar(1000) NULL,
            MachineName   varchar(100) NULL,
            IpAddress     varchar(50) NULL,
            CorrelationId uniqueidentifier NOT NULL CONSTRAINT DF_audit_AuditLog_Correlation DEFAULT (NEWID()),
            DataJson      nvarchar(max) NULL,
            CONSTRAINT PK_audit_AuditLog PRIMARY KEY CLUSTERED (AuditLogId),
            CONSTRAINT FK_audit_AuditLog_User FOREIGN KEY (UserId)
                REFERENCES dbo.auth_UserAccount(UserId)
        );

        CREATE INDEX IX_audit_AuditLog_CreatedAt
            ON dbo.audit_AuditLog(CreatedAt DESC);

        CREATE INDEX IX_audit_AuditLog_User_CreatedAt
            ON dbo.audit_AuditLog(UserId, CreatedAt DESC)
            INCLUDE (ActionCode, EntityName, EntityId);
    END;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO

IF OBJECT_ID(N'dbo.vw_auth_UserEffectivePermission', N'V') IS NOT NULL
    DROP VIEW dbo.vw_auth_UserEffectivePermission;
GO

CREATE VIEW dbo.vw_auth_UserEffectivePermission
AS
    SELECT DISTINCT
        u.UserId,
        u.LoginName,
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
    INNER JOIN dbo.auth_UserGroup AS ug
        ON ug.UserId = u.UserId
       AND ug.IsActive = 1
       AND (ug.ExpiresAt IS NULL OR ug.ExpiresAt > SYSUTCDATETIME())
    INNER JOIN dbo.auth_SecurityGroup AS g
        ON g.GroupId = ug.GroupId
       AND g.IsActive = 1
    INNER JOIN dbo.auth_GroupRole AS gr
        ON gr.GroupId = g.GroupId
       AND gr.IsActive = 1
    INNER JOIN dbo.auth_Role AS r
        ON r.RoleId = gr.RoleId
       AND r.IsActive = 1
    INNER JOIN dbo.auth_RolePermission AS rp
        ON rp.RoleId = r.RoleId
       AND rp.IsActive = 1
    INNER JOIN dbo.auth_Permission AS p
        ON p.PermissionId = rp.PermissionId
       AND p.IsActive = 1
    INNER JOIN dbo.app_Function AS f
        ON f.FunctionId = p.FunctionId
       AND f.IsActive = 1
    WHERE u.IsActive = 1;
GO

IF OBJECT_ID(N'dbo.usp_auth_UserHasPermission', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_auth_UserHasPermission;
GO

CREATE PROCEDURE dbo.usp_auth_UserHasPermission
    @LoginName varchar(100),
    @PermissionCode varchar(150)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CAST
    (
        CASE WHEN EXISTS
        (
            SELECT 1
            FROM dbo.vw_auth_UserEffectivePermission
            WHERE LoginName = @LoginName
              AND PermissionCode = @PermissionCode
        ) THEN 1 ELSE 0 END
        AS bit
    ) AS IsGranted;
END;
GO

IF OBJECT_ID(N'dbo.usp_auth_AddUserToGroup', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_auth_AddUserToGroup;
GO

CREATE PROCEDURE dbo.usp_auth_AddUserToGroup
    @UserId bigint,
    @GroupCode varchar(80),
    @AssignedByUserId bigint = NULL,
    @ExpiresAt datetime2(0) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @GroupId int =
    (
        SELECT GroupId
        FROM dbo.auth_SecurityGroup
        WHERE GroupCode = @GroupCode AND IsActive = 1
    );

    IF @GroupId IS NULL
        THROW 51001, 'Security group does not exist or is inactive.', 1;

    IF NOT EXISTS (SELECT 1 FROM dbo.auth_UserAccount WHERE UserId = @UserId AND IsActive = 1)
        THROW 51002, 'User does not exist or is inactive.', 1;

    UPDATE dbo.auth_UserGroup
       SET IsActive = 1,
           AssignedAt = SYSUTCDATETIME(),
           AssignedByUserId = @AssignedByUserId,
           ExpiresAt = @ExpiresAt
     WHERE UserId = @UserId AND GroupId = @GroupId;

    IF @@ROWCOUNT = 0
    BEGIN
        INSERT dbo.auth_UserGroup(UserId, GroupId, AssignedByUserId, ExpiresAt)
        VALUES (@UserId, @GroupId, @AssignedByUserId, @ExpiresAt);
    END;

    INSERT dbo.audit_AuditLog
        (UserId, ActionCode, EntityName, EntityId, Description)
    VALUES
        (@AssignedByUserId, 'SECURITY.USER_GROUP.ADD', 'auth_UserGroup',
         CONCAT(@UserId, ':', @GroupId), CONCAT('Added user to group ', @GroupCode));
END;
GO

IF OBJECT_ID(N'dbo.usp_auth_RemoveUserFromGroup', N'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_auth_RemoveUserFromGroup;
GO

CREATE PROCEDURE dbo.usp_auth_RemoveUserFromGroup
    @UserId bigint,
    @GroupCode varchar(80),
    @RemovedByUserId bigint = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @GroupId int =
    (
        SELECT GroupId
        FROM dbo.auth_SecurityGroup
        WHERE GroupCode = @GroupCode
    );

    IF @GroupId IS NULL
        THROW 51003, 'Security group does not exist.', 1;

    UPDATE dbo.auth_UserGroup
       SET IsActive = 0
     WHERE UserId = @UserId
       AND GroupId = @GroupId
       AND IsActive = 1;

    IF @@ROWCOUNT = 0
        THROW 51004, 'Active user-group membership does not exist.', 1;

    INSERT dbo.audit_AuditLog
        (UserId, ActionCode, EntityName, EntityId, Description)
    VALUES
        (@RemovedByUserId, 'SECURITY.USER_GROUP.REMOVE', 'auth_UserGroup',
         CONCAT(@UserId, ':', @GroupId), CONCAT('Removed user from group ', @GroupCode));
END;
GO
