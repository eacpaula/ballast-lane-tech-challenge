# Implementation Plan: Prepare API for Frontend Integration

**Branch**: `011-frontend-api-prep` | **Date**: 2026-05-21 | **Spec**: `specs/011-frontend-api-prep/spec.md`

**Input**: Feature specification from `/specs/011-frontend-api-prep/spec.md`

**Note**: This plan focuses on backend readiness for the first React
integration slice. It covers local API container startup, API-to-database
container connectivity, local CORS, one new read-only category listing use
case, and the tests and documentation needed to support frontend work.

## Summary

Prepare the existing backend API for local frontend consumption without adding
new business scope.

- Start with failing backend tests for the new category-listing behavior:
  Application tests for the read use case, API integration tests for the HTTP
  contract, and a PostgreSQL-backed repository test for available-category
  filtering.
- Add only the minimum backend types required to support the frontend form
  dependency: a small Application query/handler, one repository read method,
  one public read-only API endpoint, and the DTOs required for the endpoint.
- Extend local runtime support with an API Docker service, container-safe
  environment configuration, and local-only CORS for the React development
  origin.
- Keep administrator category create/update/deactivate behavior unchanged and
  protected; the new endpoint is read-only and must not collapse write and read
  responsibilities into one broad management surface.

## Implementation Design

**1. Technical approach**:

- Treat this as a backend integration-readiness slice, not a frontend feature.
- Add one small Application-level category listing use case that returns only
  available categories for post assignment.
- Expose that use case through a thin public API endpoint with explicit DTOs.
- Extend local operations support so the API can run through Docker Compose
  against the PostgreSQL container and still expose Swagger for manual
  exploration.

**2. Layers affected**:

- `docker-compose.yml`
- root environment examples such as `.env.example`
- `src/backend/BlogPlatform.Api`
- `src/backend/BlogPlatform.Application`
- `src/backend/BlogPlatform.Infrastructure`
- `tests/backend/BlogPlatform.Application.Tests`
- `tests/backend/BlogPlatform.Infrastructure.Tests`
- `tests/backend/BlogPlatform.Api.Tests`
- `README.md` and supporting backend run documentation
- no planned changes to `src/frontend/blog-web`

**3. Docker Compose strategy**:

- Add a dedicated API service to `docker-compose.yml`.
- Build the API service from a repository-local Dockerfile instead of relying
  on host-installed runtime state.
- Put the API and PostgreSQL services on the default Compose network so the API
  can use the service name `postgres` as the database host.
- Expose one host port for local browser/API usage and keep Swagger reachable
  through that port.
- Keep the container definition intentionally small: build context, environment,
  service dependency, and port exposure only.

**4. API configuration strategy**:

- Continue to use `Program.cs` as the explicit composition root.
- Bind API runtime settings from configuration/environment without introducing
  a new configuration framework.
- Add local CORS configuration in the API layer only.
- Preserve thin controllers and explicit DTOs; the controller for category
  listing should delegate to an Application handler rather than reading from a
  repository directly.
- Keep existing admin category write endpoints unchanged in behavior and
  protection.

**5. Database connection strategy**:

- Reuse the existing Infrastructure connection settings shape.
- Extend configuration defaults so host-based local execution can still use
  `localhost`, while containerized API execution can override the database host
  to `postgres`.
- Keep all database access behind Infrastructure dependency injection and
  existing repository abstractions.
- Extend `ICategoryRepository` with one read method for available categories
  rather than bypassing the repository from API code.

**6. CORS strategy**:

- Allow only the local frontend development origins needed for Vite-based local
  development.
- Prefer configuration-driven local origins so the same code path works for
  host-based and container-based API startup.
- Keep the policy narrow and explicit; do not introduce permissive wildcard
  development CORS as the default.

**7. Category listing endpoint strategy**:

- Add a new public read-only endpoint dedicated to frontend form data, most
  likely `GET /api/categories/available`.
- Implement a small Application query/handler pair that returns only available
  categories and no management behavior.
- Add an explicit response DTO that contains only category selection data
  needed by the frontend form.
- Keep category create/update/deactivate on the existing administrator-only
  endpoints and reuse current authorization boundaries unchanged.

