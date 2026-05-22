# Quickstart: Paginate Categories and Support Category Descriptions

## Goal

Validate that category lists now return paginated data and that administrator
category create and update flows persist optional descriptions without changing
the current authorization rules.

## Preconditions

- Docker and Docker Compose are available locally
- PostgreSQL, API, and frontend services can start from the repository
- The existing category management flow is already available as the baseline

## Validation Steps

1. Start the local stack:

```bash
docker compose up -d postgres api frontend
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

4. Validate available-category pagination:
   - call `GET /api/categories/available?page=1&pageSize=10`
   - confirm the response returns `items` plus pagination metadata
   - confirm only available categories appear

5. Validate administrator category pagination:
   - authenticate as the seeded administrator
   - call `GET /api/categories?page=1&pageSize=10`
   - confirm the response returns `items` plus pagination metadata
   - confirm unavailable categories are still visible to the admin list when
     that is the current management expectation

6. Validate description create and update behavior:
   - create a category with both title and description
   - update the category description
   - clear the description and confirm the category remains valid

7. Validate frontend behavior:
   - open the administrator category management screen
   - confirm the description field is editable
   - confirm category list rendering still works with the paginated API
   - open a post create or edit flow and confirm category loading still works
     against the available-category endpoint

8. Validate authorization behavior:
   - confirm non-admin users cannot create, update, or deactivate categories
   - confirm the available-category read path remains read-only

## Expected Outcome

- Category lists use the project’s paginated response style.
- Administrator category writes support optional descriptions.
- Existing authorization and duplicate-title rules remain intact.
- The frontend adapts to the revised category contracts without a broader UI
  redesign.
