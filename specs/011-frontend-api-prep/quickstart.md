# Quickstart: Backend Preparation for Frontend Integration

## Goal

Prepare the existing API so a local React frontend can call it with minimal
setup friction.

## Step 1: Start with failing tests

Create failing tests before production code:

1. Application test for listing only available post categories.
2. Repository integration test for raw SQL available-category filtering.
3. API integration test for anonymous `GET /api/categories/available`.
4. API integration test proving unavailable categories are excluded.
5. Regression check for admin-only category write endpoints.

## Step 2: Add only the compile-minimum backend types

Introduce only the minimum types required to satisfy those tests:

- one Application query/handler for available categories
- one repository abstraction method and its raw SQL implementation
- one API response DTO
- one public read-only API action for category listing
- API runtime configuration changes for Docker and local CORS
- Compose/Docker artifacts needed to run the API next to PostgreSQL

## Step 3: Keep responsibilities in the correct layer

- Controllers translate HTTP requests only.
- Application owns the category-list use case.
- Infrastructure owns raw SQL and container-safe connectivity only through DI.
- Existing admin category management stays separate from public read behavior.
- Do not add frontend code, tag listing, production deployment features, or new
  business scope in this slice.

## Step 4: Validate local runtime readiness

Run the automated tests first:

```bash
dotnet test tests/backend/BlogPlatform.Application.Tests/BlogPlatform.Application.Tests.csproj
dotnet test tests/backend/BlogPlatform.Infrastructure.Tests/BlogPlatform.Infrastructure.Tests.csproj
dotnet test tests/backend/BlogPlatform.Api.Tests/BlogPlatform.Api.Tests.csproj
```

Then verify the local backend runtime:

```bash
docker compose up -d postgres api
docker compose ps
```

Finally confirm:

- Swagger is reachable through the API service URL
- `GET /api/categories/available` returns only available categories
- Existing admin category write routes still require administrator access
