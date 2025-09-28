# Architecture Decision Records (ADRs)

This directory contains all Architecture Decision Records for the SportPlanner project.

## What are ADRs?

Architecture Decision Records (ADRs) are documents that capture important architectural decisions made during the development of a project. They provide context, reasoning, and consequences of architectural choices.

## When to create an ADR?

From `.clinerules\00-development-gate.md`, you MUST create an ADR when:

- **Changing architecture patterns** (e.g., switching from custom JWT to Supabase Auth)
- **Introducing new technologies** (e.g., new database, framework, library)
- **Major infrastructure changes** (e.g., cloud provider, deployment strategy)
- **Security architecture decisions** (e.g., authentication method, data encryption)
- **Breaking changes** that affect multiple teams/features

## ADR Template

Use the template in `template.md` for new ADRs.

## Naming Convention

ADRs follow the format: `YYYYMMDD-NNN-short-title.md`

Example: `20250928-001-supabase-auth-migration.md`

## Process

1. Before implementing architectural changes, create ADR
2. Review ADR with team
3. Implement only after ADR approval
4. Reference ADR in PR description with link

## Categories

| Category | Description |
|----------|-------------|
| `ARCH` | Architecture patterns, frameworks |
| `SEC` | Security decisions |
| `DB` | Database, data persistence |
| `INFRA` | Infrastructure, deployment |
| `UX` | UI/UX architectural decisions |

## Current ADRs

| ID | Title | Date | Status |
|----|-------|------|--------|
| 000 | Template | N/A | Template |

## Resources

- [ADR Inspiration](https://adr.github.io/)
- [Spotify Tech Blog](https://engineering.atspotify.com/2020/04/14/when-should-i-write-an-architecture-decision-record/)
