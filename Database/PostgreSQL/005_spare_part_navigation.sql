-- Đăng ký chức năng con để cây quản lý, menu module và phân quyền dùng chung dữ liệu.
BEGIN;

INSERT INTO public."app_Function"
    ("FunctionCode", "ParentFunctionId", "DisplayName", "Description", "NavigationTarget",
     "SortOrder", "IsVisible", "IsActive", "IsWide", "DevelopmentStatus")
SELECT seed.code, parent."FunctionId", seed.caption, seed.caption, seed.target,
       seed.sort, true, true, false, 'COMPLETED'
FROM (VALUES
    ('ASSET.SPARE_PART.MATERIAL', '備品資料', 'SparePartMaterialView', 10),
    ('ASSET.SPARE_PART.MACHINE', '設備管理', 'SparePartMachineView', 20),
    ('ASSET.SPARE_PART.TRANSACTION', '進出庫管理', 'SparePartTransactionView', 30),
    ('ASSET.SPARE_PART.RECHECK', '複盤作業', 'SparePartRecheckView', 40),
    ('ASSET.SPARE_PART.MY_RECOVERY', '我的回收作業', 'SparePartRecoveryTaskView', 50),
    ('ASSET.SPARE_PART.MANAGEMENT', '機邊庫【經理室】', NULL, 60)
) seed(code, caption, target, sort)
JOIN public."app_Function" parent ON parent."FunctionCode" = 'ASSET.SPARE_PART'
ON CONFLICT ("FunctionCode") DO NOTHING;

INSERT INTO public."app_Function"
    ("FunctionCode", "ParentFunctionId", "DisplayName", "Description", "NavigationTarget",
     "SortOrder", "IsVisible", "IsActive", "IsWide", "DevelopmentStatus")
SELECT seed.code, parent."FunctionId", seed.caption, seed.caption, seed.target,
       seed.sort, true, true, false, 'COMPLETED'
FROM (VALUES
    ('ASSET.SPARE_PART.INSPECTION', '盤點批次', 'SparePartInspectionView', 10),
    ('ASSET.SPARE_PART.COST', '成本計算', 'SparePartCostView', 20),
    ('ASSET.SPARE_PART.RECOVERY', '回收管理', 'SparePartRecoveryView', 30)
) seed(code, caption, target, sort)
JOIN public."app_Function" parent ON parent."FunctionCode" = 'ASSET.SPARE_PART.MANAGEMENT'
ON CONFLICT ("FunctionCode") DO NOTHING;

INSERT INTO public."auth_Permission"
    ("FunctionId", "ActionCode", "PermissionCode", "DisplayName", "IsActive")
SELECT "FunctionId", 'VIEW', "FunctionCode" || '.VIEW', '檢視' || "DisplayName", true
FROM public."app_Function"
WHERE "FunctionCode" LIKE 'ASSET.SPARE_PART.%'
  AND "NavigationTarget" IS NOT NULL
ON CONFLICT ("PermissionCode") DO NOTHING;

-- Giữ phạm vi truy cập của từng role đang có; không cấp các mục 經理室 cho role chỉ xem.
INSERT INTO public."auth_RolePermission" ("RoleId", "PermissionId", "IsActive")
SELECT DISTINCT existing."RoleId", child_permission."PermissionId", true
FROM public."auth_RolePermission" existing
JOIN public."auth_Permission" original ON original."PermissionId" = existing."PermissionId" AND original."IsActive"
JOIN public."auth_Permission" child_permission ON child_permission."ActionCode" = 'VIEW' AND child_permission."IsActive"
JOIN public."app_Function" child ON child."FunctionId" = child_permission."FunctionId"
JOIN public."app_Function" parent ON parent."FunctionId" = child."ParentFunctionId"
WHERE existing."IsActive"
  AND child."FunctionCode" LIKE 'ASSET.SPARE_PART.%'
  AND (
      (parent."FunctionCode" = 'ASSET.SPARE_PART' AND original."PermissionCode" IN ('ASSET.SPARE_PART.VIEW', 'ASSET.SPARE_PART.ADMIN'))
      OR (parent."FunctionCode" = 'ASSET.SPARE_PART.MANAGEMENT' AND original."PermissionCode" IN ('ASSET.SPARE_PART.APPROVE', 'ASSET.SPARE_PART.ADMIN'))
  )
ON CONFLICT DO NOTHING;

COMMIT;
