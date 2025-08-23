@echo off
setlocal enabledelayedexpansion

echo Starting submodule download...
echo.

cd /d "%~dp0"

REM Check if .gitmodules exists
if not exist ".gitmodules" (
    echo ERROR: .gitmodules file not found
    echo This script must be run from a directory containing .gitmodules
    pause
    exit /b 1
)

echo Reading submodules from .gitmodules...
type .gitmodules
echo.

REM Parse .gitmodules to extract submodule information
set "current_path="
set "current_url="
set "download_count=0"

for /f "delims=" %%i in (.gitmodules) do (
    set "line=%%i"
    
    REM Check for path line
    echo !line! | findstr /c:"path = " >nul
    if !errorlevel! equ 0 (
        for /f "tokens=2* delims== " %%j in ("!line!") do (
            set "current_path=%%j"
            set "current_path=!current_path: =!"
        )
    )
    
    REM Check for url line
    echo !line! | findstr /c:"url = " >nul
    if !errorlevel! equ 0 (
        for /f "tokens=2* delims== " %%j in ("!line!") do (
            set "current_url=%%j"
            set "current_url=!current_url: =!"
            
            REM When we have both path and url, download the submodule
            if not "!current_path!"=="" if not "!current_url!"=="" (
                echo.
                echo Downloading submodule from: !current_url!
                echo To directory: !current_path!
                
                REM Clean up existing directory
                if exist "!current_path!" rmdir /s /q "!current_path!" 2>nul
                
                REM Create parent directory if needed
                for %%p in ("!current_path!") do (
                    if not exist "%%~dp" mkdir "%%~dp" 2>nul
                )
                
                REM Clone the repository
                git clone "!current_url!" "!current_path!"
                
                if exist "!current_path!" (
                    echo ✓ Successfully downloaded to: !current_path!
                    
                    REM Count files
                    for /f %%k in ('dir /s /b "!current_path!\*" 2^>nul ^| find /c /v ""') do (
                        echo   Downloaded %%k files
                    )
                    set /a download_count+=1
                ) else (
                    echo ✗ Failed to download to: !current_path!
                )
                
                REM Reset for next submodule
                set "current_path="
                set "current_url="
                echo.
            )
        )
    )
)

echo ====================================
if !download_count! gtr 0 (
    echo SUCCESS! Downloaded !download_count! submodule(s)
    echo.
    echo To integrate with git submodules later, you can run:
    echo git submodule update --init --recursive
) else (
    echo No submodules were downloaded
    echo Check .gitmodules format and your internet connection
)

echo.
pause
