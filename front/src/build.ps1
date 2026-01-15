# build.ps1
param(
    [string]$Arg1
)

# 等价于：if "%1" == "" ( CHCP 65001 & @echo off )
if ([string]::IsNullOrEmpty($Arg1)) {
    # 将控制台输出编码切到 UTF-8（类似 chcp 65001）
    [Console]::OutputEncoding = [System.Text.UTF8Encoding]::new($false)
    $OutputEncoding = [System.Text.UTF8Encoding]::new($false)
}

Write-Host "building..."

# 等价于：call gulp build
& gulp build
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

# 等价于：if "%1" neq "nopause" ( pause )
if ($Arg1 -ne "nopause") {
    Read-Host "Press Enter to continue"
}
