#requires -Version 5.1
[CmdletBinding()]
param(
    [switch]$NoPause
)

$ErrorActionPreference = 'Stop'

function Pause-IfNeeded {
    param([switch]$NoPause)
    if (-not $NoPause) { Read-Host 'Press Enter to continue...' }
}

try {
    if (-not $NoPause) {
        # 等价于 CHCP 65001：尽量用 UTF-8 输出
        try { [Console]::OutputEncoding = [System.Text.UTF8Encoding]::new($false) } catch {}
    }

    $scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
    Set-Location -Path $scriptDir

    # call build.bat nopause => 调用 build.ps1 并禁用暂停
    $buildPs1 = Join-Path $scriptDir 'build.ps1'
    if (-not (Test-Path -LiteralPath $buildPs1)) {
        throw "Missing file: $buildPs1"
    }

    & $buildPs1 -NoPause

    # rd /Q /S wwwroot
    $wwwroot = Join-Path $scriptDir 'wwwroot'
    if (Test-Path -LiteralPath $wwwroot) {
        Remove-Item -LiteralPath $wwwroot -Recurse -Force
    }

    if (-not $NoPause) {
        Write-Host 'Ready to release?'
        Pause-IfNeeded -NoPause:$NoPause
    }

    Write-Host 'Please wait, file syncing, do not close this window.'

    # call wsl -e ansible-playbook publish.yaml -v
    $publishYaml = Join-Path $scriptDir 'publish.yaml'
    if (-not (Test-Path -LiteralPath $publishYaml)) {
        throw "Missing file: $publishYaml"
    }

    & wsl -e ansible-playbook publish.yaml -v
    if ($LASTEXITCODE -ne 0) {
        throw "WSL/ansible-playbook failed. ExitCode=$LASTEXITCODE"
    }

    Write-Host 'published!'

    Pause-IfNeeded -NoPause:$NoPause
}
catch {
    Write-Error $_
    Pause-IfNeeded -NoPause:$NoPause
    exit 1
}
