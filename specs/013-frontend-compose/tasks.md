---

description: "Task list for frontend Docker Compose service implementation"
---

# Tasks: Frontend Docker Compose Service

**Input**: Design documents from `/specs/013-frontend-compose/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/

**Tests**: Use operational Docker Compose validation, frontend/API reachability
checks, and existing run/build commands for this infrastructure slice. No new
backend unit or integration tests are required.

**Organization**: Tasks are grouped by user story to enable independent
implementation and verification of the local full-stack runtime slice.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1)
- Include exact file paths in descriptions

## Path Conventions

- **Runtime root**: `docker-compose.yml`, `.env.example`, `README.md`
- **Frontend app**: `src/frontend/blog-web/`
- **Frontend config**: `src/frontend/blog-web/Dockerfile`,
  `src/frontend/blog-web/vite.config.ts`
- **API config**: `src/backend/BlogPlatform.Api/appsettings.json`,
  `src/backend/BlogPlatform.Api/appsettings.Development.json`
- **Feature docs**: `specs/013-frontend-compose/`

## Phase 1: Setup

**Purpose**: Confirm the current runtime baseline and prepare the frontend app
for container-based local development.

- [ ] T001 Review the existing `postgres` and `api` services in
      `docker-compose.yml` before adding frontend runtime wiring
- [ ] T002 Review current local stack documentation in `README.md` and
      `src/frontend/blog-web/README.md` to identify the gaps for full-stack
      Compose usage
- [ ] T003 Review current frontend runtime configuration in
      `src/frontend/blog-web/package.json` and
      `src/frontend/blog-web/vite.config.ts` before adding container-oriented
      changes

---

## Phase 2: Foundational Runtime

**Purpose**: Create the shared frontend container and configuration foundation
needed for full-stack Compose startup.

**CRITICAL**: Full-stack validation depends on this phase being complete.

- [ ] T004 Create a development-oriented frontend Dockerfile in
      `src/frontend/blog-web/Dockerfile`
- [ ] T005 Configure the frontend Dockerfile working directory, dependency
      install path, exposed port, and Vite startup command in
      `src/frontend/blog-web/Dockerfile`
- [ ] T006 Add a frontend service to `docker-compose.yml`
- [ ] T007 Configure frontend service build path, working directory, mounted
      volumes, port mapping, and startup command in `docker-compose.yml`
- [ ] T008 Add the frontend API base URL environment variable and any related
      frontend runtime values to `.env.example`
- [ ] T009 Wire the frontend service environment configuration into
      `docker-compose.yml`
- [ ] T010 Update `src/frontend/blog-web/vite.config.ts` so the Vite dev server
      is reachable from the container and local browser
- [ ] T011 Add or update frontend environment loading support in
      `src/frontend/blog-web/` only if the app needs one documented entry point
      for `VITE_API_BASE_URL`
- [ ] T012 Review and extend local frontend origin support in
      `src/backend/BlogPlatform.Api/appsettings.json` and
      `src/backend/BlogPlatform.Api/appsettings.Development.json` only if the
      chosen frontend Compose URL is not already allowed

**Checkpoint**: The frontend container definition, Compose wiring, and local
environment configuration are ready for runtime validation.

---

## Phase 3: User Story 1 - Full Stack Runs Together Locally (Priority: P1) 🎯 MVP

**Goal**: Prove that the database, API, and frontend can run together through
Docker Compose and are reachable on their documented local URLs.

**Independent Test**: Run `docker compose up -d postgres api frontend`, confirm
all three services are running, open the frontend in a browser, confirm Swagger
remains reachable, and verify the frontend receives the expected API base URL
configuration for future integration.

### Implementation for User Story 1

- [ ] T013 [US1] Add explicit service dependency hints for frontend startup in
      `docker-compose.yml`
- [ ] T014 [US1] Document the browser-facing frontend URL and API URL in
      `docker-compose.yml`-aligned runtime notes within `README.md`
- [ ] T015 [US1] Run `docker compose config` from the repository root and
      resolve any configuration issues in `docker-compose.yml`
- [ ] T016 [US1] Start `postgres`, `api`, and `frontend` together with Docker
      Compose from the repository root
- [ ] T017 [US1] Verify container status and startup health with the documented
      service checks in `specs/013-frontend-compose/quickstart.md`
- [ ] T018 [US1] Validate the frontend loads from the documented local browser
      URL using the Compose-based stack
- [ ] T019 [US1] Validate Swagger remains reachable from the documented API URL
      while the frontend service is running
- [ ] T020 [US1] Confirm the frontend container is receiving the documented
      `VITE_API_BASE_URL` configuration for future API integration checks
- [ ] T021 [US1] Stop and restart the Compose stack once to confirm the
      full-stack workflow is repeatable for demo usage

**Checkpoint**: The local full stack is operational and independently
verifiable.

---

## Final Phase: Polish & Cross-Cutting Concerns

**Purpose**: Finish the operational documentation and confirm the slice stayed
small and local-development oriented.

- [ ] T022 [P] Update full-stack Docker Compose instructions in `README.md`
- [ ] T023 [P] Update frontend-local container notes in
      `src/frontend/blog-web/README.md`
- [ ] T024 Re-run the documented full-stack validation flow in
      `specs/013-frontend-compose/quickstart.md` and record any follow-up notes
      there
- [ ] T025 Review `docker-compose.yml`, `.env.example`, and
      `src/frontend/blog-web/Dockerfile` for unnecessary complexity and remove
      anything not needed for local interview-demo usage
- [ ] T026 Confirm the implementation did not introduce frontend screens,
      backend business logic changes, production deployment setup, reverse
      proxies, or other out-of-scope work

---

## Dependencies & Execution Order

### Phase Dependencies

- Setup starts first
- Foundational Runtime depends on Setup
- User Story 1 depends on the frontend container and Compose wiring being
  complete
- Final Polish depends on User Story 1 being complete

### Within User Story 1

- Compose configuration must be complete before runtime validation begins
- Service startup must succeed before browser and Swagger reachability checks
- Environment configuration verification should happen while the stack is
  running

### Parallel Opportunities

- T022 and T023 can run in parallel after T021

---

## Implementation Strategy

### MVP First

1. Create the frontend Dockerfile
2. Add the frontend service and environment wiring to Docker Compose
3. Adjust Vite/API local runtime configuration only where required
4. Validate the full stack end to end with Compose

### Incremental Delivery

1. Finish runtime and environment setup
2. Prove the stack starts and all local URLs remain reachable
3. Document the workflow clearly for reviewers
4. Stop before adding frontend features or production deployment complexity

## Notes

- Tasks are intentionally runtime- and documentation-focused for this slice
- Browser-visible API configuration must remain explicit and easy to explain
- Keep the Docker setup small, readable, and aligned with local demo usage
