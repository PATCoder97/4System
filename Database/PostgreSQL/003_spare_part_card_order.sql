-- Chỉ hiển thị card 機邊庫 để mở module SparePart hiện có, giữ nguyên mã permission.
BEGIN;

UPDATE public."app_Function"
SET "DisplayName" = '機邊庫',
    "NavigationTarget" = 'SparePartForm',
    "IsVisible" = true,
    "SortOrder" = 10,
    "UpdatedAt" = CURRENT_TIMESTAMP AT TIME ZONE 'UTC'
WHERE "FunctionCode" = 'ASSET.SPARE_PART';

UPDATE public."app_Function"
SET "IsVisible" = false,
    "UpdatedAt" = CURRENT_TIMESTAMP AT TIME ZONE 'UTC'
WHERE "FunctionCode" = 'ASSET.MACHINE_WAREHOUSE';

COMMIT;
