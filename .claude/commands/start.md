# Reiniciar servicios de desarrollo

Reinicia los servicios de frontend (Angular) y backend (.NET) de SportPlannerV2.

**Proceso automático:**
1. ✋ Detener procesos existentes de Angular (node) y .NET (dotnet)
2. 🧹 Limpiar puertos ocupados (4200 para Angular, 5000/5001 para .NET)
3. 🎨 Iniciar frontend (Angular dev server en http://localhost:4200)
4. ⚙️ Iniciar backend (.NET API en https://localhost:5001)
5. ✅ Verificar que ambos servicios responden

**Instrucciones:**

Detecta el sistema operativo y ejecuta el script correspondiente:

**Windows (PowerShell):**
```powershell
./scripts/restart-services.ps1
```

**Linux/macOS (Bash):**
```bash
./scripts/restart-services.sh
```

**El script hace:**
- Detecta y mata procesos de `node` (Angular) y `dotnet` (Backend)
- Libera puertos 4200, 5000, 5001 si están ocupados
- Inicia `npm start` en `front/SportPlanner/`
- Inicia `dotnet run` en `back/SportPlanner/src/SportPlanner.API/`
- En Windows: abre ventanas separadas para cada servicio
- En Linux/macOS: ejecuta en background y genera logs en `/tmp/`

**Resultado esperado:**
- Frontend corriendo en http://localhost:4200
- Backend corriendo en https://localhost:5001
- Ambos servicios verificados y respondiendo
