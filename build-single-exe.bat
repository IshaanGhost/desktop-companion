@echo off
REM Build script to create a single .exe file for Desktop Companion
REM This creates a self-contained executable that doesn't require .NET to be installed

echo Building Desktop Companion as single-file executable...
echo.

REM Clean previous builds
echo Cleaning previous builds...
dotnet clean -c Release

REM Publish as single-file self-contained executable
echo.
echo Publishing as single-file executable...
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -p:DebugType=None -p:DebugSymbols=false

if %ERRORLEVEL% EQU 0 (
    echo.
    echo Build successful!
    echo.
    echo Executable location: bin\Release\net8.0-windows\win-x64\publish\DesktopCompanions.exe
    echo This .exe file can be run on any Windows 10/11 computer without installing .NET!
    echo.
    echo Opening publish folder...
    start explorer.exe /select,"bin\Release\net8.0-windows\win-x64\publish\DesktopCompanions.exe"
) else (
    echo.
    echo Build failed!
    exit /b 1
)

pause

