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

5. Validate authenticated default listing behavior:
   - log in as the seeded regular user
   - load the default feed without a search term
   - confirm the feed remains public-only and does not add owned private posts
     just because the user is authenticated

6. Validate search pagination:
   - search for a seeded public term
   - confirm results reset to page one
   - load additional pages if available
   - clear the search and confirm the default paginated listing returns

7. Validate authenticated private-result isolation:
   - log in as the seeded regular user
   - search for a term that matches an owned private post fixture
   - confirm the owned private match can appear
   - confirm another user’s private match does not appear

8. Spot-check the paginated API directly:

```bash
curl "http://localhost:5034/api/posts?page=1&pageSize=6"
curl "http://localhost:5034/api/posts?q=architecture&page=1&pageSize=6"
```

9. Validate Redis cache key isolation and expiry directly:

```bash
curl "http://localhost:5034/api/posts?page=1&pageSize=6"
docker exec blog-platform-redis redis-cli keys 'posts:list*'
docker exec blog-platform-redis redis-cli ttl 'posts:list|query:__all__|page:1|size:6|viewer:anonymous'
sleep 32
docker exec blog-platform-redis redis-cli exists 'posts:list|query:__all__|page:1|size:6|viewer:anonymous'
```

   Expected notes:
   - anonymous requests create viewer-safe keys such as
     `posts:list|query:__all__|page:1|size:6|viewer:anonymous`
   - TTL starts at roughly 30 seconds
   - the cache entry is gone after the TTL window expires

## Expected Outcome

- The feed uses paginated backend responses for both default listing and search.
- Infinite-scroll behavior loads additional pages without redesigning the UI.
- Cached read results can be reused for 30 seconds without leaking
  viewer-specific private data.
- Short-lived stale list/search results after writes are accepted and
  documented as an intentional trade-off.
