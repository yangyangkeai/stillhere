#requires -Version 5.1
[CmdletBinding()]
param(
    [switch]$NoPause
)

$ErrorActionPreference = 'Stop'

function Write-Step([string]$Message) {
    Write-Host $Message
}

function Get-EnvValue([string]$Name) {
    $v = [Environment]::GetEnvironmentVariable($Name, 'Process')
    if ([string]::IsNullOrEmpty($v)) { $v = [Environment]::GetEnvironmentVariable($Name, 'User') }
    if ([string]::IsNullOrEmpty($v)) { $v = [Environment]::GetEnvironmentVariable($Name, 'Machine') }
    return $v
}

try {
    Write-Step 'clear...'

    $scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
    Set-Location -Path $scriptDir

    $repoRoot = Resolve-Path (Join-Path $scriptDir '..\..')
    Set-Location -Path $repoRoot

    $publishDir = Join-Path $repoRoot 'core-publish'

    # 清理输出目录 core-publish（存在才删）
    if (Test-Path -LiteralPath $publishDir) {
        Remove-Item -LiteralPath $publishDir -Recurse -Force
    }

    # 确保输出目录存在（避免后续 Set-Location 报错）
    New-Item -ItemType Directory -Path $publishDir -Force | Out-Null

    # 回到 Core/WebApp 执行 publish
    Set-Location -Path (Join-Path $repoRoot 'Core\WebApp')

    Write-Step 'building...'
    dotnet publish `
        -o "$publishDir" `
        -r linux-x64 `
        -c Release `
        --self-contained `
        -p:DefineConstants="FORCE_LINUX%3BRELEASE"

    Write-Step 'build completed.'

    # 进入输出目录（此时必然存在）
    Set-Location -Path $publishDir

    # 删除 appsettings.development.json 和 wwwroot（存在才删）
    $devSettings = Join-Path $publishDir 'appsettings.development.json'
    if (Test-Path -LiteralPath $devSettings) {
        Remove-Item -LiteralPath $devSettings -Force
    }

    $wwwroot = Join-Path $publishDir 'wwwroot'
    if (Test-Path -LiteralPath $wwwroot) {
        Remove-Item -LiteralPath $wwwroot -Recurse -Force
    }

    # 读取并替换 appsettings.json 占位符为环境变量（文件存在才处理）
    $appsettings = Join-Path $publishDir 'appsettings.json'
    if (!(Test-Path -LiteralPath $appsettings)) {
        throw "Missing file: $appsettings"
    }

    $db  = Get-EnvValue 'P_ST_DB'
    $usr = Get-EnvValue 'P_ST_DB_USER'
    $pwd = Get-EnvValue 'P_ST_DB_PWD'

    if ([string]::IsNullOrEmpty($db))  { throw 'Env var P_ST_DB is not set.' }
    if ([string]::IsNullOrEmpty($usr)) { throw 'Env var P_ST_DB_USER is not set.' }
    if ([string]::IsNullOrEmpty($pwd)) { throw 'Env var P_ST_DB_PWD is not set.' }

    $content = [IO.File]::ReadAllText($appsettings, [Text.Encoding]::UTF8)
    $content = $content.Replace('${P_ST_DB}', $db).
            Replace('${P_ST_DB_USER}', $usr).
            Replace('${P_ST_DB_PWD}', $pwd)

    [IO.File]::WriteAllText($appsettings, $content, (New-Object Text.UTF8Encoding($false)))

    Write-Step 'appsettings.json patched from env vars.'

    if (-not $NoPause) {
        Read-Host 'Press Enter to continue...'
    }
}
catch {
    Write-Error $_
    if (-not $NoPause) {
        Read-Host 'Press Enter to continue...'
    }
    exit 1
}
