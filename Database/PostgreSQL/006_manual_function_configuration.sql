BEGIN;
ALTER TABLE public."app_Function" ADD COLUMN IF NOT EXISTS "IconKey" varchar(64);

-- Đưa chức năng quản trị về cùng module để menu có thể thêm nhóm/mục bằng cấu hình.
INSERT INTO public."app_Function" ("FunctionCode","ParentFunctionId","DisplayName","SortOrder","IsVisible","IsActive","IsWide","DevelopmentStatus")
SELECT seed.code, root."FunctionId", seed.caption, seed.sort, true, true, false, 'COMPLETED'
FROM (VALUES ('SYSTEM.SETTINGS.PEOPLE','人員與組織',10), ('SYSTEM.SETTINGS.AUTHORIZATION','權限與安全',20),
             ('SYSTEM.SETTINGS.GOVERNANCE','稽核與設定',30)) seed(code,caption,sort)
JOIN public."app_Function" root ON root."FunctionCode"='SYSTEM.SETTINGS'
ON CONFLICT DO NOTHING;

UPDATE public."app_Function" child SET "ParentFunctionId"=parent."FunctionId", "IsVisible"=true
FROM public."app_Function" parent, (VALUES
 ('SYSTEM.USER','SYSTEM.SETTINGS.PEOPLE'), ('SYSTEM.DEPARTMENT','SYSTEM.SETTINGS.PEOPLE'), ('SYSTEM.JOB_TITLE','SYSTEM.SETTINGS.PEOPLE'),
 ('SYSTEM.GROUP','SYSTEM.SETTINGS.AUTHORIZATION'), ('SYSTEM.ROLE','SYSTEM.SETTINGS.AUTHORIZATION'),
 ('SYSTEM.AUDIT','SYSTEM.SETTINGS.GOVERNANCE'), ('SYSTEM.FUNCTION','SYSTEM.SETTINGS.GOVERNANCE'),
 ('SYSTEM.SETTING','SYSTEM.SETTINGS.GOVERNANCE'), ('SYSTEM.HEALTH','SYSTEM.SETTINGS.GOVERNANCE')
) seed(code,parent_code)
WHERE child."FunctionCode"=seed.code AND parent."FunctionCode"=seed.parent_code
AND child."ParentFunctionId"=(SELECT "FunctionId" FROM public."app_Function" WHERE "FunctionCode"='SYSTEM');

INSERT INTO public."app_Function" ("FunctionCode","ParentFunctionId","DisplayName","NavigationTarget","SortOrder","IsVisible","IsActive","IsWide","DevelopmentStatus")
SELECT 'SYSTEM.USER.PERMISSION',"FunctionId",'人員權限','EmployeePermissionView',10,true,true,false,'COMPLETED'
FROM public."app_Function" WHERE "FunctionCode"='SYSTEM.SETTINGS.AUTHORIZATION'
ON CONFLICT DO NOTHING;
UPDATE public."auth_Permission" SET "FunctionId"=(SELECT "FunctionId" FROM public."app_Function" WHERE "FunctionCode"='SYSTEM.USER.PERMISSION')
WHERE "PermissionCode" IN ('SYSTEM.USER.PERMISSION.VIEW','SYSTEM.USER.PERMISSION.ADMIN');


UPDATE public."app_Function" SET "IconKey" = CASE "NavigationTarget"
 WHEN 'SparePartMaterialView' THEN 'View' WHEN 'SparePartMachineView' THEN 'Equipment'
 WHEN 'SparePartTransactionView' THEN 'Transfer' WHEN 'SparePartRecheckView' THEN 'Refresh'
 WHEN 'SparePartRecoveryTaskView' THEN 'UserTransfer' WHEN 'SparePartInspectionView' THEN 'SelectionChecked'
 WHEN 'SparePartCostView' THEN 'Cost' WHEN 'SparePartRecoveryView' THEN 'Schedule'
 WHEN 'EmployeeManagementView' THEN 'ChangeUser' WHEN 'DepartmentManagementView' THEN 'Department'
 WHEN 'JobTitleManagementView' THEN 'Promote' WHEN 'EmployeePermissionView' THEN 'AddUserGroup'
 WHEN 'SecurityGroupManagementView' THEN 'AddUserGroup' WHEN 'RolePermissionManagementView' THEN 'SelectionChecked'
 WHEN 'AuditLogView' THEN 'View' WHEN 'FunctionManagementView' THEN 'Equipment'
 WHEN 'SystemSettingManagementView' THEN 'Edit' WHEN 'SystemHealthView' THEN 'Info'
 ELSE 'Department' END WHERE "IconKey" IS NULL;
UPDATE public."sys_SystemSetting" SET "SettingValue"='PG006', "UpdatedAt"=CURRENT_TIMESTAMP AT TIME ZONE 'UTC'
WHERE "SettingKey"='SYSTEM.SCHEMA_VERSION' AND "SettingValue" IS DISTINCT FROM 'PG006';
COMMIT;
