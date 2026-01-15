if "%1" == "" (
    CHCP 65001
    @echo off
)

call build.bat nopause

if "%1" neq "nopause" (
    echo Ready to release? 
    pause   
)

echo Please wait, file syncing, do not close this window.
call wsl -e ansible-playbook publish.yaml -v
echo published!

if "%1" neq "nopause" (
    pause
)