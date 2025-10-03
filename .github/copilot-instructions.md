<!--
Short, actionable instructions for AI coding agents working on SportPlannerV2.
Keep this file small — prefer concrete, discoverable rules and links to source files.
-->
# Copilot / AI Agent Instructions — SportPlannerV2

Before making any code changes, read and respect the project's quality gate and rules in `.clinerules/` (see `.clinerules/00-development-gate.md`). That checklist is mandatory and enforced by pre-commit hooks.

What this project is (big picture)
- Full-stack app: Angular 20 frontend (`front/SportPlanner`) + .NET 8 backend (`back/SportPlanner`).
- Clean Architecture on backend (Domain → Application → Infrastructure → API). See `back/SportPlanner/src/SportPlanner.API/Program.cs` for DI & auth wiring.
- Frontend uses standalone Angular components, feature-based folders, Angular Material + Tailwind. See `front/SportPlanner/package.json` and `src/app/features/` for examples.

Quick commands (run from `src/`):
- Frontend dev: `cd front/SportPlanner` then `npm start` (or `ng serve`).
- Frontend build/watch/tests: `npm run build`, `npm run watch`, `npm test`.
- Backend build/run/tests: `cd back/SportPlanner` then `dotnet build`, `dotnet run --project src/SportPlanner.API`, `dotnet test`.
- EF migrations: use `dotnet ef` against `src/SportPlanner.Infrastructure` with `--startup-project src/SportPlanner.API`.

Project-specific conventions the agent must follow
- ALWAYS read `.clinerules/*` relevant files before changes (security, naming, angular structure, testing). The PR template requires this.
- Frontend: No NgModules — use standalone components and lazy-load with `loadComponent`. Prefer Signals for local reactive state and services for shared state.
  - Example pattern: feature folder with `components/`, `pages/`, `services/`, and a `*.routes.ts` that uses `loadComponent`.
- Backend: Clean Architecture + Vertical Slice. Use MediatR-style commands/queries in Application layer and keep EF usage confined to Infrastructure.
- Authentication: Supabase JWT (Authority + Audience). Do NOT implement custom JWT validation. See `Program.cs` for JWT setup and `.clinerules/13-supabase-jwt.md` for rules.

Integration points to be careful with
- Supabase: frontend uses `@supabase/supabase-js`; backend validates JWT using Supabase Authority (JWKS). Secrets and URLs live in configuration — do not commit secrets.
- Database: PostgreSQL via EF Core. Migrations live in `SportPlanner.Infrastructure/Migrations`.

Safe edit rules for agents
- Never change `.clinerules/` files or `docs/adr/` without human approval — those are governance artifacts.
- When adding migrations: create migration in `SportPlanner.Infrastructure` and update `docs/adr/` if it changes public schema/behavior.
- Tests: add unit tests alongside new logic (follow `.clinerules/04-testing.md`). Backend Application/Domain aim for >80% coverage.

Where to look for examples (high-signal files)
- Frontend entry & scripts: `front/SportPlanner/package.json`
- Frontend feature example: `front/SportPlanner/src/app/features/` (search for `training` feature)
- Backend composition/root: `back/SportPlanner/src/SportPlanner.API/Program.cs`
- Infrastructure & EF: `back/SportPlanner/src/SportPlanner.Infrastructure/`
- Quality rules & checklist: `.clinerules/` and pre-commit hook at `.husky/pre-commit` which runs `.clinerules/validate-quality-gate.js`.

Common pitfalls for automated edits
- Do not start long-running dev servers in blocking mode. See `.clinerules/06-tool-usage.md` for background process guidance.
- Avoid editing localization or secrets files without context (e.g., `public/assets/i18n/` or `appsettings.*.json`).

If unsure or the change affects architecture, security, or public API:
- Create an ADR in `docs/adr/` and stop; request human review.

Sign-off and follow-up
- After applying changes, run the relevant build & tests locally (frontend & backend), and ensure `.clinerules/validate-quality-gate.js` passes.
- Leave a short comment in the PR describing which `.clinerules` entries you validated.

Questions or incomplete areas: ask which feature folder and whether an ADR is required before continuing.
