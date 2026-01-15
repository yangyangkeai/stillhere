# publish.ps1
param(
    [string]$Arg1
)

# 如果未传参，则切换控制台到 UTF-8（等价于 chcp 65001 的目的：保证中文/UTF-8 输出）
if ([string]::IsNullOrEmpty($Arg1)) {
    [Console]::OutputEncoding = [System.Text.UTF8Encoding]::new($false)
    $OutputEncoding = [System.Text.UTF8Encoding]::new($false)
}

# 改为调用 build.ps1，并传递 nopause
& "$PSScriptRoot\build.ps1" "nopause"
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

# 保留 nopause 行为
if ($Arg1 -ne "nopause") {
    Write-Host "Ready to release?"
    Read-Host "Press Enter to continue"
}

Write-Host "Please wait, file syncing, do not close this window."

& wsl -e ansible-playbook publish.yaml -v
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "published!"

if ($Arg1 -ne "nopause") {
    Read-Host "Press Enter to continue"
}
