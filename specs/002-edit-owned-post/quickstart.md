# Quickstart: TDD-First Edit-Owned-Post Core Implementation

## Goal

Implement the smallest possible Domain/Application slice for editing an existing
owned blog post without touching API, Infrastructure, or frontend code.

## Step 1: Write failing tests first

Create failing tests in these projects:

- `tests/backend/BlogPlatform.Domain.Tests`
- `tests/backend/BlogPlatform.Application.Tests`

Recommended first test cases:

1. Domain updates a post with valid title and content/body values.
2. Domain rejects a blank updated title.
3. Domain rejects blank updated content/body.
4. Application rejects a missing authenticated user.
5. Application rejects a missing target post.
6. Application rejects editing a post owned by another user.
7. Application saves and returns an owned-post edit successfully.

## Step 2: Add only compile-minimum production types

Introduce only the types required to make those tests compile:

- `EditBlogPostCommand`
- `EditBlogPostResult`
- `EditBlogPostHandler`
- minimal `BlogPost` update behavior if the existing Domain type does not yet
  support the edit tests
- minimal `IPostRepository` read/update members

Keep these types inside:

- `src/backend/BlogPlatform.Domain`
- `src/backend/BlogPlatform.Application`

## Step 3: Implement behavior in the correct layer

- Keep required title/content invariants in `BlogPost`.
- Keep post lookup, ownership checks, and result shaping in
  `EditBlogPostHandler`.
- Use test doubles in Application tests instead of real database access.
- Reuse the existing post concept from the create-post slice where that reduces
  duplication.

## Step 4: Verify with focused test runs

Run the core test projects repeatedly during red-green-refactor:

```bash
dotnet test tests/backend/BlogPlatform.Domain.Tests/BlogPlatform.Domain.Tests.csproj
dotnet test tests/backend/BlogPlatform.Application.Tests/BlogPlatform.Application.Tests.csproj
```

## Step 5: Defer everything else

Do not add or change:

- `src/backend/BlogPlatform.Api`
- `src/backend/BlogPlatform.Infrastructure`
- `src/frontend/blog-web`
- `database/`

Those layers come only after the core edit-post use case is stable and
test-covered.
