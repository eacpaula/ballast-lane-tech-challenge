## Database Implementation Strategy

The project uses PostgreSQL running through Docker Compose for local development and demo purposes.

The database schema is based on the existing `Database-Schema-Diagram.md`, but the first implementation will focus only on the tables required by the current use cases: users, roles, user roles, posts, post categories, tags, post tags, and post reactions.

Schema creation and seed data will be handled through SQL scripts under the `database/` folder. This keeps the setup explicit and aligned with the challenge restriction against Entity Framework and Dapper.

## Local Database Workspace

The local database bootstrap uses these repository locations:

- `docker-compose.yml`
- `.env.example`
- `database/scripts/`
- `database/seeds/`

The current bootstrap flow keeps database setup small and explicit. It does not
introduce a migration framework yet.

## Environment Variables

Use a local `.env` file or shell environment variables to override these values
if needed:

- `BLOG_PLATFORM_DB_NAME`
- `BLOG_PLATFORM_DB_USER`
- `BLOG_PLATFORM_DB_PASSWORD`
- `BLOG_PLATFORM_DB_PORT`

Defaults are documented in `.env.example`.

## Demo Credentials

These credentials are for local development and technical interview demos only:

- Administrator
  - Email: `admin@blogplatform.local`
  - Username: `admin`
  - Password: `Admin123!`
- Regular user
  - Email: `user@blogplatform.local`
  - Username: `user`
  - Password: `User123!`

Seeded password values are stored as hashes, not plain text.

## Schema Scope

The initial executable schema intentionally includes only:

- `users`
- `roles`
- `user_roles`
- `post_categories`
- `posts`
- `tags`
- `post_tags`
- `post_reactions`

The following diagram tables are intentionally deferred because the current
implemented use cases do not need them yet:

- `permissions`
- `modules`
- `settings`
- `role_permissions`

Deferring them keeps the bootstrap flow aligned with the existing application
and avoids premature RBAC expansion before Infrastructure/Data Access work
requires it.

## Startup Workflow

1. Copy `.env.example` to `.env` if you want to override the defaults.
   Set `BLOG_PLATFORM_DB_PORT` to another local port if `5432` is already in
   use on your machine.
2. Start PostgreSQL:

   ```bash
   docker compose up -d postgres
   ```

3. Wait for the service to become healthy:

   ```bash
   docker compose ps
   ```

The PostgreSQL container mounts `database/scripts/` into
`/docker-entrypoint-initdb.d` and `database/seeds/` into `/seed-data`. The
entrypoint runs the ordered schema scripts and then executes the seed scripts
through `database/scripts/009-apply-seeds.sql` on first initialization.

## Inspect the Database

Open a shell in the container:

```bash
docker compose exec postgres psql -U "${BLOG_PLATFORM_DB_USER:-blogplatform}" -d "${BLOG_PLATFORM_DB_NAME:-blogplatform}"
```

Useful inspection queries:

```sql
\dt
SELECT username, email FROM users ORDER BY id;
SELECT title FROM roles ORDER BY id;
SELECT title FROM post_categories ORDER BY id;
SELECT title, public_post, available FROM posts ORDER BY id;
SELECT reaction_type, visitor_identifier FROM post_reactions ORDER BY id;
```

## Reset Workflow

To rebuild the local environment from scratch:

```bash
docker compose down -v
docker compose up -d postgres
```

Removing the named volume forces PostgreSQL to run the initialization and seed
scripts again on the next startup.

## Validation Workflow

After startup, validate the bootstrap with:

1. `docker compose ps` to confirm the container is healthy.
2. `docker compose logs postgres` to confirm the initialization completed.
3. `psql` inspection queries to confirm:
   - seeded users exist
   - administrator and regular roles exist
   - seeded categories exist
   - seeded posts exist
   - seeded tags and post-tag links exist
   - seeded reactions exist

## Deferred Work

This slice does not include:

- repository implementation
- Npgsql data-access classes
- frontend integration
- migration framework adoption

## Local API Integration

The backend API now consumes the seeded PostgreSQL environment directly through
raw SQL repositories. Start the database first, then run:

```bash
dotnet run --project src/backend/BlogPlatform.Api/BlogPlatform.Api.csproj
```

Local API exploration endpoints:

- Swagger UI: `http://localhost:5034/swagger`
- OpenAPI document: `http://localhost:5034/swagger/v1/swagger.json`

Demo credentials remain:

- Administrator: `admin@blogplatform.local` / `Admin123!`
- Regular user: `user@blogplatform.local` / `User123!`

Suggested demo order:

1. `POST /api/auth/login`
2. `GET /api/posts`
3. `GET /api/posts/{postId}`
4. `POST /api/posts/{postId}/reactions`
5. Authenticated post create/update/delete
6. Administrator-only category create/update/deactivate
