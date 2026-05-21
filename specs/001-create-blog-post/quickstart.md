# Quickstart: TDD-First Core Implementation

## Goal

Implement the smallest possible Domain/Application slice for authenticated blog
post creation without touching API, Infrastructure, or frontend code.

## Step 1: Write failing tests first

Create or replace the placeholder tests in these projects:

- `tests/backend/BlogPlatform.Domain.Tests`
- `tests/backend/BlogPlatform.Application.Tests`

Recommended first test cases:

1. Domain creates a valid `BlogPost` when title and content are present.
2. Domain rejects a blank title.
3. Domain rejects blank content.
4. Application rejects a missing or unavailable category.
5. Application rejects a missing authenticated user before category lookup or persistence.
6. Application saves a valid post with the authenticated user as owner and
   returns the created result.

Concrete first test files:

- `tests/backend/BlogPlatform.Domain.Tests/Posts/BlogPostTests.cs`
- `tests/backend/BlogPlatform.Application.Tests/Posts/CreateBlogPostHandlerSuccessTests.cs`
- `tests/backend/BlogPlatform.Application.Tests/Posts/CreateBlogPostHandlerValidationTests.cs`
- `tests/backend/BlogPlatform.Application.Tests/Posts/CreateBlogPostHandlerAuthenticationTests.cs`

## Step 2: Add only compile-minimum production types

Introduce only the types required to make those tests compile:

- `BlogPost`
- `CreateBlogPostCommand`
- `CreateBlogPostResult`
- `CreateBlogPostHandler`
- `ICategoryRepository`
- `IPostRepository`

Keep these types inside:

- `src/backend/BlogPlatform.Domain`
- `src/backend/BlogPlatform.Application`

Concrete production files:

- `src/backend/BlogPlatform.Domain/Posts/BlogPost.cs`
- `src/backend/BlogPlatform.Application/Posts/CreateBlogPostCommand.cs`
- `src/backend/BlogPlatform.Application/Posts/CreateBlogPostResult.cs`
- `src/backend/BlogPlatform.Application/Posts/CreateBlogPostHandler.cs`
- `src/backend/BlogPlatform.Application/Abstractions/IPostRepository.cs`
- `src/backend/BlogPlatform.Application/Abstractions/ICategoryRepository.cs`

## Step 3: Implement behavior in the correct layer

- Put title/content invariants in `BlogPost`.
- Put category lookup and persistence orchestration in
  `CreateBlogPostHandler`.
- Use test doubles in Application tests instead of real database access.

## Step 4: Verify with focused test runs

Run the core test projects repeatedly during red-green-refactor:

```bash
dotnet test tests/backend/BlogPlatform.Domain.Tests/BlogPlatform.Domain.Tests.csproj
dotnet test tests/backend/BlogPlatform.Application.Tests/BlogPlatform.Application.Tests.csproj
```

Run both together before closing the increment:

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

Those layers come only after the core use case is stable and test-covered.
