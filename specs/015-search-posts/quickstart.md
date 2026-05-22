# Quickstart: Search Posts Through Backend API

## Goal

Validate that post search is handled by the backend rather than by client-side
filtering, while preserving visibility rules for anonymous and authenticated
users.

## Preconditions

- Docker and Docker Compose are available locally
- PostgreSQL, API, and frontend services can start from the current repository
- Existing seeded public posts are present
- Test fixtures or seed-aligned API tests cover at least one owned non-public
  matching post for authenticated search validation

## Validation Steps

1. Run the backend automated checks:

```bash
dotnet test tests/backend/BlogPlatform.Application.Tests/BlogPlatform.Application.Tests.csproj
dotnet test tests/backend/BlogPlatform.Infrastructure.Tests/BlogPlatform.Infrastructure.Tests.csproj
dotnet test tests/backend/BlogPlatform.Api.Tests/BlogPlatform.Api.Tests.csproj
```

2. Run the frontend validation checks from `src/frontend/blog-web`:

```bash
npm run lint
npm run build
```

3. Start the local stack:

```bash
docker compose up -d postgres api frontend
docker compose ps
```

4. Validate anonymous search:
   - open `http://localhost:5173`
   - search for a known public seeded term such as `architecture`
   - confirm the browser shows only matching public posts
   - clear the search and confirm the default listing returns

5. Validate authenticated search:
   - log in as `user@blogplatform.local` / `User123!`
   - search for a term that matches one of the user’s own private or non-public
     posts when such a fixture exists
   - confirm owned private matches can appear
   - confirm other users’ private posts do not appear

6. Spot-check the API directly:

```bash
curl "http://localhost:5034/api/posts?q=architecture"
```

If needed, repeat with an authenticated request using a seeded bearer token from
`POST /api/auth/login`.

## Expected Outcome

- Anonymous and authenticated users both receive backend-filtered results.
- Empty search behaves like the existing default list route.
- Owned private matches may appear only for the authenticated owner.
- The frontend no longer relies on local-only filtering for post search.
