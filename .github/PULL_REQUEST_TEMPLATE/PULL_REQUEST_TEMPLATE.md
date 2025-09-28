## Description
Brief description of the changes made in this PR.

## Quality Gate Checklist ✅

**MANDATORY**: Confirm you completed the Development Quality Gate before starting development.

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

## Changes Made
- [ ] **ADR Created** (if applicable) - Link: #___
- [ ] **Tests Added/Updated** - Coverage >=80%
- [ ] **Documentation Updated** (if API changes)
- [ ] **Security Review** - No sensitive data exposure

## Testing
- [ ] **Unit Tests** passing
- [ ] **Integration Tests** passing
- [ ] **Manual Testing** completed
- [ ] **Cross-browser** tested

## Security Impact
- [ ] **No security changes** - Safe to merge
- [ ] **Security changes** - Extra review needed
- [ ] **Critical security** - Alert security team

## Breaking Changes
- [ ] **None** - Backward compatible
- [ ] **Minor** - Documentation updated
- [ ] **Major** - Migration guide needed

## Screenshots/UI Changes
<!-- Add screenshots for UI changes -->

## Deployment Notes
<!-- Any special deployment considerations -->

---

**By checking the Quality Gate checkbox above, I confirm I have reviewed ALL relevant .clinerules/ before making these changes.**

⚠️ **PR will be blocked if Quality Gate is not completed**
