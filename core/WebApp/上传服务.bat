CHCP 65001
@echo off

:: 接收文件名
echo Please enter service file name.
set /p file=

:: 先上传文件
call wsl -e ansible my_server -m copy -a "src=./%file% dest=/usr/lib/systemd/system/"

:: 开机启动
call wsl -e ansible my_server -m shell -a "systemctl daemon-reload"
call wsl -e ansible my_server -m shell -a "systemctl enable %file%"

echo ok!

pause
