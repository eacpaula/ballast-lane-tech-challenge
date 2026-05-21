# Implementation Plan: Frontend Docker Compose Service

**Branch**: `013-frontend-compose` | **Date**: 2026-05-21 | **Spec**:
`specs/013-frontend-compose/spec.md`

**Input**: Feature specification from
`/specs/013-frontend-compose/spec.md`

**Note**: This plan focuses on local full-stack runtime readiness only. It adds
the React frontend to Docker Compose, aligns frontend environment and API CORS
configuration for container-based local development, and documents how to run
the database, API, and frontend together for interview demos.

## Summary

Add the existing frontend app to the local Docker Compose stack without
expanding into new product behavior.

- Create a development-oriented frontend Dockerfile for the Vite app.
- Add a frontend service to the existing Compose file alongside `postgres` and
  `api`.
- Pass the API base URL into the frontend through environment configuration
  suitable for the Compose network and browser access.
- Extend local API CORS configuration only as needed to support the frontend
  Docker-based URL.
- Keep the setup simple enough that reviewers can bring up the full stack with
  one documented Compose workflow.

## Implementation Design

**1. Technical approach**:

- Treat this as a local runtime orchestration slice, not a frontend feature.
- Run the frontend container in development mode through the Vite dev server so
  the setup stays simple and aligned with the current app structure.
- Use Docker Compose service wiring and environment variables instead of adding
  reverse proxies, production static hosting, or multi-environment deployment
  machinery.
- Keep backend feature behavior unchanged; only local runtime configuration may
  expand where the frontend service requires it.

**2. Files and folders affected**:

- `docker-compose.yml`
- root environment examples such as `.env.example`
- `src/frontend/blog-web/Dockerfile`
- `src/frontend/blog-web/package.json` only if container-friendly scripts or
  host settings are needed
- `src/frontend/blog-web/vite.config.ts`
- optional frontend environment files if the app needs one local API base URL
  entry point
- `src/backend/BlogPlatform.Api/appsettings.json`
- `src/backend/BlogPlatform.Api/appsettings.Development.json`
- `README.md`
- `src/frontend/blog-web/README.md`
- no planned changes to backend business logic, domain models, or persistence

**3. Frontend Dockerfile strategy**:

- Build a small development-oriented frontend image from the existing
  `blog-web` app directory.
- Install Node dependencies in the image and start the Vite dev server as the
  container entrypoint.
- Keep the Dockerfile focused on local development and demo usage rather than
  production bundling or static asset serving.
- Avoid multi-stage optimization unless it is needed for clarity or startup
  reliability.

**4. Docker Compose strategy**:

- Add a dedicated `frontend` service to `docker-compose.yml`.
- Put the frontend on the same default Compose network as the API and database.
- Expose one local frontend port for browser access.
- Use service dependencies to improve startup order clarity, while relying on
  the frontend to talk to the browser-facing API URL where appropriate.
- Keep the service definition explicit and small: build path, environment,
  command, dependency hints, and port exposure only.

**5. Environment variable strategy**:

- Provide a frontend-facing environment variable for the API base URL.
- Keep the value browser-correct for local usage, since frontend code runs in
  the browser even when the dev server itself is containerized.
- Use a small documented environment surface instead of introducing multiple
  overlapping frontend config layers.
- Extend `.env.example` only with the values needed for Compose-based frontend
  execution.

**6. Vite/container networking considerations**:

- Ensure Vite listens on a host/interface that works from inside the container
  and remains reachable from the local browser through the mapped port.
- Keep the dev server in a mode that supports local iteration and interview
  demos without extra proxy infrastructure.
- Prefer direct environment configuration for the API URL over hidden dev-server
  proxy behavior in this slice.

**7. API CORS strategy**:

- Reuse the existing local-origin CORS configuration path.
- Add the frontend Docker-based browser URL only if it differs from already
  supported local origins.
