# Tasks: Configure PostgreSQL Database Environment and Schema Initialization

**Input**: Design documents from `/specs/008-postgres-db-setup/`

**Prerequisites**: `plan.md`, `spec.md`, `research.md`, `data-model.md`, `contracts/`

**Tests**: This infrastructure slice uses bootstrap and inspection validation
instead of backend unit/API/repository tests. Validation tasks must prove local
startup, schema creation, seed loading, and reset behavior.

**Organization**: Tasks are grouped by infrastructure user story so the local
database bootstrap flow can be delivered and validated incrementally.

## Phase 1: Setup

**Purpose**: Create or normalize the database workspace and local environment
entry points used by the rest of the feature.

- [ ] T001 Create or normalize the database workspace structure in `database/scripts/.gitignore`, `database/seeds/.gitignore`, and `database/migrations/.gitignore`
- [ ] T002 Create or update the database bootstrap workspace note in `docs/Database-Implementation-Strategy.md`
- [ ] T003 Create or update a local environment sample for PostgreSQL settings in `.env.example`

---

## Phase 2: Foundational Infrastructure

**Purpose**: Add shared local container and initialization foundations that
block all later schema and seed work.

- [ ] T004 Create the local PostgreSQL service definition in `docker-compose.yml`
- [ ] T005 [P] Document the expected local database variables in `.env.example`
- [ ] T006 [P] Add a bootstrap execution note or helper entrypoint reference in `docs/Database-Implementation-Strategy.md`

**Checkpoint**: The repository has a consistent local PostgreSQL bootstrap
entry point and documented environment variables.

---

## Phase 3: User Story 1 - Developer bootstraps the local PostgreSQL schema (Priority: P1) 🎯 MVP

**Goal**: Let a developer start PostgreSQL locally and initialize the current
use-case schema from repository-managed raw SQL scripts.

**Independent Test**: A developer can start the PostgreSQL container, apply the
ordered schema scripts, and inspect the required current-use-case tables
without adding repository or API code.

- [ ] T007 [US1] Create the base schema initialization script for shared extensions, timestamps, or setup comments in `database/scripts/000-bootstrap.sql`
- [ ] T008 [P] [US1] Create the users table schema script in `database/scripts/001-create-users.sql`
- [ ] T009 [P] [US1] Create the roles table schema script in `database/scripts/002-create-roles.sql`
- [ ] T010 [P] [US1] Create the user-role junction schema script in `database/scripts/003-create-user-roles.sql`
- [ ] T011 [P] [US1] Create the post-categories schema script in `database/scripts/004-create-post-categories.sql`
- [ ] T012 [P] [US1] Create the posts schema script with category and ownership relationships in `database/scripts/005-create-posts.sql`
- [ ] T013 [P] [US1] Create the tags schema script in `database/scripts/006-create-tags.sql`
- [ ] T014 [P] [US1] Create the post-tags junction schema script in `database/scripts/007-create-post-tags.sql`
- [ ] T015 [P] [US1] Create the post-reactions schema script with user or visitor reaction support in `database/scripts/008-create-post-reactions.sql`
- [ ] T016 [US1] Review schema ordering and cross-table constraints against `docs/Database-Schema-Diagram.md` in `database/scripts/000-bootstrap.sql`, `database/scripts/001-create-users.sql`, `database/scripts/002-create-roles.sql`, `database/scripts/003-create-user-roles.sql`, `database/scripts/004-create-post-categories.sql`, `database/scripts/005-create-posts.sql`, `database/scripts/006-create-tags.sql`, `database/scripts/007-create-post-tags.sql`, and `database/scripts/008-create-post-reactions.sql`
- [ ] T017 [US1] Validate that the PostgreSQL container starts and the schema scripts are mounted and applied successfully through `docker-compose.yml`

**Checkpoint**: The local PostgreSQL environment can start and create the
current-use-case schema from repository-managed SQL.

---

## Phase 4: User Story 2 - Developer loads demo data and can reset the environment (Priority: P2)

**Goal**: Let a developer load a compact demo dataset, identify demo
credentials, and reset the database predictably for repeatable interview usage.

**Independent Test**: A developer can rebuild the environment from scratch,
load demo users/roles/categories/posts/tags/reactions, inspect the resulting
records, and repeat the reset flow predictably.

