/* Seed only stable system codes. No real user is created by this script. */
SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    MERGE dbo.app_Function AS target
    USING (VALUES
        ('MAIN',              NULL,       N'主頁',           NULL,                           10),
        ('SYSTEM',            NULL,       N'系統管理',       NULL,                           90),
        ('SYSTEM.USER',       'SYSTEM',   N'使用者管理',     'UserManagementForm',           10),
        ('SYSTEM.GROUP',      'SYSTEM',   N'安全性群組',     'SecurityGroupManagementForm',  20),
        ('SYSTEM.ROLE',       'SYSTEM',   N'角色與權限',     'RoleManagementForm',           30),
        ('SYSTEM.FUNCTION',   'SYSTEM',   N'功能清單',       'FunctionManagementForm',       40)
    ) AS source(FunctionCode, ParentCode, DisplayName, NavigationTarget, SortOrder)
    ON target.FunctionCode = source.FunctionCode
    WHEN MATCHED THEN UPDATE SET
        target.DisplayName = source.DisplayName,
        target.NavigationTarget = source.NavigationTarget,
        target.SortOrder = source.SortOrder,
        target.IsActive = 1
    WHEN NOT MATCHED THEN INSERT
        (ParentFunctionId, FunctionCode, DisplayName, NavigationTarget, SortOrder)
        VALUES (NULL, source.FunctionCode, source.DisplayName, source.NavigationTarget, source.SortOrder);

    UPDATE child
       SET ParentFunctionId = parent.FunctionId
      FROM dbo.app_Function AS child
      INNER JOIN (VALUES
          ('SYSTEM.USER', 'SYSTEM'),
          ('SYSTEM.GROUP', 'SYSTEM'),
          ('SYSTEM.ROLE', 'SYSTEM'),
          ('SYSTEM.FUNCTION', 'SYSTEM')
      ) AS relation(ChildCode, ParentCode) ON relation.ChildCode = child.FunctionCode
      INNER JOIN dbo.app_Function AS parent ON parent.FunctionCode = relation.ParentCode;

    MERGE dbo.auth_Permission AS target
    USING
    (
        SELECT f.FunctionId, seed.ActionCode, seed.PermissionCode, seed.DisplayName
        FROM (VALUES
            ('MAIN',            'ACCESS', 'MAIN.ACCESS',            N'存取主頁'),
            ('SYSTEM',          'ACCESS', 'SYSTEM.ACCESS',          N'存取系統管理'),
            ('SYSTEM.USER',     'VIEW',   'SYSTEM.USER.VIEW',       N'檢視使用者'),
            ('SYSTEM.USER',     'ADMIN',  'SYSTEM.USER.ADMIN',      N'管理使用者'),
            ('SYSTEM.GROUP',    'VIEW',   'SYSTEM.GROUP.VIEW',      N'檢視安全性群組'),
            ('SYSTEM.GROUP',    'ADMIN',  'SYSTEM.GROUP.ADMIN',     N'管理安全性群組'),
            ('SYSTEM.ROLE',     'VIEW',   'SYSTEM.ROLE.VIEW',       N'檢視角色與權限'),
            ('SYSTEM.ROLE',     'ADMIN',  'SYSTEM.ROLE.ADMIN',      N'管理角色與權限'),
            ('SYSTEM.FUNCTION', 'VIEW',   'SYSTEM.FUNCTION.VIEW',   N'檢視功能清單'),
            ('SYSTEM.FUNCTION', 'ADMIN',  'SYSTEM.FUNCTION.ADMIN',  N'管理功能清單')
        ) AS seed(FunctionCode, ActionCode, PermissionCode, DisplayName)
        INNER JOIN dbo.app_Function AS f ON f.FunctionCode = seed.FunctionCode
    ) AS source
    ON target.PermissionCode = source.PermissionCode
    WHEN MATCHED THEN UPDATE SET
        target.DisplayName = source.DisplayName,
        target.IsActive = 1
    WHEN NOT MATCHED THEN INSERT
        (FunctionId, ActionCode, PermissionCode, DisplayName)
        VALUES (source.FunctionId, source.ActionCode, source.PermissionCode, source.DisplayName);

    MERGE dbo.auth_Role AS target
    USING (VALUES
        ('STANDARD_USER', N'標準使用者', N'登入及開啟主頁的基本權限', 1),
        ('SECURITY_ADMIN', N'安全性管理員', N'管理使用者、群組、角色與權限', 1)
    ) AS source(RoleCode, RoleName, Description, IsSystemRole)
    ON target.RoleCode = source.RoleCode
    WHEN MATCHED THEN UPDATE SET
        target.RoleName = source.RoleName,
        target.Description = source.Description,
        target.IsActive = 1
    WHEN NOT MATCHED THEN INSERT
        (RoleCode, RoleName, Description, IsSystemRole)
        VALUES (source.RoleCode, source.RoleName, source.Description, source.IsSystemRole);

    MERGE dbo.auth_SecurityGroup AS target
    USING (VALUES
        ('STANDARD_USERS', N'標準使用者', N'系統使用者的預設群組', 1),
        ('SYSTEM_ADMINISTRATORS', N'系統管理員', N'擁有完整安全性管理權限的群組', 1)
    ) AS source(GroupCode, GroupName, Description, IsSystemGroup)
    ON target.GroupCode = source.GroupCode
    WHEN MATCHED THEN UPDATE SET
        target.GroupName = source.GroupName,
        target.Description = source.Description,
        target.IsActive = 1
    WHEN NOT MATCHED THEN INSERT
        (GroupCode, GroupName, Description, IsSystemGroup)
        VALUES (source.GroupCode, source.GroupName, source.Description, source.IsSystemGroup);

    MERGE dbo.auth_GroupRole AS target
    USING
    (
        SELECT g.GroupId, r.RoleId
        FROM (VALUES
            ('STANDARD_USERS', 'STANDARD_USER'),
            ('SYSTEM_ADMINISTRATORS', 'SECURITY_ADMIN')
        ) AS mapping(GroupCode, RoleCode)
        INNER JOIN dbo.auth_SecurityGroup AS g ON g.GroupCode = mapping.GroupCode
        INNER JOIN dbo.auth_Role AS r ON r.RoleCode = mapping.RoleCode
    ) AS source
    ON target.GroupId = source.GroupId AND target.RoleId = source.RoleId
    WHEN MATCHED THEN UPDATE SET target.IsActive = 1
    WHEN NOT MATCHED THEN INSERT (GroupId, RoleId) VALUES (source.GroupId, source.RoleId);

    MERGE dbo.auth_RolePermission AS target
    USING
    (
        SELECT role.RoleId, permission.PermissionId
        FROM dbo.auth_Role AS role
        INNER JOIN dbo.auth_Permission AS permission
            ON (role.RoleCode = 'STANDARD_USER' AND permission.PermissionCode = 'MAIN.ACCESS')
            OR (role.RoleCode = 'SECURITY_ADMIN')
    ) AS source
    ON target.RoleId = source.RoleId AND target.PermissionId = source.PermissionId
    WHEN MATCHED THEN UPDATE SET target.IsActive = 1
    WHEN NOT MATCHED THEN INSERT (RoleId, PermissionId) VALUES (source.RoleId, source.PermissionId);

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
