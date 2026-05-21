# Research: Configure PostgreSQL Database Environment and Schema Initialization

## Decision 1: Use a single PostgreSQL service in Docker Compose

- **Decision**: Start with one PostgreSQL service in a repository-root
  `docker-compose.yml` dedicated to local development and interview demos.
- **Rationale**: The feature requires a simple, repeatable local environment.
  A single service keeps commands and troubleshooting easy to explain.
- **Alternatives considered**:
  - Compose with API and frontend services now: rejected because this slice is
    only about database preparation.
  - Manual local PostgreSQL installation instructions only: rejected because it
    reduces repeatability across reviewer machines.

## Decision 2: Organize executable SQL by ordered schema and seed scripts

- **Decision**: Place executable SQL under `database/scripts/` for schema and
  `database/seeds/` for demo data, using ordered filenames so the bootstrap
  sequence is obvious.
- **Rationale**: Ordered raw SQL files are explicit, easy to inspect, and align
  with the challenge requirement to avoid ORM or migration tooling.
- **Alternatives considered**:
  - One large monolithic SQL file: rejected because it is harder to review and
    reset incrementally.
  - A migration framework now: rejected because it adds tooling complexity
    before repository behavior exists.

## Decision 3: Align the initial schema strictly to implemented use cases

- **Decision**: Implement only `users`, `roles`, `user_roles`, `posts`,
  `post_categories`, `tags`, `post_tags`, and `post_reactions` in the first
  executable schema slice.
- **Rationale**: Those tables cover the current implemented behavior:
  registration/login, posts, public reads, reactions, tags, and admin category
  management.
- **Alternatives considered**:
  - Implement the entire diagram including permissions, modules, settings, and
    role permissions: rejected as unnecessary scope before those use cases
    exist in code.
  - Implement only users and posts now: rejected because category, tag, role,
    and reaction flows already exist at the specification/application level.

## Decision 4: Defer permission and module tables

- **Decision**: Defer `permissions`, `modules`, `settings`, and
  `role_permissions` until repository/API behavior requires them.
- **Rationale**: The current application uses a simple administrator concept,
  not a full RBAC graph. Adding those tables now would create schema surface
  area without immediate business value.
- **Alternatives considered**:
  - Include empty placeholder tables for completeness: rejected because they
    complicate the bootstrap story without serving a current use case.

## Decision 5: Seed a small but complete demo dataset

- **Decision**: Seed one administrator, one regular user, a small set of
  categories, public posts, tags, post-tag links, and reactions.
- **Rationale**: A compact dataset is enough to demonstrate current flows while
  keeping manual inspection practical during a technical interview.
- **Alternatives considered**:
  - No seed data: rejected because it would slow demos and later repository
    verification.
  - Large seed data set: rejected because it adds noise without interview value.

## Decision 6: Document demo credentials explicitly as local-only

- **Decision**: Publish demo credentials in documentation as local/demo-only
  values and keep the seed expectations transparent.
- **Rationale**: The interview setup needs predictable credentials, but the
  documentation must clearly distinguish them from production practices.
- **Alternatives considered**:
  - Hide demo credentials entirely: rejected because it makes setup and review
    slower.
  - Treat demo values as production-ready defaults: rejected for security and
    clarity reasons.

## Decision 7: Use environment variables for container configuration only

- **Decision**: Keep environment variables limited to local PostgreSQL
  container settings such as database name, username, password, port, and
  optional connection string guidance.
- **Rationale**: This keeps the environment predictable while avoiding early
  coupling to API startup or repository implementation details.
- **Alternatives considered**:
  - Hardcode all container settings in Compose: rejected because it reduces
    local flexibility.
  - Introduce a broad app-wide environment strategy now: rejected because that
    belongs to later API/infrastructure integration work.

## Decision 8: Validate with bootstrap and inspection workflows, not repository code

- **Decision**: Treat validation for this slice as successful start, reset,
  schema creation, and demo-data inspection workflows rather than repository
  implementation tests.
- **Rationale**: Repository classes do not exist yet, so the highest-value
  validation is proving the environment can be created and re-created reliably.
- **Alternatives considered**:
  - Wait for repository tests before validating the database environment:
    rejected because it couples two slices unnecessarily.
  - Add ad hoc manual-only validation with no documented checks: rejected
    because repeatability would be weak.
