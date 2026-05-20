# Implementation Plan: [FEATURE]

**Branch**: `[###-feature-name]` | **Date**: [DATE] | **Spec**: [link]

**Input**: Feature specification from `/specs/[###-feature-name]/spec.md`

**Note**: This template is filled in by the `/speckit-plan` command. See
`.specify/templates/plan-template.md` for the execution workflow.

## Summary

[Extract from feature spec: primary requirement + technical approach]

## Technical Context

**Language/Version**: .NET 10 LTS (backend), TypeScript (frontend)

**Primary Dependencies**: ASP.NET Core Web API, Npgsql, xUnit, React, Vite

**Storage**: PostgreSQL

**Testing**: xUnit unit tests, API integration tests, PostgreSQL-backed
repository tests where practical

**Target Platform**: Web application with ASP.NET Core backend and browser-based
frontend

**Project Type**: Full-stack web application

**Performance Goals**: Keep response behavior acceptable for interview-scale demo
usage; document any feature-specific constraints if they matter

**Constraints**: Raw SQL with Npgsql only; ProblemDetails-style errors; backend
authorization enforcement; explicit DTOs; no unnecessary enterprise patterns

**Scale/Scope**: Small interview MVP for a blog platform, not a full CMS

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- Test-first: Does each backend story identify the failing unit, integration,
  and repository tests that must exist before production code?
- Backend-first: Is backend API, business logic, persistence, and security work
  sequenced ahead of dependent frontend work?
- Architecture: Do planned changes preserve clear boundaries between Domain,
  Application, Infrastructure/Data, API, Tests, and Frontend?
- Data access: Is all persistence work limited to parameterized raw SQL with
  Npgsql in Infrastructure/Data only?
- Security: Are authentication, admin authorization, ownership rules, and
  protected endpoint rejection covered by planned tests?
- Scope: Does the plan stay within the approved MVP and avoid CMS-style
  expansions?
- API consistency: Will validation and failures use ProblemDetails-style
  responses, explicit DTOs, and OpenAPI derived from implementation?

## Project Structure

### Documentation (this feature)

```text
specs/[###-feature]/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
└── tasks.md
```

### Source Code (repository root)

```text
backend/
├── src/
│   ├── Domain/
│   ├── Application/
│   ├── Infrastructure/
│   └── Api/
└── tests/
    ├── Domain.Tests/
    ├── Application.Tests/
    ├── Api.IntegrationTests/
    └── Infrastructure.Tests/

frontend/
├── src/
│   ├── components/
│   ├── features/
│   ├── pages/
│   └── services/
└── tests/
```

**Structure Decision**: Keep the backend layered by responsibility and isolated
from the frontend. Any deviation from this structure must be justified in the
Complexity Tracking section.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| [e.g., extra abstraction or dependency] | [current need] | [why the constitution-compliant option was insufficient] |
