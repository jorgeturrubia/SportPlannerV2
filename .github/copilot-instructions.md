<!-- Short, actionable instructions for AI coding agents working on SportPlannerV2. Keep it concise. -->
# Copilot / AI Agent Instructions — SportPlannerV2

Before editing code
- Read `.clinerules/00-development-gate.md`. Do not modify files under `.clinerules/` or `docs/adr/` without human approval.

Big picture (quick)
- Frontend: Angular 20 (standalone components, Signals, Tailwind v4) at `front/SportPlanner`.
- Backend: .NET 8 Clean Architecture at `back/SportPlanner/src` (Domain → Application → Infrastructure → API).
- Auth: Supabase JWT using Authority/JWKS in `back/SportPlanner/src/SportPlanner.API/Program.cs` — do not replace with custom JWT logic.

Fast commands (run from `src/`)
- Frontend dev: `cd front/SportPlanner` → `npm start` (or `ng serve`).
- Frontend tests/build: `npm test`, `npm run build`, `npm run watch`.
- Backend: `cd back/SportPlanner` → `dotnet build`, `dotnet run --project src/SportPlanner.API`, `dotnet test`.
- EF migrations: run from `back/SportPlanner`:
  dotnet ef migrations add <Name> --project src/SportPlanner.Infrastructure --startup-project src/SportPlanner.API

Concrete conventions to follow
- No NgModules. Use `loadComponent` for route-level lazy loading. Example: feature folder with `components/`, `pages/`, `services/`, and `X.routes.ts`.
- Use Angular `inject()` for DI in components/services (not constructor injection). E.g. `const ns = inject(NotificationService);`.
- Prefer Signals for local reactive state; services for shared state.
 - MANDATORY: Use Angular Signals as the default reactive mechanism across the frontend. Avoid using Subjects/BehaviorSubjects as primary state holders unless documented in an ADR.
- Backend: implement MediatR-style commands/queries in `SportPlanner.Application`; keep EF Core access inside `SportPlanner.Infrastructure`.

Integration & sensitive areas
- Supabase: frontend uses `@supabase/supabase-js`; backend authenticates via Authority/JWKS. Secrets and keys live in environment/appsettings — do not commit.
- Database: PostgreSQL via EF Core; migrations under `SportPlanner.Infrastructure/Migrations`.

Project-specific note: global notifications
- Files: `front/SportPlanner/src/app/shared/notifications/notification.model.ts`, `notification.service.ts`, `notifications.container.ts`.
- Usage: inject `NotificationService` and call `ns.success(msg, title)` or `ns.error(msg, title)`. The global container is included in the root template (`app.html`).

Quality & safety
- Pre-commit hooks run `.clinerules/validate-quality-gate.js` (see `.husky/pre-commit`). Ensure checks pass before committing.
- Tests: add unit tests with new logic and follow `.clinerules/04-testing.md`. Backend Application/Domain aim for >80% coverage.

Where to look first (high-signal files)
- Frontend entry: `front/SportPlanner/package.json`, `front/SportPlanner/src/app/features/` (search `training`).
- Backend root: `back/SportPlanner/src/SportPlanner.API/Program.cs`, `back/SportPlanner/src/SportPlanner.Application/`.
- Infrastructure/EF: `back/SportPlanner/src/SportPlanner.Infrastructure/`.

If a change affects architecture, security, or public API: create an ADR in `docs/adr/` using `docs/adr/template.md` and request human review.

If anything is unclear (feature folder, migration impact, ADR requirement), ask before proceeding.