- [ ] T018 [US2] Create the demo users seed script with one administrator and one regular user in `database/seeds/001-seed-users.sql`
- [ ] T019 [P] [US2] Create the demo roles seed script for administrator and regular-user roles in `database/seeds/002-seed-roles.sql`
- [ ] T020 [P] [US2] Create the demo user-role assignment seed script in `database/seeds/003-seed-user-roles.sql`
- [ ] T021 [P] [US2] Create the demo categories seed script in `database/seeds/004-seed-post-categories.sql`
- [ ] T022 [P] [US2] Create the demo posts seed script for public content and ownership coverage in `database/seeds/005-seed-posts.sql`
- [ ] T023 [P] [US2] Create the demo tags seed script in `database/seeds/006-seed-tags.sql`
- [ ] T024 [P] [US2] Create the demo post-tag link seed script in `database/seeds/007-seed-post-tags.sql`
- [ ] T025 [P] [US2] Create the demo post-reactions seed script in `database/seeds/008-seed-post-reactions.sql`
- [ ] T026 [US2] Add a simple local reset helper workflow in `docs/Database-Implementation-Strategy.md`
- [ ] T027 [US2] Document demo credentials, startup, initialization, inspection, and reset steps in `docs/Database-Implementation-Strategy.md`
- [ ] T028 [US2] Update `README.md` with database start and reset commands if the file exists, otherwise document the final workflow only in `docs/Database-Implementation-Strategy.md`
- [ ] T029 [US2] Validate that demo seed data is applied successfully and inspect seeded users, roles, categories, posts, tags, and reactions through `docker-compose.yml` and `database/seeds/001-seed-users.sql`, `database/seeds/002-seed-roles.sql`, `database/seeds/003-seed-user-roles.sql`, `database/seeds/004-seed-post-categories.sql`, `database/seeds/005-seed-posts.sql`, `database/seeds/006-seed-tags.sql`, `database/seeds/007-seed-post-tags.sql`, and `database/seeds/008-seed-post-reactions.sql`
- [ ] T030 [US2] Validate the reset-and-rebuild flow from a clean local state through `docker-compose.yml` and `docs/Database-Implementation-Strategy.md`

**Checkpoint**: The local PostgreSQL environment is demo-ready, documented,
and repeatable from a clean reset.

---

## Final Phase: Polish & Cross-Cutting

**Purpose**: Finalize feature guidance, scope boundaries, and implementation
readiness for later Infrastructure/Data Access work.

- [ ] T031 [P] Update the feature quickstart to match the implemented Compose, schema, seed, and reset workflow in `specs/008-postgres-db-setup/quickstart.md`
- [ ] T032 Review the implemented database scope against deferred tables and document any intentional omissions in `docs/Database-Implementation-Strategy.md`
- [ ] T033 Mark completed work and final validation status in `specs/008-postgres-db-setup/tasks.md`

---

## Dependencies & Execution Order

### Phase Dependencies

- Phase 1 starts first
- Phase 2 depends on Phase 1 and blocks all story work
- Phase 3 depends on Phase 2
- Phase 4 depends on Phase 3 because demo seed data requires the schema to
  exist first
- Final Phase depends on Phases 3 and 4

### User Story Dependencies

- `US1` is the MVP slice and should be completed first.
- `US2` depends on `US1` because seed scripts and reset validation require a
  working container and initialized schema.

### Within Each User Story

- Container and environment setup must exist before schema work begins.
- Schema files must be created in foreign-key-safe order before validation.
- Seed files must be created after the schema is stable and in dependency order.
- Documentation and reset guidance should be finalized only after the actual
  workflow has been validated.

### Parallel Opportunities

- `T005` and `T006` can run in parallel after `T004`.
- `T008`-`T015` can run in parallel once `T007` defines the bootstrap pattern,
  as long as final dependency ordering is reconciled in `T016`.
- `T019`-`T025` can run in parallel after `T018` establishes the demo-user
  seed assumptions.
- `T031` and `T032` can run in parallel after `T030`.

---

## Implementation Strategy

### MVP First

1. Normalize the `database/` workspace and local environment sample.
2. Add the single PostgreSQL Compose service.
3. Deliver `US1` so the local database can start and initialize the current
   schema from raw SQL.
4. Validate `US1` independently before adding demo data and reset flows.

### Incremental Delivery

1. Finish the schema bootstrap path first.
2. Add the demo seed dataset and reset workflow.
3. Update documentation after the real workflow has been exercised.
4. Keep deferred tables and production concerns explicitly out of scope.

## Notes

- `[P]` tasks indicate parallelizable work across different files.
- `US1` and `US2` are derived from the feature’s bootstrap and demo workflows
  because this infrastructure spec does not define product-facing user stories.
- Validation tasks in this slice replace unit/API/repository tests and must
  prove local startup, schema application, seed loading, and repeatable reset
  behavior.
