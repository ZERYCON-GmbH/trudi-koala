@echo off
echo.
echo ===========================================================================
echo.
echo TRuDI Build Script
echo.
echo ===========================================================================
echo.

setlocal EnableExtensions
setlocal EnableDelayedExpansion

echo Build started at: %DATE% %TIME%
where git >nul
IF %ERRORLEVEL% EQU 0 (
	echo Last git tag: 
	git describe --tags --dirty
	echo Last commit hash: 
	git rev-parse HEAD
)

echo CERTIFICATE_SUBJECT_NAME=%CERTIFICATE_SUBJECT_NAME%

echo.
echo ===========================================================================
echo.
echo Build 64-Bit version 
echo.
echo ===========================================================================
echo.

echo.
echo ===========================================================================
echo.
echo Build ASP.NET Core backend
echo.
echo ===========================================================================
echo.

cd TRuDI.Backend
rmdir /S /Q bin\dist 2>nul
rmdir /S /Q ..\..\dist\win-unpacked 2>nul

rem Normal build to generate the precompiled views
dotnet build -c Release
if errorlevel 1 goto error

rem Publish the application
dotnet publish -c Release -r win7-x64 --self-contained -o bin\dist\win7-x64 -p:SelfContainedBuild=true
if errorlevel 1 goto error

echo.
echo ===========================================================================
echo.
echo Delete file not needed for TRuDI deployment
echo.
echo ===========================================================================
echo.

del /S bin\dist\*.pdb 2>nul
rmdir /S /Q bin\dist\win7-x64\es 2>nul
rmdir /S /Q bin\dist\win7-x64\fr 2>nul
rmdir /S /Q bin\dist\win7-x64\it 2>nul
rmdir /S /Q bin\dist\win7-x64\ja 2>nul
rmdir /S /Q bin\dist\win7-x64\ko 2>nul
rmdir /S /Q bin\dist\win7-x64\ru 2>nul
rmdir /S /Q bin\dist\win7-x64\zh-Hans 2>nul
rmdir /S /Q bin\dist\win7-x64\zh-Hant 2>nul

echo.
echo ===========================================================================
echo.
echo Copy precompiled Views to self-contained output
echo.
echo ===========================================================================
echo.

copy bin\Release\netcoreapp2.0\TRuDI.Backend.PrecompiledViews.dll bin\dist\win7-x64\TRuDI.Backend.PrecompiledViews.dll

echo.
echo ===========================================================================
echo.
echo Build Electron frontend
echo.
echo ===========================================================================
echo.

cd ..\TRuDI.Frontend
cmd /c npm install
if errorlevel 1 goto error

echo.
echo ===========================================================================
echo.
echo Generate checksums file
echo.
echo ===========================================================================
echo.

cmd /c node ..\Utils\createDigestList.js ..\TRuDI.Backend\bin\dist\win7-x64 checksums-win32-x64.json
if errorlevel 1 goto error

echo.
echo ===========================================================================
echo.
echo Build Electron Setup
echo.
echo ===========================================================================
echo.

if defined CERTIFICATE_SUBJECT_NAME (
	call node_modules\.bin\electron-builder.cmd --x64 --config.win.certificateSubjectName="%CERTIFICATE_SUBJECT_NAME%"
) else (
	call node_modules\.bin\electron-builder.cmd --x64
)

if errorlevel 1 goto error

echo.
echo ===========================================================================
echo.
echo Rename the create file to add the target architecture to the filename
echo.
echo ===========================================================================
echo.

for %%x in (..\..\dist\NSIS_OUTPUT_*.exe) do (
    set srcName="%%x" 
	set destName="%%~nx-x86_64.exe"
	set destName=!destName:NSIS_OUTPUT_=!

	del ..\..\dist\!destName! 2>nul
	del /q ..\..\dist\*.blockmap 2>nul
	ren !srcName! !destName!
)

echo.
echo ===========================================================================
echo.
echo Build 32-Bit version
echo.
echo ===========================================================================
echo.

echo.
echo ===========================================================================
echo.
echo Build ASP.NET Core backend
echo.
echo ===========================================================================
echo.

cd ..\TRuDI.Backend
rmdir /S /Q bin\dist 2>nul
rmdir /S /Q ..\..\dist\win-unpacked 2>nul

rem Normal build to generate the precompiled views
dotnet build -c Release
if errorlevel 1 goto error

rem Publish the application
dotnet publish -c Release -r win7-x86 --self-contained -o bin\dist\win7-x86 -p:SelfContainedBuild=true
if errorlevel 1 goto error

echo.
echo ===========================================================================
echo.
echo Delete file not needed for TRuDI deployment
echo.
echo ===========================================================================
echo.

del /S bin\dist\*.pdb 2>nul
rmdir /S /Q bin\dist\win7-x86\es 2>nul
rmdir /S /Q bin\dist\win7-x86\fr 2>nul
rmdir /S /Q bin\dist\win7-x86\it 2>nul
rmdir /S /Q bin\dist\win7-x86\ja 2>nul
rmdir /S /Q bin\dist\win7-x86\ko 2>nul
rmdir /S /Q bin\dist\win7-x86\ru 2>nul
rmdir /S /Q bin\dist\win7-x86\zh-Hans 2>nul
rmdir /S /Q bin\dist\win7-x86\zh-Hant 2>nul

echo.
echo ===========================================================================
echo.
echo Copy precompiled Views to self-contained output
echo.
echo ===========================================================================
echo.

copy bin\Release\netcoreapp2.0\TRuDI.Backend.PrecompiledViews.dll bin\dist\win7-x86\TRuDI.Backend.PrecompiledViews.dll

echo.
echo ===========================================================================
echo.
echo Build Electron frontend
echo.
echo ===========================================================================
echo.

cd ..\TRuDI.Frontend

echo.
echo ===========================================================================
echo.
echo Generate checksums file
echo.
echo ===========================================================================
echo.

cmd /c node ..\Utils\createDigestList.js ..\TRuDI.Backend\bin\dist\win7-x86 checksums-win32-x86.json
if errorlevel 1 goto error

echo.
echo ===========================================================================
echo.
echo Build Electron Setup
echo.
echo ===========================================================================
echo.

if defined CERTIFICATE_SUBJECT_NAME (
	call node_modules\.bin\electron-builder.cmd --ia32 --config.win.certificateSubjectName="%CERTIFICATE_SUBJECT_NAME%"
) else (
	call node_modules\.bin\electron-builder.cmd --ia32
)

if errorlevel 1 goto error

echo Rename the create file to add the target architecture to the filename
for %%x in (..\..\dist\NSIS_OUTPUT_*.exe) do (
    set srcName="%%x" 
	set destName="%%~nx-x86_32.exe"
	set destName=!destName:NSIS_OUTPUT_=!

	del ..\..\dist\!destName! 2>nul
	del /q ..\..\dist\*.blockmap 2>nul
	ren !srcName! !destName!
)

echo.
echo ===========================================================================
echo.
echo Build finished
echo.
echo ===========================================================================
echo.
exit 0

:error
echo.
echo ===========================================================================
echo.
echo Build failed!
echo.
echo ===========================================================================
echo.
exit 1
