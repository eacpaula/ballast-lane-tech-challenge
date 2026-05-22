# Quickstart: Add Paginated Post Search With Redis Caching

## Goal

Validate that post listing and post search now use page-based pagination and
30-second Redis caching without leaking private results across viewer contexts.

## Preconditions

- Docker and Docker Compose are available locally
- PostgreSQL, API, frontend, and Redis services can start from the repository
- Existing backend-powered search is already present as the baseline behavior

## Validation Steps

1. Start the full local stack:

```bash
docker compose up -d postgres redis api frontend
docker compose ps
```

2. Run backend automated checks:

```bash
dotnet test tests/backend/BlogPlatform.Application.Tests/BlogPlatform.Application.Tests.csproj
dotnet test tests/backend/BlogPlatform.Infrastructure.Tests/BlogPlatform.Infrastructure.Tests.csproj
dotnet test tests/backend/BlogPlatform.Api.Tests/BlogPlatform.Api.Tests.csproj
```

3. Run frontend validation from `src/frontend/blog-web`:

```bash
npm run lint
npm run build
```

4. Validate anonymous pagination:
   - open `http://localhost:5173`
   - confirm the first page loads
   - scroll until the next page request is triggered
   - confirm a loading indicator appears for next-page fetch
   - confirm an end-of-list state appears when no more pages exist

5. Validate search pagination:
   - search for a seeded public term
   - confirm results reset to page one
   - load additional pages if available
   - clear the search and confirm the default paginated listing returns

6. Validate authenticated private-result isolation:
   - log in as the seeded regular user
   - search for a term that matches an owned private post fixture
   - confirm the owned private match can appear
   - confirm another user’s private match does not appear

7. Spot-check the paginated API directly:

```bash
curl "http://localhost:5034/api/posts?page=1&pageSize=6"
curl "http://localhost:5034/api/posts?q=architecture&page=1&pageSize=6"
```

8. If Redis inspection is useful during validation, confirm cache entries are
   appearing and expiring under viewer-safe keys from the local Redis service.

## Expected Outcome

- The feed uses paginated backend responses for both default listing and search.
- Infinite-scroll behavior loads additional pages without redesigning the UI.
- Cached read results can be reused for 30 seconds without leaking
  viewer-specific private data.
- Short-lived stale list/search results after writes are accepted and
  documented as an intentional trade-off.
