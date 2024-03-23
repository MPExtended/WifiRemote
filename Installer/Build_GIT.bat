@echo off
cls
Title Building MediaPortal WiFi Remote (RELEASE)

ECHO.
ECHO Building WiFiRemote ...

cd ..\Sources

setlocal enabledelayedexpansion

if "%programfiles(x86)%XXX"=="XXX" goto 32BIT
	:: 64-bit
	set PROGS=%programfiles(x86)%
	goto CONT
:32BIT
	set PROGS=%ProgramFiles%
:CONT
IF NOT EXIST "%PROGS%\Team MediaPortal\MediaPortal\" SET PROGS=C:

:: Prepare version
for /f "tokens=*" %%a in ('git rev-list HEAD --count') do set REVISION=%%a 
set REVISION=%REVISION: =%
"..\Installer\Tools\sed.exe" -i "s/\$WCREV\$/%REVISION%/g" WifiRemote\Properties\AssemblyInfo.cs

:: Build
"%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBUILD.exe" /target:Rebuild /property:Configuration=RELEASE /property:Platform="Any CPU" /fl /flp:logfile=WiFiRemote.log;verbosity=diagnostic WiFiRemote.sln

: Revert version
git checkout WifiRemote\Properties\AssemblyInfo.cs 

cd ..\Installer

pause
