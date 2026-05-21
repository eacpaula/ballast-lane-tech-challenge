# Contract: Database Bootstrap Workflow

## Purpose

This contract defines the expected local workflow for starting, initializing,
resetting, and inspecting the PostgreSQL database environment before repository
implementation exists.

## Startup Contract

### Input

- Local environment variables for database name, username, password, and port
- A repository checkout with Docker Compose and `database/` artifacts present

### Expected Outcome

- A local PostgreSQL container starts successfully
- The database is reachable on the documented local port
- Schema initialization artifacts are available to run in the documented order

## Schema Initialization Contract

### Input

- A running local PostgreSQL container
- Ordered SQL schema scripts under `database/scripts/`

### Expected Outcome

- The current-use-case tables are created successfully:
  - `users`
  - `roles`
  - `user_roles`
  - `posts`
  - `post_categories`
  - `tags`
  - `post_tags`
  - `post_reactions`
- Required relationships for current use cases exist after initialization

## Seed Data Contract

### Input

- An initialized database schema
- Ordered seed scripts under `database/seeds/`

### Expected Outcome

- Demo users exist for at least:
  - one administrator
  - one regular user
- Demo categories, posts, tags, post-tag links, and reactions exist for local
  verification
- Demo credentials are documented for local-only use

## Reset Contract

### Input

- An existing local database environment that may already contain schema and
  demo data

### Expected Outcome

- The local environment can be cleared and recreated predictably
- Re-running startup, schema initialization, and seed workflows returns the
  environment to the documented demo-ready state

## Deferred Behavior

The following are intentionally outside this contract:

- Repository implementation details
- API dependency injection or startup wiring
- HTTP endpoints
- Frontend behavior
- Migration framework behavior
- Full RBAC table usage beyond simple administrator role seeding
