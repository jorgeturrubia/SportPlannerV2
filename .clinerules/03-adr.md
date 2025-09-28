# Architecture Decision Records (ADR)

## Estructura de ADR
Cada decisión arquitectónica relevante se documenta en `docs/adr/` con el formato:

```
YYYYMMDD-número-título-decisión.md
```

## Template de ADR
```markdown
# ADR-XXX: Título de la Decisión

**Fecha**: YYYY-MM-DD
**Estado**: [Propuesta|Aceptada|Rechazada|Deprecada|Superseded by ADR-YYY]
**Contexto**: Sprint/Feature/Issue relacionado

## Contexto
Descripción del problema o situación que requiere una decisión.

## Opciones Consideradas
1. **Opción A**: Descripción, pros, contras
2. **Opción B**: Descripción, pros, contras
3. **Opción C**: Descripción, pros, contras

## Decisión
Opción elegida y justificación.

## Consecuencias
### Positivas
- Beneficio 1
- Beneficio 2

### Negativas
- Riesgo/limitación 1
- Riesgo/limitación 2

## Referencias
- Issue: #123
- PR: #456
- Documentación: enlace
```

## Decisiones que Requieren ADR
- Elección de tecnologías principales
- Patrones arquitectónicos
- Estructuras de datos críticas
- Integraciones con servicios externos
- Decisiones de seguridad
- Cambios que afecten múltiples features

## Gestión de ADRs
- Revisar ADRs en code reviews
- Actualizar estado cuando cambien las circunstancias
- Referenciar ADRs en commits relacionados
- Mantener índice actualizado en `docs/adr/README.md`

## ADRs Específicos para SportPlanner
- Arquitectura de microservicios vs monolito
- Gestión de estado (signals vs services)
- Estrategia de autenticación y autorización
- Estructura de base de datos para entrenamientos
- Integración con dispositivos wearables
