@echo off
setlocal

pushd "%~dp0"
powershell.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File "%~dp0Installer\Build-Installer.ps1" %*
set "BUILD_EXIT_CODE=%ERRORLEVEL%"
popd

if not "%BUILD_EXIT_CODE%"=="0" (
    echo.
    echo Xay dung bo cai that bai. Ma loi: %BUILD_EXIT_CODE%
    pause
    exit /b %BUILD_EXIT_CODE%
)

echo.
echo Da tao bo cai thanh cong.
pause
exit /b 0