**8. Testing strategy**:

- Follow test-first sequencing:
  - Application test proving the category listing use case returns only
    available categories.
  - Repository integration test proving the raw SQL category query filters to
    available categories only.
  - API integration tests proving anonymous access, response shape, and
    exclusion of unavailable categories.
  - Manual verification of Docker Compose startup and Swagger reachability.
- Re-run existing admin category authorization tests to ensure write protection
  remains intact after the new public read endpoint is added.

**9. README/documentation update strategy**:

- Extend `README.md` with the local Compose-based backend startup flow,
  including the API service.
- Document the local API URL, Swagger URL, and the new category-list endpoint
  for frontend consumption.
- Clarify why the category-list endpoint is public and why category write
  operations remain administrator-only.
- Keep documentation limited to local development and interview demo usage.

**10. Risks and trade-offs**:

- Adding the API service to Compose introduces one more runtime artifact, but
  it removes local startup ambiguity before frontend work begins.
- A dedicated available-categories endpoint is slightly narrower than a generic
  `GET /api/categories`, but that narrowness keeps intent explicit and reduces
  confusion between public reads and admin management.
- Configuration-driven CORS adds a small amount of setup surface, but it avoids
  permissive development defaults.
- Deferring tag listing keeps the slice small; if the first frontend form later
  proves it needs tags immediately, that can be added as a separate small
  follow-up feature.

## Technical Context

**Language/Version**: .NET 10 LTS

**Primary Dependencies**: ASP.NET Core Web API, Npgsql, xUnit, Docker Compose,
existing raw SQL Infrastructure repositories

**Storage**: PostgreSQL

**Testing**: xUnit Application tests, API integration tests, and
PostgreSQL-backed repository tests where practical

**Target Platform**: ASP.NET Core backend consumed by a local React/Vite
frontend in browser-based development

**Project Type**: Full-stack web application with a layered .NET backend

**Performance Goals**: Maintain interview-scale local responsiveness and quick
developer startup; no advanced optimization goals in this slice

**Constraints**: Raw SQL with Npgsql only; thin controllers; explicit DTOs;
local-only CORS; backend-owned business rules; no Entity Framework, Dapper,
Mediator, or MediatR

**Scale/Scope**: Small backend readiness slice covering one new read endpoint
plus local container/runtime preparation

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- PASS Test-first: the plan starts with failing Application, API integration,
  and repository tests for the new category-listing behavior.
- PASS Backend-first: the slice is limited to backend runtime, API, and
  persistence changes that unblock later frontend work.
- PASS Architecture: API delegates to Application, Application stays
  Infrastructure-agnostic, and Infrastructure remains DI-only.
- PASS Data access: the only new persistence behavior is a raw SQL repository
  read method in Infrastructure.
- PASS Security: category write operations remain admin-only, and the new
  public read endpoint is intentionally limited to non-sensitive available
  category data.
- PASS Scope: no frontend implementation, new business expansion, or production
  deployment work is included.
- PASS API consistency: the new endpoint will use explicit DTOs, existing
  ProblemDetails patterns for failures, and remain visible through Swagger.

## Project Structure

### Documentation (this feature)

```text
specs/011-frontend-api-prep/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── frontend-api-readiness-contracts.md
└── tasks.md
```

### Source Code (repository root)

```text
src/
├── backend/
│   ├── BlogPlatform.Domain/
│   ├── BlogPlatform.Application/
│   ├── BlogPlatform.Infrastructure/
│   └── BlogPlatform.Api/
└── frontend/
    └── blog-web/

tests/
└── backend/
    ├── BlogPlatform.Domain.Tests/
    ├── BlogPlatform.Application.Tests/
    ├── BlogPlatform.Infrastructure.Tests/
    └── BlogPlatform.Api.Tests/
```

**Structure Decision**: This slice should primarily touch
`src/backend/BlogPlatform.Api`, `src/backend/BlogPlatform.Application`,
`src/backend/BlogPlatform.Infrastructure`, and the matching backend test
projects, plus root Docker/README files. No frontend source changes belong in
this plan.

## Complexity Tracking

No constitution violations are expected for this slice.
