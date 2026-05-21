# Research: Frontend Docker Compose Service

## Decision 1: Use a development-oriented frontend container

- **Decision**: Run the frontend container with the Vite development server
  instead of introducing a production-style static hosting layer.
- **Rationale**: The feature scope is local development and interview-demo
  execution only. The Vite dev server is already the current frontend runtime
  path and keeps the Docker setup easier to explain.
- **Alternatives considered**:
  - Static production build served by Nginx: more deployment-like, but outside
    the requested scope.
  - Reverse proxy plus static hosting: adds complexity with little value for
    this local-only slice.

## Decision 2: Use explicit frontend API base URL environment configuration

- **Decision**: Pass a dedicated frontend API base URL through environment
  configuration rather than relying on hidden proxy behavior.
- **Rationale**: When the frontend dev server runs in a container, the browser
  still needs a browser-valid API URL. A clear environment variable is easier to
  document and reason about than a proxy-only approach.
- **Alternatives considered**:
  - Dev-server proxy without explicit frontend configuration: possible, but less
    transparent for interview review.
  - Hard-coded API URL in frontend code: too rigid and harder to override.

## Decision 3: Keep Compose wiring explicit and small

- **Decision**: Add only the frontend service, its build/runtime settings, and
  the minimum service dependency hints needed for a reliable local stack.
- **Rationale**: The current Compose file is intentionally small. Matching that
  style keeps the stack understandable and avoids drifting into deployment
  orchestration complexity.
- **Alternatives considered**:
  - Expanded orchestration with extra helper services: unnecessary for this
    challenge.
  - Separate frontend-only Compose file: increases operational overhead.

## Decision 4: Reuse the existing local CORS configuration path

- **Decision**: Extend the API’s existing local-origin configuration only if the
  frontend Compose browser URL differs from the current supported origins.
- **Rationale**: The API already uses a configuration-driven local CORS path.
  Reusing it keeps changes narrow and avoids new policy mechanisms.
- **Alternatives considered**:
  - Wildcard development CORS: easier short term, but less disciplined.
  - No API change at all: valid only if the chosen frontend URL is already
    covered.

## Decision 5: Validate through operational full-stack startup

- **Decision**: Use `docker compose config`, `docker compose up`, `docker
  compose ps`, and direct URL checks as the primary validation path.
- **Rationale**: This slice is infrastructure and runtime oriented. Operational
  validation demonstrates the behavior that matters most for local demo usage.
- **Alternatives considered**:
  - New automated browser test infrastructure: more complete, but out of scope
    for a small runtime configuration slice.
