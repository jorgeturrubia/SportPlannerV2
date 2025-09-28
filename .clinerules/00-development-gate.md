# 00 - Development Quality Gate 🚪

## **MANDATORY: Pre-Development Context Review**

**Antes de escribir UNA sola línea de código**, debes tener el contexto completo del proyecto y asegurar compliance con TODAS las reglas.

Esta regla es el "policeman" que garantiza que revisas todas las otras reglas antes de cualquier cambio.

---

## **🔍 CONTEXT REVIEW CHECKLIST**

### **1. Project Architecture Review**
- [ ] **Leí `.clinerules\01-clean-code.md`** - Principios SOLID, DRY, responsabilidades claras
- [ ] **Leí `.clinerules\14-dotnet-backend.md`** - Clean Architecture, Vertical Slice, Entity Framework
- [ ] **Leí `.clinerules\10-angular-structure.md`** - Standalone components, feature-based structure
- [ ] **Leí `.clinerules\03-adr.md`** - Cuando crear ADRs y template

### **2. Security & Authentication Review**
- [ ] **Leí `.clinerules\13-supabase-jwt.md`** - JWT validation, JWKS, role-based access
- [ ] **Leí `.clinerules\05-security.md`** - Autenticación, validación de entrada, gestión de secrets
- [ ] **Confirmé NO usar custom JWT** - Solo Supabase Auth con validation correcta
- [ ] **Validé issuer/audience** - Debe ser Supabase, no local

### **3. Code Quality Standards Review**
- [ ] **Leí `.clinerules\02-naming.md`** - PascalCase, camelCase, kebab-case según contexto
- [ ] **Leí `.clinerules\06-tool-usage.md`** - Comandos largos en background, procesos async
- [ ] **Validé imports structure** - Por feature, no global
- [ ] **Confirmé no mixins UI+logic** - Separación clara

### **4. Testing Strategy Review**
- [ ] **Leí `.clinerules\04-testing.md`** - Coverage 80%, AAA pattern, mocks strategy
- [ ] **Planifiqué tests** - Unit, Integration, E2E según alcance
- [ ] **Setup de mocks** - Supabase, localStorage, HTTP calls

### **5. UI/UX Standards Review**
- [ ] **Leí `.clinerules\10-angular-structure.md`** - Component communication patterns
- [ ] **Leí `.clinerules\12-material-animations.md`** - Transition patterns
- [ ] **Leí `.clinerules\11-tailwind.md`** - Utility classes, responsive design
- [ ] **Leí `.clinerules\15-ui-ux-excellence.md`** - User experience principles

---

## **🎯 DECISION TREE: ¿PUEDO EMPEZAR A CODIFICAR?**

### **SI es cambio menor** (bugfix, typo, refactor internal)
- [ ] Afecta arquitectura? → No
- [ ] Afecta security? → No
- [ ] Afecta UI/UX patterns? → No
- [ ] → **POK** empezar directamente

### **SI es feature nueva o cambio significativo**
- [ ] Completé checklist arriba? → Sí
- [ ] Requiere ADR? (`.clinerules\03-adr.md`) → Si sí, crear ADR primero
- [ ] Afecta múltiples capas? → Pair programming
- [ ] → **AHORA SÍ** puedes empezar a codificar

---

## **🚨 BLOCKERS: NO PUEDES CONTINUAR SI**

### **Architecture Violations**
- ❌ Usar NgModules en lugar de standalone components
- ❌ Mezclar UI+logic en mismo componente
- ❌ No seguir vertical slice architecture
- ❌ Custom JWT sin Supabase validation

### **Security Violations**
- ❌ Almacenar credenciales no seguras
- ❌ Usar localStorage para tokens sensibles
- ❌ No validar JWT con JWKS endpoint
- ❌ Exposición de sensitive data

### **Code Quality Violations**
- ❌ Naming conventions incorrectas
- ❌ No separar responsabilidades
- ❌ Tests faltantes o incompletos
- ❌ No documentation para APIs públicas

---

## **🔧 AUTOMATION TOOLS REQUIRED**

Para enforzar esta regla, crea:
1. **Pre-commit hook** que requiera confirmation de checklist
2. **IDE extension** que muestre toast "Revisaste las reglas?"
3. **PR template** que incluya este checklist
4. **Build validation** que falle sin reference a compliance

---

## **📝 DEVELOPMENT WORKFLOW**

```
1. Leer todas las .clinerules/ relevantes
2. Completar checklist arriba
3. Si aplica → Crear ADR en docs/adr/
4. Si inseguro → Pair programming session
5. Implementar con TDD: Tests → Code → Refactor
6. Pre-commit: Validar checklist completado
7. PR: Incluir link a ADR si aplica
8. Code Review: Validar compliance con todas las reglas
```

---

## **🎖️ REWARD FOR COMPLIANCE**

- 🚀 **Faster development** - No rework por violations
- 🛡️ **Fewer security issues** - Rules probadas en batalla
- 👥 **Better collaboration** - Estándares consistentes
- 🏆 **Professional code** - Orgullo de craftsmanship

**Recuerda:** Esta regla existe porque violar las otras reglas cuesta tiempo y dinero. ¡Sé profesional, sigue las reglas! 💪
