#!/bin/sh
# TRuDI Build Script
#
# Required packages to build on Ubuntu:
#
#   sudo apt-get install --no-install-recommends -y icnsutils graphicsmagick xz-utils
#   sudo apt-get install xorriso -y
#   sudo npm install electron-builder -g


cd TRuDI.Backend
rm -rf bin/dist
rm -rf ../../dist/linux-unpacked

dotnet build -c Release
dotnet publish -c Release -r linux-x64 --self-contained -o bin/dist/linux-x64 -p:SelfContainedBuild=true

# Delete file not needed for TRuDI deployment
find bin/dist/ -name '*.pdb' -delete
rm -rf bin/dist/linux-x64/es
rm -rf bin/dist/linux-x64/fr
rm -rf bin/dist/linux-x64/it
rm -rf bin/dist/linux-x64/ja
rm -rf bin/dist/linux-x64/ko
rm -rf bin/dist/linux-x64/ru
rm -rf bin/dist/linux-x64/zh-Hans
rm -rf bin/dist/linux-x64/zh-Hant

# Copy additional dependencies of .Net Core
mkdir bin/dist/linux-x64/netcoredeps
cp /usr/lib/x86_64-linux-gnu/libunwind.so.8 bin/dist/linux-x64/netcoredeps
cp /usr/lib/x86_64-linux-gnu/libunwind-x86_64.so.8 bin/dist/linux-x64/netcoredeps
cp /lib/x86_64-linux-gnu/libssl.so.1.0.0 bin/dist/linux-x64/netcoredeps
cp /lib/x86_64-linux-gnu/libcrypto.so.1.0.0 bin/dist/linux-x64/netcoredeps

# Copy precompiled Views to self-contained output
cp bin/Release/netcoreapp2.0/TRuDI.Backend.PrecompiledViews.dll bin/dist/linux-x64/TRuDI.Backend.PrecompiledViews.dll

# Build Electron frontend
cd ../TRuDI.Frontend
npm install

# Generate checksums file
node ../Utils/createDigestList.js ../TRuDI.Backend/bin/dist/linux-x64 checksums-linux.json

USE_SYSTEM_XORRISO=true npm run dist


