# Research: Frontend MVP

## Decision 1: Use `react-router-dom` for page routing and route guards

- **Decision**: Add `react-router-dom` as the only new frontend runtime
  dependency for routing, nested layouts, and route protection.
- **Rationale**: The MVP needs public, authenticated, and admin-only pages.
  Routing is the one missing browser capability that should not be rebuilt by
  hand. `react-router-dom` covers this cleanly without expanding into a larger
  framework.
- **Alternatives considered**:
  - Manual route switching in React state: too fragile and harder to explain.
  - A full meta-framework: too large for the interview scope.

## Decision 2: Use a thin typed `fetch` client instead of Axios or a query library

- **Decision**: Implement one shared API client around browser `fetch` and keep
  endpoint modules explicit by feature.
- **Rationale**: The backend already provides stable DTOs and ProblemDetails
  responses. A small wrapper is enough for base URL handling, bearer token
  attachment, JSON parsing, and normalized errors.
- **Alternatives considered**:
  - Axios: adds dependency surface without solving a real MVP problem.
  - React Query or similar: useful later, but too much machinery for the first
    slice.

## Decision 3: Store session data in local storage and decode JWT claims only for UI guidance

- **Decision**: Persist the authentication payload plus basic user identity in
  local storage and decode JWT claims locally to recover user id and roles after
  refresh.
- **Rationale**: The login response returns a token but does not return an
  explicit administrator flag. The JWT already carries role claims, so decoding
  the payload is enough for route and navigation behavior while keeping backend
  authorization as the source of truth.
- **Alternatives considered**:
  - Add a new backend profile endpoint immediately: possible, but unnecessary if
    the token already exposes the needed UI claims.
  - Infer admin state from failed requests only: too poor for navigation and
    guard UX.

## Decision 4: Persist a simple anonymous visitor identifier in local storage

- **Decision**: Generate and persist a small visitor identifier in local storage
  for anonymous reaction requests.
- **Rationale**: The reaction endpoint accepts a visitor identifier for
  anonymous actors. Keeping one browser-stable identifier supports repeated demo
  flows without adding fingerprinting or broader identity complexity.
- **Alternatives considered**:
  - Generate a new identifier on every reaction: unstable and harder to reason
    about.
  - Require login for reactions: directly conflicts with the existing public
    capability.

## Decision 5: Organize the frontend by app, shared primitives, and features

- **Decision**: Use `app/`, `components/`, `features/`, and `lib/` folders
  rather than a flatter single-layer `src/`.
- **Rationale**: The MVP now spans auth, public posts, author tools, and admin
  tooling. A modest feature structure keeps files discoverable without jumping
  to a heavy architecture.
- **Alternatives considered**:
  - Keep everything under `src/` with page-level files only: too noisy once
    forms, hooks, and API modules appear.
  - Introduce a domain-driven frontend framework: too abstract for the project
    size.

## Decision 6: Treat missing author/admin read endpoints as a backend-first prerequisite

- **Decision**: Plan a small prerequisite backend slice before dependent
  frontend pages for:
  - listing the authenticated user’s posts
  - loading a protected owned post for editing
  - listing all categories for admin management
- **Rationale**: The current API supports public reads and protected writes, but
  it does not yet provide the read contracts required by the “My Posts” and
  admin category pages. Building the frontend around implicit assumptions would
  create fragile workarounds.
- **Alternatives considered**:
  - Reuse public post reads for edit flows: insufficient for unavailable or
    non-public owned posts.
  - Build admin category management from only the public available-categories
    list: insufficient because unavailable categories are excluded.

## Decision 7: Keep frontend validation lightweight for the first MVP slice

- **Decision**: Use lint/build checks plus manual full-stack flow validation,
  while relying on existing backend API tests for contract safety.
- **Rationale**: The frontend currently has no test framework, and introducing
  a broad browser-test setup would consume disproportionate effort relative to
  the MVP scope.
- **Alternatives considered**:
  - Add a full frontend test stack immediately: useful later, but too much
    setup for the first complete slice.
  - Rely on manual checks only without lint/build: too weak for a technical
    interview deliverable.
