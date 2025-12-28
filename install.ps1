#
# CNC Predictive Maintenance Tool - Windows Installation Script
# Usage: irm https://raw.githubusercontent.com/JonaShoukri/CNC-IIoT-Predictive-Maintenance-Tool/main/install.ps1 | iex
#

$ErrorActionPreference = "Stop"

$Repo = "JonaShoukri/CNC-IIoT-Predictive-Maintenance-Tool"
$AppName = "CNC-Predictive-Maintenance"
$InstallDir = "$env:LOCALAPPDATA\Programs\$AppName"

Write-Host ""
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "  CNC Predictive Maintenance Installer" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host ""

# Detect architecture
$Platform = "win-x64"
Write-Host "Platform: $Platform" -ForegroundColor Green

# Get latest release
Write-Host "Fetching latest release..."
try {
    $Release = Invoke-RestMethod -Uri "https://api.github.com/repos/$Repo/releases/latest"
    $Version = $Release.tag_name
    Write-Host "Latest version: $Version" -ForegroundColor Green
} catch {
    Write-Host "Failed to fetch latest version. Make sure a release exists." -ForegroundColor Red
    exit 1
}

# Download
$DownloadUrl = "https://github.com/$Repo/releases/download/$Version/$AppName-$Platform.zip"
$TempDir = New-TemporaryFile | ForEach-Object { Remove-Item $_; New-Item -ItemType Directory -Path $_ }
$ZipPath = Join-Path $TempDir "$AppName.zip"

Write-Host ""
Write-Host "Downloading from: $DownloadUrl"

try {
    Invoke-WebRequest -Uri $DownloadUrl -OutFile $ZipPath
} catch {
    Write-Host "Download failed." -ForegroundColor Red
    Remove-Item -Recurse -Force $TempDir
    exit 1
}

# Extract and install
Write-Host "Extracting..."
Expand-Archive -Path $ZipPath -DestinationPath $TempDir -Force

Write-Host "Installing to $InstallDir..."
if (-not (Test-Path $InstallDir)) {
    New-Item -ItemType Directory -Path $InstallDir -Force | Out-Null
}

Copy-Item -Path (Join-Path $TempDir "$AppName.exe") -Destination $InstallDir -Force

# Add to PATH
$CurrentPath = [Environment]::GetEnvironmentVariable("Path", "User")
if ($CurrentPath -notlike "*$InstallDir*") {
    Write-Host "Adding to PATH..." -ForegroundColor Yellow
    [Environment]::SetEnvironmentVariable("Path", "$CurrentPath;$InstallDir", "User")
    $env:Path = "$env:Path;$InstallDir"
}

# Cleanup
Remove-Item -Recurse -Force $TempDir

Write-Host ""
Write-Host "Installation complete!" -ForegroundColor Green
Write-Host ""
Write-Host "Run the application with:" -ForegroundColor White
Write-Host "  $AppName" -ForegroundColor Green
Write-Host ""
Write-Host "Note: You may need to restart your terminal for PATH changes to take effect." -ForegroundColor Yellow
