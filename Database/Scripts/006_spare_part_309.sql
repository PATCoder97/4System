SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID(N'dbo.dt309_Units', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.dt309_Units
        (
            Id int NOT NULL CONSTRAINT PK_dt309_Units PRIMARY KEY,
            DisplayName nvarchar(255) NOT NULL
        );
    END;

    IF OBJECT_ID(N'dbo.dt309_Storages', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.dt309_Storages
        (
            Id int NOT NULL CONSTRAINT PK_dt309_Storages PRIMARY KEY,
            DisplayName nvarchar(255) NOT NULL
        );
    END;

    IF OBJECT_ID(N'dbo.dt309_Materials', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.dt309_Materials
        (
            Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_dt309_Materials PRIMARY KEY,
            IdDept varchar(4) NOT NULL,
            Code varchar(64) NULL,
            DisplayName nvarchar(2048) NOT NULL,
            TypeUse nvarchar(32) NULL,
            Location nvarchar(32) NOT NULL,
            IdUnit int NOT NULL,
            MinQuantity float NOT NULL CONSTRAINT DF_dt309_Materials_MinQuantity DEFAULT (0),
            QuantityInStorage float NOT NULL CONSTRAINT DF_dt309_Materials_Storage DEFAULT (0),
            QuantityInMachine float NOT NULL CONSTRAINT DF_dt309_Materials_Machine DEFAULT (0),
            Price int NOT NULL CONSTRAINT DF_dt309_Materials_Price DEFAULT (0),
            ExpDate date NULL,
            IdManager varchar(10) NOT NULL,
            DelTime datetime NULL,
            UserDel varchar(10) NULL,
            IsDisable bit NULL,
            DisabledBy varchar(10) NULL,
            DisabledDate datetime NULL,
            EnabledBy varchar(10) NULL,
            EnabledDate datetime NULL,
            ReplacementMaterialId int NULL,
            ReplacementDate datetime NULL,
            BaseMaterialId int NULL,
            IsRecoveredOld bit NOT NULL CONSTRAINT DF_dt309_Materials_IsRecoveredOld DEFAULT (0),
            CONSTRAINT FK_dt309_Materials_Unit FOREIGN KEY (IdUnit) REFERENCES dbo.dt309_Units(Id),
            CONSTRAINT FK_dt309_Materials_Replacement FOREIGN KEY (ReplacementMaterialId) REFERENCES dbo.dt309_Materials(Id),
            CONSTRAINT FK_dt309_Materials_Base FOREIGN KEY (BaseMaterialId) REFERENCES dbo.dt309_Materials(Id)
        );
        CREATE INDEX IX_dt309_Materials_DeptActive ON dbo.dt309_Materials(IdDept, DelTime, IsDisable);
        CREATE UNIQUE INDEX UX_dt309_Materials_ReplacementMaterialId_Active
            ON dbo.dt309_Materials(ReplacementMaterialId)
            WHERE ReplacementMaterialId IS NOT NULL AND DelTime IS NULL;
    END;

    IF OBJECT_ID(N'dbo.dt309_Machines', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.dt309_Machines
        (
            Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_dt309_Machines PRIMARY KEY,
            IdDept varchar(4) NOT NULL,
            DisplayName nvarchar(512) NOT NULL,
            Location nvarchar(32) NOT NULL,
            Quantity int NOT NULL CONSTRAINT DF_dt309_Machines_Quantity DEFAULT (1),
            ImpLevel varchar(8) NULL
        );
        CREATE INDEX IX_dt309_Machines_Dept ON dbo.dt309_Machines(IdDept);
    END;

    IF OBJECT_ID(N'dbo.dt309_MachineMaterials', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.dt309_MachineMaterials
        (
            Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_dt309_MachineMaterials PRIMARY KEY,
            MachineId int NOT NULL,
            MaterialId int NOT NULL,
            CONSTRAINT FK_dt309_MachineMaterials_Machine FOREIGN KEY (MachineId) REFERENCES dbo.dt309_Machines(Id),
            CONSTRAINT FK_dt309_MachineMaterials_Material FOREIGN KEY (MaterialId) REFERENCES dbo.dt309_Materials(Id),
            CONSTRAINT UQ_dt309_MachineMaterials UNIQUE (MachineId, MaterialId)
        );
    END;

    IF OBJECT_ID(N'dbo.dt309_Transactions', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.dt309_Transactions
        (
            Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_dt309_Transactions PRIMARY KEY,
            StorageId int NOT NULL,
            MaterialId int NOT NULL,
            Quantity float NOT NULL,
            TransactionType nvarchar(10) NULL,
            CreatedDate datetime NULL CONSTRAINT DF_dt309_Transactions_CreatedDate DEFAULT (GETDATE()),
            AftQuantity float NULL,
            TotalQuantity float NULL,
            UserDo varchar(10) NOT NULL CONSTRAINT DF_dt309_Transactions_UserDo DEFAULT (''),
            [Desc] nvarchar(512) NULL,
            NotifyDate datetime NULL,
            CONSTRAINT FK_dt309_Transactions_Storage FOREIGN KEY (StorageId) REFERENCES dbo.dt309_Storages(Id),
            CONSTRAINT FK_dt309_Transactions_Material FOREIGN KEY (MaterialId) REFERENCES dbo.dt309_Materials(Id)
        );
        CREATE INDEX IX_dt309_Transactions_MaterialDate ON dbo.dt309_Transactions(MaterialId, CreatedDate);
    END;

    IF OBJECT_ID(N'dbo.dt309_Prices', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.dt309_Prices
        (
            Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_dt309_Prices PRIMARY KEY,
            MaterialId int NOT NULL,
            Price int NOT NULL,
            ChangedBy varchar(10) NOT NULL,
            ChangedAt datetime NOT NULL CONSTRAINT DF_dt309_Prices_ChangedAt DEFAULT (GETDATE()),
            CONSTRAINT FK_dt309_Prices_Material FOREIGN KEY (MaterialId) REFERENCES dbo.dt309_Materials(Id)
        );
    END;

    IF OBJECT_ID(N'dbo.dt309_MaterialPhoto', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.dt309_MaterialPhoto
        (
            Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_dt309_MaterialPhoto PRIMARY KEY,
            MaterialId int NOT NULL,
            EncryptionName varchar(64) NOT NULL,
            ActualName nvarchar(256) NOT NULL,
            UploadedBy varchar(10) NULL,
            UploadedDate datetime NOT NULL CONSTRAINT DF_dt309_MaterialPhoto_UploadedDate DEFAULT (GETDATE()),
            IsActive bit NOT NULL CONSTRAINT DF_dt309_MaterialPhoto_IsActive DEFAULT (1),
            CONSTRAINT FK_dt309_MaterialPhoto_Material FOREIGN KEY (MaterialId) REFERENCES dbo.dt309_Materials(Id)
        );
        CREATE INDEX IX_dt309_MaterialPhoto_Material ON dbo.dt309_MaterialPhoto(MaterialId, IsActive);
    END;

    IF OBJECT_ID(N'dbo.dt309_InspectionBatch', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.dt309_InspectionBatch
        (
            Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_dt309_InspectionBatch PRIMARY KEY,
            BatchName nvarchar(256) NOT NULL,
            CreatedDate datetime NOT NULL CONSTRAINT DF_dt309_InspectionBatch_CreatedDate DEFAULT (GETDATE()),
            ExpiryDate datetime NULL,
            NotifyNo int NULL,
            ReportDate date NULL,
            IsCancelled bit NOT NULL CONSTRAINT DF_dt309_InspectionBatch_IsCancelled DEFAULT (0),
            CancelledBy varchar(10) NULL,
            CancelledDate datetime NULL,
            CancelReason nvarchar(500) NULL
        );
    END;

    IF OBJECT_ID(N'dbo.dt309_InspectionBatchMaterial', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.dt309_InspectionBatchMaterial
        (
            Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_dt309_InspectionBatchMaterial PRIMARY KEY,
            BatchId int NOT NULL,
            MaterialId int NOT NULL,
            InitialQuantity float NOT NULL,
            ActualQuantity float NULL,
            ConfirmationDate datetime NULL,
            ConfirmedBy varchar(10) NULL,
            IsComplete bit NULL,
            Description nvarchar(512) NULL,
            CONSTRAINT FK_dt309_InspectionBatchMaterial_Batch FOREIGN KEY (BatchId) REFERENCES dbo.dt309_InspectionBatch(Id),
            CONSTRAINT FK_dt309_InspectionBatchMaterial_Material FOREIGN KEY (MaterialId) REFERENCES dbo.dt309_Materials(Id),
            CONSTRAINT UQ_dt309_InspectionBatchMaterial UNIQUE (BatchId, MaterialId)
        );
    END;

    IF OBJECT_ID(N'dbo.dt309_RecoveryTickets', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.dt309_RecoveryTickets
        (
            Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_dt309_RecoveryTickets PRIMARY KEY,
            TicketNo nvarchar(40) NOT NULL,
            IssueTransactionId int NOT NULL,
            RestockInTransactionId int NULL,
            NewMaterialId int NOT NULL,
            OldBaseMaterialId int NOT NULL,
            OldRecoveryMaterialId int NULL,
            RecoveryOption varchar(20) NOT NULL,
            Quantity float NOT NULL,
            SourceStorageId int NOT NULL,
            RestockStorageId int NULL,
            AssignedUserId varchar(10) NULL,
            PlannedDisposeDate datetime NULL,
            ActualDisposeDate datetime NULL,
            Status varchar(40) NOT NULL,
            Description nvarchar(1000) NULL,
            ResultNote nvarchar(1000) NULL,
            EvidenceSubmittedDate datetime NULL,
            CompletedBy varchar(10) NULL,
            CompletedDate datetime NULL,
            CancelledBy varchar(10) NULL,
            CancelledDate datetime NULL,
            CancelReason nvarchar(500) NULL,
            CreatedBy varchar(10) NOT NULL,
            CreatedDate datetime NOT NULL CONSTRAINT DF_dt309_RecoveryTickets_CreatedDate DEFAULT (GETDATE()),
            UpdatedBy varchar(10) NULL,
            UpdatedDate datetime NULL,
            CONSTRAINT UQ_dt309_RecoveryTickets_TicketNo UNIQUE (TicketNo),
            CONSTRAINT FK_dt309_RecoveryTickets_IssueTransaction FOREIGN KEY (IssueTransactionId) REFERENCES dbo.dt309_Transactions(Id),
            CONSTRAINT FK_dt309_RecoveryTickets_RestockTransaction FOREIGN KEY (RestockInTransactionId) REFERENCES dbo.dt309_Transactions(Id),
            CONSTRAINT FK_dt309_RecoveryTickets_NewMaterial FOREIGN KEY (NewMaterialId) REFERENCES dbo.dt309_Materials(Id),
            CONSTRAINT FK_dt309_RecoveryTickets_OldBaseMaterial FOREIGN KEY (OldBaseMaterialId) REFERENCES dbo.dt309_Materials(Id),
            CONSTRAINT FK_dt309_RecoveryTickets_OldRecoveryMaterial FOREIGN KEY (OldRecoveryMaterialId) REFERENCES dbo.dt309_Materials(Id),
            CONSTRAINT FK_dt309_RecoveryTickets_SourceStorage FOREIGN KEY (SourceStorageId) REFERENCES dbo.dt309_Storages(Id),
            CONSTRAINT FK_dt309_RecoveryTickets_RestockStorage FOREIGN KEY (RestockStorageId) REFERENCES dbo.dt309_Storages(Id)
        );
        CREATE INDEX IX_dt309_RecoveryTickets_StatusUser ON dbo.dt309_RecoveryTickets(Status, AssignedUserId, CreatedDate);
    END;

    IF OBJECT_ID(N'dbo.dt309_RecoveryEvidence', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.dt309_RecoveryEvidence
        (
            Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_dt309_RecoveryEvidence PRIMARY KEY,
            RecoveryTicketId int NOT NULL,
            ActualName nvarchar(256) NOT NULL,
            EncryptionName varchar(64) NOT NULL,
            FileExt varchar(16) NULL,
            UploadedBy varchar(10) NULL,
            UploadedDate datetime NOT NULL CONSTRAINT DF_dt309_RecoveryEvidence_UploadedDate DEFAULT (GETDATE()),
            IsActive bit NOT NULL CONSTRAINT DF_dt309_RecoveryEvidence_IsActive DEFAULT (1),
            CONSTRAINT FK_dt309_RecoveryEvidence_Ticket FOREIGN KEY (RecoveryTicketId) REFERENCES dbo.dt309_RecoveryTickets(Id)
        );
    END;

    IF OBJECT_ID(N'dbo.dt309_RecoveryGuides', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.dt309_RecoveryGuides
        (
            Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_dt309_RecoveryGuides PRIMARY KEY,
            Title nvarchar(256) NOT NULL,
            ActualName nvarchar(256) NOT NULL,
            EncryptionName varchar(64) NOT NULL,
            FileExt varchar(16) NULL,
            DisplayOrder int NOT NULL CONSTRAINT DF_dt309_RecoveryGuides_DisplayOrder DEFAULT (1),
            IsActive bit NOT NULL CONSTRAINT DF_dt309_RecoveryGuides_IsActive DEFAULT (1),
            UploadedBy varchar(10) NULL,
            UploadedDate datetime NOT NULL CONSTRAINT DF_dt309_RecoveryGuides_UploadedDate DEFAULT (GETDATE())
        );
    END;

    IF OBJECT_ID(N'dbo.dt309_Attachment', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.dt309_Attachment
        (
            Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_dt309_Attachment PRIMARY KEY,
            Thread varchar(16) NOT NULL,
            EncryptionName varchar(64) NOT NULL,
            ActualName nvarchar(256) NOT NULL
        );
        CREATE INDEX IX_dt309_Attachment_Thread ON dbo.dt309_Attachment(Thread);
    END;

    MERGE dbo.dt309_Units AS target
    USING (VALUES
        (1,N'PR'),(2,N'CA'),(3,N'L'),(4,N'M'),(5,N'PC'),(6,N'KG'),(7,N'SH'),
        (8,N'ST'),(9,N'BX'),(10,N'FT'),(11,N'GR'),(12,N'TK'),(13,N'RL'),(14,N'BC')
    ) AS source(Id, DisplayName)
    ON target.Id = source.Id
    WHEN NOT MATCHED THEN INSERT (Id, DisplayName) VALUES (source.Id, source.DisplayName);

    MERGE dbo.dt309_Storages AS target
    USING (VALUES (1,N'機邊庫'),(2,N'課庫')) AS source(Id, DisplayName)
    ON target.Id = source.Id
    WHEN MATCHED THEN UPDATE SET DisplayName = source.DisplayName
    WHEN NOT MATCHED THEN INSERT (Id, DisplayName) VALUES (source.Id, source.DisplayName);

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO

IF OBJECT_ID(N'dbo.vw_309_User', N'V') IS NOT NULL
    DROP VIEW dbo.vw_309_User;
GO
CREATE VIEW dbo.vw_309_User
AS
SELECT
    u.UserId AS Id,
    COALESCE(NULLIF(e.DisplayNameTW, N''), NULLIF(e.DisplayNameVN, N''), u.UserId) AS DisplayName,
    CAST(NULL AS varchar(256)) AS SecondaryPassword,
    u.CreatedAt AS DateCreate,
    COALESCE(d.DepartmentCode, '') AS IdDepartment,
    e.DisplayNameVN,
    CAST(NULL AS date) AS DOB,
    CAST(NULL AS varchar(30)) AS CitizenID,
    CAST(NULL AS nvarchar(100)) AS Nationality,
    CAST(NULL AS varchar(30)) AS JobCode,
    CAST(NULL AS nvarchar(100)) AS PCName,
    CAST(NULL AS varchar(45)) AS IPAddress,
    CAST(NULL AS nvarchar(300)) AS Addr,
    CAST(NULL AS varchar(30)) AS PhoneNum1,
    CAST(NULL AS varchar(30)) AS PhoneNum2,
    CAST(CASE WHEN u.IsActive = 1 THEN 0 ELSE 1 END AS int) AS Status,
    CAST(NULL AS bit) AS Sex,
    CAST(NULL AS varchar(30)) AS ActualJobCode,
    u.UpdatedAt AS LastUpdate,
    CAST(NULL AS date) AS ResignPlan,
    CAST(NULL AS nvarchar(100)) AS RecognizedEducation,
    CAST(NULL AS nvarchar(200)) AS EducationName,
    CAST(NULL AS date) AS ResignDate,
    CAST(NULL AS date) AS JobEffectiveDate
FROM dbo.auth_UserAccount AS u
LEFT JOIN dbo.hr_EmployeeProfile AS e ON e.EmployeeProfileId = u.EmployeeProfileId
LEFT JOIN dbo.dm_Department AS d ON d.DepartmentId = e.DepartmentId;
GO

IF OBJECT_ID(N'dbo.vw_309_Department', N'V') IS NOT NULL
    DROP VIEW dbo.vw_309_Department;
GO
CREATE VIEW dbo.vw_309_Department
AS
SELECT
    d.DepartmentCode AS Id,
    d.DepartmentId AS IdChild,
    CAST(NULL AS int) AS IdParent,
    d.DepartmentName AS DisplayName,
    d.DepartmentName AS DisplayNameVN,
    CAST(0 AS bit) AS IsGroup,
    CAST(NULL AS int) AS AuthorizedHeadcount,
    CAST(1 AS bit) AS IsActive
FROM dbo.dm_Department AS d;
GO

IF OBJECT_ID(N'dbo.vw_309_Group', N'V') IS NOT NULL
    DROP VIEW dbo.vw_309_Group;
GO
CREATE VIEW dbo.vw_309_Group
AS
SELECT
    g.GroupId AS Id,
    g.GroupName AS DisplayName,
    g.Description AS Describe,
    g.GroupId AS Prioritize,
    CAST(NULL AS varchar(30)) AS IdDept
FROM dbo.auth_SecurityGroup AS g
WHERE g.IsActive = 1;
GO

IF OBJECT_ID(N'dbo.vw_309_GroupUser', N'V') IS NOT NULL
    DROP VIEW dbo.vw_309_GroupUser;
GO
CREATE VIEW dbo.vw_309_GroupUser
AS
SELECT
    CAST(ROW_NUMBER() OVER (ORDER BY ug.GroupId, ug.UserId) AS int) AS Id,
    ug.GroupId AS IdGroup,
    ug.UserId AS IdUser
FROM dbo.auth_UserGroup AS ug
WHERE ug.IsActive = 1
  AND (ug.ExpiresAt IS NULL OR ug.ExpiresAt > SYSUTCDATETIME());
GO

BEGIN TRY
    BEGIN TRANSACTION;

    MERGE dbo.app_Function AS target
    USING (SELECT
        parentData.FunctionId AS ParentFunctionId,
        'ASSET.SPARE_PART' AS FunctionCode,
        N'309 備品備件管理' AS DisplayName,
        N'配件、設備、進出庫、盤點、成本及回收管理' AS Description,
        'SparePartForm' AS NavigationTarget,
        20 AS SortOrder,
        CAST(1 AS bit) AS IsVisible,
        CAST(1 AS bit) AS IsActive,
        'COMPLETED' AS DevelopmentStatus,
        CAST(1 AS bit) AS IsWide
        FROM dbo.app_Function AS parentData
        WHERE parentData.FunctionCode = 'ASSET') AS source
    ON target.FunctionCode = source.FunctionCode
    WHEN MATCHED THEN UPDATE SET
        ParentFunctionId = source.ParentFunctionId,
        DisplayName = source.DisplayName,
        Description = source.Description,
        NavigationTarget = source.NavigationTarget,
        SortOrder = source.SortOrder,
        IsVisible = source.IsVisible,
        IsActive = source.IsActive,
        DevelopmentStatus = source.DevelopmentStatus,
        IsWide = source.IsWide,
        UpdatedAt = SYSUTCDATETIME()
    WHEN NOT MATCHED THEN INSERT
        (ParentFunctionId, FunctionCode, DisplayName, Description, NavigationTarget,
         SortOrder, IsVisible, IsActive, DevelopmentStatus, IsWide)
        VALUES
        (source.ParentFunctionId, source.FunctionCode, source.DisplayName, source.Description,
         source.NavigationTarget, source.SortOrder, source.IsVisible, source.IsActive,
         source.DevelopmentStatus, source.IsWide);

    MERGE dbo.auth_Permission AS target
    USING
    (
        SELECT f.FunctionId, p.ActionCode, p.PermissionCode, p.DisplayName
        FROM dbo.app_Function AS f
        CROSS JOIN (VALUES
            ('VIEW',    'ASSET.SPARE_PART.VIEW',    N'檢視備品備件'),
            ('CREATE',  'ASSET.SPARE_PART.CREATE',  N'新增備品備件資料'),
            ('UPDATE',  'ASSET.SPARE_PART.UPDATE',  N'修改備品備件資料'),
            ('DELETE',  'ASSET.SPARE_PART.DELETE',  N'刪除備品備件資料'),
            ('APPROVE', 'ASSET.SPARE_PART.APPROVE', N'核准盤點與回收作業'),
            ('EXPORT',  'ASSET.SPARE_PART.EXPORT',  N'匯出備品備件報表'),
            ('ADMIN',   'ASSET.SPARE_PART.ADMIN',   N'管理備品備件功能')
        ) AS p(ActionCode, PermissionCode, DisplayName)
        WHERE f.FunctionCode = 'ASSET.SPARE_PART'
    ) AS source
    ON target.PermissionCode = source.PermissionCode
    WHEN MATCHED THEN UPDATE SET
        target.FunctionId = source.FunctionId,
        target.ActionCode = source.ActionCode,
        target.DisplayName = source.DisplayName,
        target.IsActive = 1
    WHEN NOT MATCHED THEN INSERT (FunctionId, ActionCode, PermissionCode, DisplayName, IsActive)
        VALUES (source.FunctionId, source.ActionCode, source.PermissionCode, source.DisplayName, 1);

    MERGE dbo.auth_Role AS target
    USING (VALUES
        ('SPARE_PART_VIEWER',  N'備品備件查閱者', N'可檢視及匯出 309 資料', 0),
        ('SPARE_PART_OPERATOR',N'備品備件作業員', N'可執行 309 日常新增與修改作業', 0),
        ('SPARE_PART_MANAGER', N'備品備件管理員', N'可管理 309 全部作業', 0)
    ) AS source(RoleCode, RoleName, Description, IsSystemRole)
    ON target.RoleCode = source.RoleCode
    WHEN MATCHED THEN UPDATE SET
        RoleName = source.RoleName,
        Description = source.Description,
        IsActive = 1
    WHEN NOT MATCHED THEN INSERT (RoleCode, RoleName, Description, IsSystemRole, IsActive)
        VALUES (source.RoleCode, source.RoleName, source.Description, source.IsSystemRole, 1);

    MERGE dbo.auth_RolePermission AS target
    USING
    (
        SELECT r.RoleId, p.PermissionId
        FROM dbo.auth_Role AS r
        CROSS JOIN dbo.auth_Permission AS p
        WHERE p.PermissionCode LIKE 'ASSET.SPARE_PART.%'
          AND (
              (r.RoleCode = 'SPARE_PART_VIEWER' AND p.ActionCode IN ('VIEW','EXPORT')) OR
              (r.RoleCode = 'SPARE_PART_OPERATOR' AND p.ActionCode IN ('VIEW','CREATE','UPDATE','EXPORT')) OR
              (r.RoleCode IN ('SPARE_PART_MANAGER','SECURITY_ADMIN'))
          )
    ) AS source
    ON target.RoleId = source.RoleId AND target.PermissionId = source.PermissionId
    WHEN MATCHED THEN UPDATE SET target.IsActive = 1
    WHEN NOT MATCHED THEN INSERT (RoleId, PermissionId, IsActive)
        VALUES (source.RoleId, source.PermissionId, 1);

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
