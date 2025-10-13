# 🤖 GitHub Copilot Instructions — SportPlannerV2

> **Purpose**: This file redirects AI agents to the comprehensive instruction files organized by scope.

---

## 📍 Navigation Guide

SportPlannerV2 uses a **three-tier agent instruction system**:

### 1️⃣ **General Project Instructions** (START HERE)
📄 **File**: [`AGENTS.md`](../AGENTS.md) (root: `C:\Proyectos\SportPlannerV2\src\AGENTS.md`)

**Read this FIRST for:**
- 🔄 Complete workflow methodology (Deep Thinking → Planning → Implementation → Verification → Completion)
- 🚨 Mandatory quality gate checklist
- 🛡️ Security standards (Supabase JWT, secrets management, OWASP compliance)
- 📋 Naming conventions (files, classes, variables, database)
- 🏗️ Architecture Decision Records (ADR) process
- ⚡ Common commands and project structure
- 🎯 Where to find things in the codebase

**This file contains:**
- Cross-cutting concerns for the entire project
- Quality standards and best practices
- Decision-making frameworks
- Error handling procedures
- Testing standards

---

### 2️⃣ **Frontend-Specific Instructions**
📄 **File**: [`front/SportPlanner/AGENTS.md`](../front/SportPlanner/AGENTS.md)

**Read this when working on:**
- Angular 20 components (standalone, no NgModules)
- Tailwind CSS styling (**NO Angular Material**)
- Angular Signals for state management
- Supabase client integration
- Frontend routing and lazy loading
- UI/UX patterns and component library

**This file contains:**
- Angular-specific patterns and conventions
- Tailwind component library (buttons, forms, modals, tables)
- Signal-based state management patterns
- Frontend testing strategies
- Component communication patterns

---

### 3️⃣ **Backend-Specific Instructions**
📄 **File**: [`back/SportPlanner/AGENTS.md`](../back/SportPlanner/AGENTS.md)

**Read this when working on:**
- .NET 8 Clean Architecture (Domain → Application → Infrastructure → API)
- MediatR commands/queries (CQRS pattern)
- Entity Framework Core and migrations
- Supabase JWT validation (Authority/JWKS)
- Repository pattern and domain entities
- Backend testing (Unit, Integration)

**This file contains:**
- Clean Architecture layer responsibilities
- Domain-driven design patterns
- EF Core best practices and migration workflows
- Backend validation and error handling
- Testing patterns (AAA, mocking strategies)

---

## 🚀 Quick Start Workflow

**When you receive a task:**

1. **🧠 Read [`AGENTS.md`](../AGENTS.md)** (general instructions)
   - Understand the mandatory workflow (5 phases)
   - Check if you need an ADR
   - Review security/quality standards

2. **🎯 Identify scope** and read the specific agent file:
   - Frontend work? → Read [`front/SportPlanner/AGENTS.md`](../front/SportPlanner/AGENTS.md)
   - Backend work? → Read [`back/SportPlanner/AGENTS.md`](../back/SportPlanner/AGENTS.md)
   - Full-stack? → Read both

3. **📝 Create a plan** using TodoWrite (see workflow in AGENTS.md)

4. **🔨 Implement** following the patterns in the relevant agent file

5. **✅ Verify** (Lint → Build → Tests → Self-review)

---

## ⚡ Ultra-Quick Reference

**Technology Stack:**
- **Frontend**: Angular 20 + Tailwind CSS v4 + Signals + Supabase client
- **Backend**: .NET 8 + EF Core + PostgreSQL + Supabase JWT (Authority/JWKS)
- **Auth**: Supabase (JWT tokens, JWKS validation)

**Critical Rules:**
- ❌ **NO Angular Material** — Use Tailwind CSS only
- ❌ **NO custom JWT logic** — Use Supabase Authority/JWKS
- ❌ **NO hardcoded secrets** — Use environment variables
- ✅ **Use Angular Signals** for reactive state (not BehaviorSubject)
- ✅ **Follow Clean Architecture** layers (Domain → Application → Infrastructure → API)
- ✅ **Create ADRs** for architectural/security changes

**Fast Commands:**
```bash
# Frontend (from src/)
cd front/SportPlanner
npm start              # Dev server (localhost:4200)
npm run build          # Production build
npm test               # Run tests
npm run lint           # ESLint

# Backend (from src/)
cd back/SportPlanner
dotnet build           # Build solution
dotnet test            # Run tests
dotnet run --project src/SportPlanner.API  # Start API

# EF Migrations (from back/SportPlanner/)
dotnet ef migrations add <Name> --project src/SportPlanner.Infrastructure --startup-project src/SportPlanner.API
dotnet ef database update --project src/SportPlanner.Infrastructure --startup-project src/SportPlanner.API
```

---

## 📚 Additional Resources

- **ADR Template**: [`docs/adr/template.md`](../docs/adr/template.md)
- **Training System Spec**: [`docs/training-system-complete-specification.md`](../docs/training-system-complete-specification.md)
- **ADR Index**: [`docs/adr/README.md`](../docs/adr/README.md)

---

## 🛑 Protected Files (Human Approval Required)

**NEVER modify these without explicit human approval:**
- `AGENTS.md`, `front/SportPlanner/AGENTS.md`, `back/SportPlanner/AGENTS.md`
- `docs/adr/**` (Architecture Decision Records)
- `.github/workflows/**` (CI/CD pipelines)
- `.husky/**` (Git hooks)

---

**Last Updated**: 2025-10-13  
**Status**: ✅ Active redirection file  
**Version**: 3.0 (Three-tier agent system)