- Keep CORS narrow and development-oriented; avoid wildcard expansion or
  production-style broad allowances.
- Do not change endpoint authorization or backend feature rules.

**8. Validation strategy**:

- Validate the slice through:
  - `docker compose config`
  - `docker compose up -d postgres api frontend`
  - service status inspection with `docker compose ps`
  - frontend reachability in a browser-accessible URL
  - API reachability from the frontend-configured base URL path
  - Swagger reachability to confirm the API remains available
- Keep validation manual and operational rather than adding new browser test
  infrastructure.

**9. Documentation update strategy**:

- Update `README.md` with one full-stack Compose workflow covering database,
  API, and frontend startup.
- Update frontend-local documentation with any container-specific notes that are
  useful for local contributors.
- Document the frontend URL, API URL, and any local environment variables that
  matter for the full stack.
- Keep documentation short and demo-oriented.

**10. Risks and trade-offs**:

- Using the Vite dev server in Docker is less production-like than static asset
  hosting, but it is simpler and better aligned with local interview demos.
- Browser-facing API configuration can be slightly counterintuitive because the
  frontend server runs in a container while the code executes in the browser,
  but documenting that split keeps the setup predictable.
- Adding a frontend Compose service increases local runtime surface area, but it
  removes friction when demonstrating the full stack end to end.
- Deferring production container optimization keeps the slice focused and avoids
  unnecessary deployment complexity.

## Technical Context

**Language/Version**: .NET 10 LTS (backend), TypeScript with React 19 and Vite
8 (frontend)

**Primary Dependencies**: ASP.NET Core Web API, Npgsql, React, Vite,
TailwindCSS, Docker Compose

**Storage**: PostgreSQL

**Testing**: Operational validation through Docker Compose startup, frontend/API
reachability checks, and existing frontend/API run commands where useful

**Target Platform**: Local full-stack web application running through Docker
Compose and consumed in a browser

**Project Type**: Full-stack web application with separate backend and frontend
apps

**Performance Goals**: Reliable local startup and acceptable demo responsiveness
for interview-scale usage

**Constraints**: Local development only; no backend business logic changes; no
production hosting complexity; no reverse proxy requirement; no direct Ballast
Lane branding expansion

**Scale/Scope**: Small infrastructure slice covering frontend containerization,
Compose wiring, environment configuration, local CORS alignment, and
documentation

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- PASS Test-first: no backend business behavior changes are planned, so backend
  fail-first testing is not implicated; validation focuses on runtime startup
  and connectivity.
- PASS Backend-first: the backend API and data services already exist, and this
  slice only adds frontend runtime orchestration on top of that stable base.
- PASS Architecture: backend layers remain unchanged, frontend runtime changes
  stay outside Application/Domain/Infrastructure business boundaries, and API
  changes remain configuration-only.
- PASS Data access: no new persistence behavior is introduced.
- PASS Security: endpoint authorization and business rules stay unchanged; only
  local CORS compatibility is under consideration.
- PASS Scope: the slice is restricted to frontend Docker runtime, environment
  wiring, CORS compatibility, and documentation.
- PASS API consistency: no API contract or error-handling behavior changes are
  planned.
- PASS Frontend governance: the slice does not alter the Tailwind design-system
  direction and keeps the frontend architecture small and maintainable.

## Project Structure

### Documentation (this feature)

```text
specs/013-frontend-compose/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── local-full-stack-runtime-contract.md
└── tasks.md
```

### Source Code (repository root)

```text
docker-compose.yml

src/
├── backend/
│   └── BlogPlatform.Api/
└── frontend/
    └── blog-web/
        ├── Dockerfile
        ├── package.json
        ├── vite.config.ts
        └── src/
```

**Structure Decision**: Keep the work in root runtime files, the frontend app
directory, and API configuration files only. No backend domain or application
code should be touched in this plan.

## Complexity Tracking

No constitution violations are expected for this slice.
