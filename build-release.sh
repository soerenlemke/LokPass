#!/bin/bash

VERSION="0.0.1"
PROJECT="LokPass.Desktop/LokPass.Desktop.csproj"

echo "Building LokPass v$VERSION for all platforms..."

# Clean previous builds
rm -rf publish
rm -f *.zip

# Build for all platforms
echo "Building Windows x64..."
dotnet publish $PROJECT -c Release -r win-x64 --self-contained true \
  -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true \
  -o ./publish/win-x64

echo "Building Linux x64..."
dotnet publish $PROJECT -c Release -r linux-x64 --self-contained true \
  -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true \
  -o ./publish/linux-x64

echo "Building macOS x64..."
dotnet publish $PROJECT -c Release -r osx-x64 --self-contained true \
  -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true \
  -o ./publish/osx-x64

echo "Building macOS ARM64..."
dotnet publish $PROJECT -c Release -r osx-arm64 --self-contained true \
  -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true \
  -o ./publish/osx-arm64

# Create archives
echo "Creating archives..."
cd publish/win-x64 && zip -r ../../LokPass-v$VERSION-win-x64.zip . && cd ../..
cd publish/linux-x64 && zip -r ../../LokPass-v$VERSION-linux-x64.zip . && cd ../..
cd publish/osx-x64 && zip -r ../../LokPass-v$VERSION-osx-x64.zip . && cd ../..
cd publish/osx-arm64 && zip -r ../../LokPass-v$VERSION-osx-arm64.zip . && cd ../..

echo ""
echo "✅ Build complete! Archives created:"
ls -lh *.zip

echo ""
echo "📦 To create GitHub release, run:"
echo "  git tag -a v$VERSION -m 'Release version $VERSION'"
echo "  git push origin v$VERSION"
echo "  gh release create v$VERSION *.zip --title 'Version $VERSION'"