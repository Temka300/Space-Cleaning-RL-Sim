# & $python $runner $config "--run-id=$RunId" "--force"

param(
    [string]$RunId = "space_cleaner_custom_ppo_v1"
)

$ErrorActionPreference = "Stop"

$projectRoot = Split-Path -Parent $PSScriptRoot
$python = Join-Path $projectRoot ".venv\Scripts\python.exe"
$config = Join-Path $projectRoot "config\space_cleaner_custom_ppo.yaml"
$runner = Join-Path $projectRoot "tools\run_space_cleaner_custom_ppo.py"

if (-not (Test-Path $python)) {
    throw "Python virtual environment not found at $python"
}

& $python $runner $config "--run-id=$RunId"
