/* Seed only stable system codes. No real user is created by this script. */
SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    MERGE dbo.app_Function AS target
    USING (VALUES
        ('MAIN',              NULL,       N'Trang chủ',             NULL,                           10),
        ('SYSTEM',            NULL,       N'Quản trị hệ thống',     NULL,                           90),
        ('SYSTEM.USER',       'SYSTEM',   N'Quản lý người dùng',    'UserManagementForm',           10),
        ('SYSTEM.GROUP',      'SYSTEM',   N'Nhóm bảo mật',          'SecurityGroupManagementForm',  20),
        ('SYSTEM.ROLE',       'SYSTEM',   N'Vai trò và quyền',      'RoleManagementForm',           30),
        ('SYSTEM.FUNCTION',   'SYSTEM',   N'Danh mục chức năng',    'FunctionManagementForm',       40)
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
            ('MAIN',            'ACCESS', 'MAIN.ACCESS',            N'Truy cập trang chủ'),
            ('SYSTEM',          'ACCESS', 'SYSTEM.ACCESS',          N'Truy cập quản trị hệ thống'),
            ('SYSTEM.USER',     'VIEW',   'SYSTEM.USER.VIEW',       N'Xem người dùng'),
            ('SYSTEM.USER',     'ADMIN',  'SYSTEM.USER.ADMIN',      N'Quản trị người dùng'),
            ('SYSTEM.GROUP',    'VIEW',   'SYSTEM.GROUP.VIEW',      N'Xem nhóm bảo mật'),
            ('SYSTEM.GROUP',    'ADMIN',  'SYSTEM.GROUP.ADMIN',     N'Quản trị nhóm bảo mật'),
            ('SYSTEM.ROLE',     'VIEW',   'SYSTEM.ROLE.VIEW',       N'Xem vai trò và quyền'),
            ('SYSTEM.ROLE',     'ADMIN',  'SYSTEM.ROLE.ADMIN',      N'Quản trị vai trò và quyền'),
            ('SYSTEM.FUNCTION', 'VIEW',   'SYSTEM.FUNCTION.VIEW',   N'Xem danh mục chức năng'),
            ('SYSTEM.FUNCTION', 'ADMIN',  'SYSTEM.FUNCTION.ADMIN',  N'Quản trị danh mục chức năng')
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
        ('STANDARD_USER', N'Người dùng tiêu chuẩn', N'Quyền cơ bản để đăng nhập và mở trang chủ', 1),
        ('SECURITY_ADMIN', N'Quản trị bảo mật', N'Quản lý người dùng, nhóm, vai trò và quyền', 1)
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
        ('STANDARD_USERS', N'Người dùng tiêu chuẩn', N'Nhóm mặc định của người dùng hệ thống', 1),
        ('SYSTEM_ADMINISTRATORS', N'Quản trị viên hệ thống', N'Nhóm có toàn quyền quản trị bảo mật', 1)
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
