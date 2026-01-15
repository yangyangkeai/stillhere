if "%1" == "" (
    CHCP 65001
    @echo off
)
echo clear...
cd ../../
rd /Q /S asm-publish
cd Core/App.SendMails
echo building...
dotnet publish -o ../../asm-publish -r linux-x64 -c Release --self-contained -p:DefineConstants=\"FORCE_LINUX;RELEASE\"
echo build completed.
cd ../../asm-publish
del appsettings.development.json
REM del wwwroot directory
rd /Q /S wwwroot

if "%1" neq "nopause" (
    pause
)
