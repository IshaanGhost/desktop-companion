# Build script to create a single .exe file for Desktop Companion
# This creates a self-contained executable that doesn't require .NET to be installed

Write-Host "Building Desktop Companion as single-file executable..." -ForegroundColor Cyan

# Clean previous builds
Write-Host "`nCleaning previous builds..." -ForegroundColor Yellow
dotnet clean -c Release

# Publish as single-file self-contained executable
Write-Host "`nPublishing as single-file executable..." -ForegroundColor Yellow
dotnet publish -c Release -r win-x64 --self-contained true `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:EnableCompressionInSingleFile=true `
    -p:DebugType=None `
    -p:DebugSymbols=false

if ($LASTEXITCODE -eq 0) {
    $exePath = "bin\Release\net8.0-windows\win-x64\publish\DesktopCompanions.exe"
    
    if (Test-Path $exePath) {
        $fileSize = (Get-Item $exePath).Length / 1MB
        Write-Host "`n✓ Build successful!" -ForegroundColor Green
        Write-Host "`nExecutable location: $((Resolve-Path $exePath).Path)" -ForegroundColor Cyan
        Write-Host "File size: $([math]::Round($fileSize, 2)) MB" -ForegroundColor Cyan
        Write-Host "`nThis .exe file can be run on any Windows 10/11 computer without installing .NET!" -ForegroundColor Green
        
        # Optionally open the folder
        Write-Host "`nOpening publish folder..." -ForegroundColor Yellow
        Start-Process explorer.exe -ArgumentList "/select,`"$((Resolve-Path $exePath).Path)`""
    } else {
        Write-Host "`n✗ Error: Executable not found at expected location" -ForegroundColor Red
        exit 1
    }
} else {
    Write-Host "`n✗ Build failed!" -ForegroundColor Red
    exit 1
}

