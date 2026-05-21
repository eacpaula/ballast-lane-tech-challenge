# Quickstart: TDD-First Public Read Posts Core Implementation

## Goal

Implement the smallest possible backend-core slice for public post listing and
public post detail reads without touching API, Infrastructure, database, or
frontend code.

## Step 1: Write failing tests first

Create failing tests in this project:

- `tests/backend/BlogPlatform.Application.Tests`

Recommended first test cases:

1. Application returns public, available posts in the public listing.
2. Application excludes non-public posts from the public listing.
3. Application excludes unavailable posts from the public listing.
4. Application returns a public, available post for detail reads.
5. Application rejects missing, non-public, or unavailable posts for detail
   reads.
6. Application public read flows do not require authentication input.

Concrete first test files:

- `tests/backend/BlogPlatform.Application.Tests/Posts/ListPublicPostsHandlerTests.cs`
- `tests/backend/BlogPlatform.Application.Tests/Posts/GetPublicPostByIdHandlerTests.cs`

This implementation required a small `BlogPost` state extension for `IsPublic`
and `IsAvailable`, so Application tests remain the primary driver while Domain
state is kept explicit instead of hidden inside repository behavior.

## Step 2: Add only compile-minimum production types

Introduce only the types required to make those tests compile:

- `PublicPostListItem`
- `GetPublicPostByIdResult`
- `ListPublicPostsHandler`
- `GetPublicPostByIdHandler`
- minimal `IPostRepository` public-read members
- minimal post visibility/availability state support only if tests require it

Keep these types inside:

- `src/backend/BlogPlatform.Application`
- optionally `src/backend/BlogPlatform.Domain` only if needed by tests

Concrete production files:

- `src/backend/BlogPlatform.Application/Posts/PublicPostListItem.cs`
- `src/backend/BlogPlatform.Application/Posts/GetPublicPostByIdResult.cs`
- `src/backend/BlogPlatform.Application/Posts/ListPublicPostsHandler.cs`
- `src/backend/BlogPlatform.Application/Posts/GetPublicPostByIdHandler.cs`
- `src/backend/BlogPlatform.Application/Abstractions/IPostRepository.cs`
- `src/backend/BlogPlatform.Domain/Posts/BlogPost.cs`

## Step 3: Implement behavior in the correct layer

- Keep public filtering outcomes and not-available detail handling in the
  Application handlers.
- Reuse the existing post concept and make the minimum explicit Domain change
  needed to represent `IsPublic` and `IsAvailable`.
- Use test doubles in Application tests instead of real database access.
- Do not introduce controller logic, SQL, authentication, or transport concerns
  in this slice.

## Step 4: Verify with focused test runs

Run the core test project repeatedly during red-green-refactor:

```bash
dotnet test tests/backend/BlogPlatform.Application.Tests/BlogPlatform.Application.Tests.csproj
```

If Domain tests are added later for a justified reason, run:

```bash
dotnet test tests/backend/BlogPlatform.Domain.Tests/BlogPlatform.Domain.Tests.csproj
```

For the current implementation, both suites should pass:

```bash
dotnet test tests/backend/BlogPlatform.Application.Tests/BlogPlatform.Application.Tests.csproj
dotnet test tests/backend/BlogPlatform.Domain.Tests/BlogPlatform.Domain.Tests.csproj
```

## Step 5: Defer everything else

Do not add or change:

- `src/backend/BlogPlatform.Api`
- `src/backend/BlogPlatform.Infrastructure`
- `src/frontend/blog-web`
- `database/`

Those layers come only after the core public-read use cases are stable and
test-covered.
