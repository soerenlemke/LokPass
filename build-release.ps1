$VERSION = "0.0.1"
$PROJECT = "LokPass.Desktop/LokPass.Desktop.csproj"

Write-Host "Building LokPass v$VERSION for all platforms..." -ForegroundColor Green

# Clean previous builds
Remove-Item -Path "publish" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -Path "*.zip" -Force -ErrorAction SilentlyContinue

# Build for all platforms
Write-Host "Building Windows x64..." -ForegroundColor Cyan
dotnet publish $PROJECT -c Release -r win-x64 --self-contained true `
  -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true `
  -o ./publish/win-x64

Write-Host "Building Linux x64..." -ForegroundColor Cyan
dotnet publish $PROJECT -c Release -r linux-x64 --self-contained true `
  -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true `
  -o ./publish/linux-x64

Write-Host "Building macOS x64..." -ForegroundColor Cyan
dotnet publish $PROJECT -c Release -r osx-x64 --self-contained true `
  -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true `
  -o ./publish/osx-x64

Write-Host "Building macOS ARM64..." -ForegroundColor Cyan
dotnet publish $PROJECT -c Release -r osx-arm64 --self-contained true `
  -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true `
  -o ./publish/osx-arm64

# Create archives
Write-Host "Creating archives..." -ForegroundColor Cyan
Compress-Archive -Path "publish/win-x64/*" -DestinationPath "LokPass-v$VERSION-win-x64.zip" -Force
Compress-Archive -Path "publish/linux-x64/*" -DestinationPath "LokPass-v$VERSION-linux-x64.zip" -Force
Compress-Archive -Path "publish/osx-x64/*" -DestinationPath "LokPass-v$VERSION-osx-x64.zip" -Force
Compress-Archive -Path "publish/osx-arm64/*" -DestinationPath "LokPass-v$VERSION-osx-arm64.zip" -Force

Write-Host ""
Write-Host "✅ Build complete! Archives created:" -ForegroundColor Green
Get-ChildItem *.zip | Format-Table Name, Length

Write-Host ""
Write-Host "📦 To create GitHub release, run:" -ForegroundColor Yellow
Write-Host "  git tag -a v$VERSION -m 'Release version $VERSION'"
Write-Host "  git push origin v$VERSION"
Write-Host "  gh release create v$VERSION *.zip --title 'Version $VERSION'"