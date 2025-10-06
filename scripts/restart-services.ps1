# SportPlannerV2 - Service Restart Script (PowerShell)
# Detiene y reinicia servicios de Frontend (Angular) y Backend (.NET)

# Configuración
$ErrorActionPreference = "Stop"
$ProjectRoot = Split-Path -Parent $PSScriptRoot
$FrontendDir = Join-Path $ProjectRoot "front\SportPlanner"
$BackendDir = Join-Path $ProjectRoot "back\SportPlanner\src\SportPlanner.API"

# Puertos
$FrontendPort = 4200
$BackendHttpPort = 5000
$BackendHttpsPort = 5001

# Colores
function Write-ColorOutput($ForegroundColor) {
    $fc = $host.UI.RawUI.ForegroundColor
    $host.UI.RawUI.ForegroundColor = $ForegroundColor
    if ($args) {
        Write-Output $args
    }
    $host.UI.RawUI.ForegroundColor = $fc
}

Write-Host "╔════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║  SportPlannerV2 - Service Restart Script              ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host ""

# Función: Matar proceso por puerto
function Stop-ProcessByPort {
    param([int]$Port, [string]$ProcessName)

    Write-Host "🔍 Checking port $Port for $ProcessName..." -ForegroundColor Yellow

    $process = Get-NetTCPConnection -LocalPort $Port -ErrorAction SilentlyContinue | Select-Object -ExpandProperty OwningProcess -ErrorAction SilentlyContinue

    if ($process) {
        Write-Host "   Killing process $process on port $Port" -ForegroundColor Yellow
        Stop-Process -Id $process -Force -ErrorAction SilentlyContinue
        Start-Sleep -Seconds 1
    } else {
        Write-Host "   ✓ Port $Port is free" -ForegroundColor Green
    }
}

# Función: Matar procesos por nombre
function Stop-ProcessByName {
    param([string]$ProcessName)

    Write-Host "🔍 Checking for running '$ProcessName' processes..." -ForegroundColor Yellow

    $processes = Get-Process -Name $ProcessName -ErrorAction SilentlyContinue

    if ($processes) {
        $processes | Stop-Process -Force
        Write-Host "   ✓ Killed $ProcessName processes" -ForegroundColor Green
    } else {
        Write-Host "   ✓ No $ProcessName processes found" -ForegroundColor Green
    }
}

# Step 1: Detener servicios existentes
Write-Host ""
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
Write-Host "  Step 1: Stopping existing services" -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan

# Matar procesos Node (Angular)
Stop-ProcessByName "node"
Stop-ProcessByPort $FrontendPort "Angular"

# Matar procesos .NET
Stop-ProcessByName "dotnet"
Stop-ProcessByPort $BackendHttpPort ".NET HTTP"
Stop-ProcessByPort $BackendHttpsPort ".NET HTTPS"

Start-Sleep -Seconds 2

# Step 2: Iniciar Frontend (Angular)
Write-Host ""
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
Write-Host "  Step 2: Starting Frontend (Angular)" -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan

if (-Not (Test-Path $FrontendDir)) {
    Write-Host "❌ Frontend directory not found: $FrontendDir" -ForegroundColor Red
    exit 1
}

Set-Location $FrontendDir

# Verificar node_modules
if (-Not (Test-Path "node_modules")) {
    Write-Host "⚠️  node_modules not found. Running npm install..." -ForegroundColor Yellow
    npm install
}

Write-Host "🚀 Starting Angular dev server (http://localhost:4200)" -ForegroundColor Green
Write-Host "   Running: npm start" -ForegroundColor Yellow

# Iniciar Angular en nueva ventana
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$FrontendDir'; npm start" -WindowStyle Normal

Write-Host "   ✓ Angular dev server started in new window" -ForegroundColor Green

# Step 3: Iniciar Backend (.NET)
Write-Host ""
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
Write-Host "  Step 3: Starting Backend (.NET API)" -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan

if (-Not (Test-Path $BackendDir)) {
    Write-Host "❌ Backend API directory not found: $BackendDir" -ForegroundColor Red
    exit 1
}

Set-Location $BackendDir

Write-Host "🚀 Starting .NET API (https://localhost:5001)" -ForegroundColor Green
Write-Host "   Running: dotnet run" -ForegroundColor Yellow

# Iniciar .NET en nueva ventana
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$BackendDir'; dotnet run" -WindowStyle Normal

Write-Host "   ✓ .NET API started in new window" -ForegroundColor Green

# Step 4: Esperar y verificar
Write-Host ""
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
Write-Host "  Step 4: Waiting for services to be ready" -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan

Write-Host "⏳ Waiting 10 seconds for services to start..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

# Verificar Angular
Write-Host "🔍 Checking Angular dev server..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:4200" -TimeoutSec 5 -ErrorAction SilentlyContinue
    Write-Host "   ✓ Angular is responding on http://localhost:4200" -ForegroundColor Green
} catch {
    Write-Host "   ⚠️  Angular might still be starting" -ForegroundColor Yellow
}

# Verificar .NET
Write-Host "🔍 Checking .NET API..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "https://localhost:5001" -SkipCertificateCheck -TimeoutSec 5 -ErrorAction SilentlyContinue
    Write-Host "   ✓ .NET API is responding on https://localhost:5001" -ForegroundColor Green
} catch {
    Write-Host "   ⚠️  .NET API might still be starting" -ForegroundColor Yellow
}

# Resumen final
Write-Host ""
Write-Host "╔════════════════════════════════════════════════════════╗" -ForegroundColor Green
Write-Host "║  ✅ Services Started Successfully                      ║" -ForegroundColor Green
Write-Host "╚════════════════════════════════════════════════════════╝" -ForegroundColor Green
Write-Host ""
Write-Host "📍 Frontend (Angular):  " -NoNewline -ForegroundColor Cyan
Write-Host "http://localhost:4200" -ForegroundColor White
Write-Host "📍 Backend (.NET):     " -NoNewline -ForegroundColor Cyan
Write-Host "https://localhost:5001" -ForegroundColor White
Write-Host ""
Write-Host "💡 Both services are running in separate PowerShell windows" -ForegroundColor Yellow
Write-Host "   Close those windows or press Ctrl+C to stop the services" -ForegroundColor Yellow
Write-Host ""
Write-Host "✨ Happy coding!" -ForegroundColor Green
Write-Host ""

# Volver al directorio original
Set-Location $ProjectRoot
