# Quickstart: PostgreSQL Database Environment and Schema Initialization

## Goal

Prepare the smallest repeatable local PostgreSQL environment needed for later
Infrastructure/Data Access implementation and interview demos.

## Step 1: Add the local container bootstrap

Create a repository-root Docker Compose definition for a single PostgreSQL
service with documented local environment variables.

Recommended files:

- `docker-compose.yml`
- `.env.example` or equivalent documented environment sample

If local port `5432` is already in use, override `BLOG_PLATFORM_DB_PORT` before
starting the service.

## Step 2: Create executable schema artifacts

Add ordered SQL initialization files under `database/scripts/`.

Recommended scope for the first schema slice:

- `users`
- `roles`
- `user_roles`
- `post_categories`
- `posts`
- `tags`
- `post_tags`
- `post_reactions`

Recommended file organization:

- `database/scripts/000-bootstrap.sql`
- `database/scripts/001-create-users.sql`
- `database/scripts/002-create-roles.sql`
- `database/scripts/003-create-user-roles.sql`
- `database/scripts/004-create-post-categories.sql`
- `database/scripts/005-create-posts.sql`
- `database/scripts/006-create-tags.sql`
- `database/scripts/007-create-post-tags.sql`
- `database/scripts/008-create-post-reactions.sql`
- `database/scripts/009-apply-seeds.sql`

## Step 3: Add demo seed data

Add ordered demo seed files under `database/seeds/`.

Recommended seed coverage:

- one administrator user
- one regular user
- role assignments
- categories
- public posts
- tags and post-tag links
- reactions

Recommended file organization:

- `database/seeds/001-seed-users.sql`
- `database/seeds/002-seed-roles.sql`
- `database/seeds/003-seed-user-roles.sql`
- `database/seeds/004-seed-post-categories.sql`
- `database/seeds/005-seed-posts.sql`
- `database/seeds/006-seed-tags.sql`
- `database/seeds/007-seed-post-tags.sql`
- `database/seeds/008-seed-post-reactions.sql`

## Step 4: Document local workflows

Update project documentation so a reviewer can:

- start the database
- initialize the schema
- load demo data
- reset the environment
- inspect the container and data set

Recommended documentation targets:

- `README.md` if present
- `docs/Database-Implementation-Strategy.md`

## Step 5: Validate the bootstrap flow

Verify the local environment by proving these workflows succeed:

1. Start PostgreSQL from a clean local state.
2. Apply the schema initialization scripts in the documented order.
3. Apply the seed scripts in the documented order.
4. Inspect the resulting tables and demo records.
5. Reset and rebuild the environment predictably.

## Deferred Work

Do not add yet:

- repository classes
- API startup wiring
- frontend integration
- migration frameworks
- permissions, modules, settings, or role-permission tables
