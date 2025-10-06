# Scripts de Automatización - SportPlannerV2

Scripts para automatizar tareas comunes de desarrollo.

---

## 📜 Scripts Disponibles

### 1. `restart-services` - Reiniciar Servicios de Desarrollo

Detiene y reinicia automáticamente los servicios de frontend (Angular) y backend (.NET).

#### **Uso:**

**Windows (PowerShell):**
```powershell
./restart-services.ps1
```

**Linux/macOS (Bash):**
```bash
./restart-services.sh
```

**Desde Claude Code:**
```
/start
```

#### **Lo que hace:**

1. **Detiene servicios existentes:**
   - Mata procesos de `node` (Angular dev server)
   - Mata procesos de `dotnet` (API backend)
   - Libera puertos 4200, 5000, 5001

2. **Inicia servicios frescos:**
   - Ejecuta `npm start` en `front/SportPlanner/`
   - Ejecuta `dotnet run` en `back/SportPlanner/src/SportPlanner.API/`

3. **Verifica disponibilidad:**
   - Comprueba que Angular responde en http://localhost:4200
   - Comprueba que .NET responde en https://localhost:5001

#### **Resultado:**

**Windows:**
- Abre 2 ventanas de PowerShell separadas
- Una para Angular (puerto 4200)
- Otra para .NET (puerto 5001)
- Cierra las ventanas para detener los servicios

**Linux/macOS:**
- Ejecuta ambos servicios en background
- Logs disponibles en:
  - `/tmp/angular-dev.log` (frontend)
  - `/tmp/dotnet-api.log` (backend)

#### **Ver logs en tiempo real (Linux/macOS):**

```bash
# Angular
tail -f /tmp/angular-dev.log

# .NET
tail -f /tmp/dotnet-api.log
```

#### **Detener servicios (Linux/macOS):**

```bash
# Opción 1: Usar PIDs del output del script
kill <ANGULAR_PID> <DOTNET_PID>

# Opción 2: Matar todos los procesos
pkill -f node
pkill -f dotnet
```

#### **Detener servicios (Windows):**

```powershell
# Cerrar las ventanas de PowerShell
# O desde el script:
Stop-Process -Name "node" -Force
Stop-Process -Name "dotnet" -Force
```

---

## 🔧 Troubleshooting

### Error: "Puerto ya en uso"

**Problema:** El script no pudo liberar los puertos automáticamente.

**Solución Windows:**
```powershell
# Ver qué proceso usa el puerto 4200
netstat -ano | findstr :4200

# Matar el proceso (reemplaza PID)
taskkill /PID <PID> /F
```

**Solución Linux/macOS:**
```bash
# Ver qué proceso usa el puerto 4200
lsof -ti:4200

# Matar el proceso
kill -9 $(lsof -ti:4200)
```

### Error: "npm not found" o "dotnet not found"

**Problema:** Node.js o .NET SDK no están instalados o no están en el PATH.

**Solución:**
- Instalar Node.js: https://nodejs.org/
- Instalar .NET 8 SDK: https://dotnet.microsoft.com/download/dotnet/8.0
- Verificar instalación:
  ```bash
  node --version
  npm --version
  dotnet --version
  ```

### Error: "node_modules not found"

**Problema:** Dependencias de Angular no están instaladas.

**Solución:**
```bash
cd front/SportPlanner
npm install
```

El script detecta esto automáticamente y ejecuta `npm install` si es necesario.

---

## 🆕 Crear Nuevos Scripts

Para añadir nuevos scripts de automatización:

1. **Crear el script** en esta carpeta:
   ```bash
   # Bash script
   touch scripts/my-script.sh
   chmod +x scripts/my-script.sh

   # PowerShell script
   New-Item scripts/my-script.ps1
   ```

2. **Documentarlo** en este README.

3. **Opcional:** Crear comando slash en `.claude/commands/`:
   ```bash
   # Crear archivo .claude/commands/my-command.md
   # Contenido: instrucciones para ejecutar tu script
   ```

---

## 📚 Scripts Futuros (Roadmap)

- `test-all.sh` - Ejecutar todos los tests (frontend + backend)
- `build-all.sh` - Build completo del proyecto
- `deploy.sh` - Despliegue automatizado
- `db-reset.sh` - Resetear base de datos de desarrollo
- `seed-data.sh` - Poblar base de datos con datos de prueba

---

**Última actualización:** 2025-10-06
