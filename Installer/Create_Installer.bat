@echo off
cls
Title Creating MediaPortal WiFiRemote Installer

ECHO Building Installer ...

if "%programfiles(x86)%XXX"=="XXX" goto 32BIT
    :: 64-bit
    set PROGS=%programfiles(x86)%
    goto CONT
:32BIT
    set PROGS=%ProgramFiles%
:CONT

IF NOT EXIST "%PROGS%\Team MediaPortal\MediaPortal\" SET PROGS=C:

:: Copy files
COPY /Y ..\Sources\WifiRemote\bin\Release\WifiRemote.dll Files\
COPY /Y ..\Sources\WifiRemote\Resources\WifiRemote.xml Files\

:: Get version from DLL
FOR /F "tokens=*" %%i IN ('Tools\sigcheck.exe /accepteula /nobanner /n "Files\WifiRemote.dll"') DO (SET version=%%i)

:: Temp xmp2 file
copy WifiRemoteInstaller.xmp2 WifiRemoteInstallerTemp.xmp2

:: Build MPE1
"%PROGS%\Team MediaPortal\MediaPortal\MPEMaker.exe" WifiRemoteInstallerTemp.xmp2 /B /V=%version% /UpdateXML

:: Cleanup
del WifiRemoteInstallerTemp.xmp2
