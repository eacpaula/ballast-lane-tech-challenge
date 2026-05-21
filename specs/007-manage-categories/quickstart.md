# Quickstart: TDD-First Administrator Category Management Core Implementation

## Goal

Implement the smallest possible backend-core slice for administrator category
management without touching API, Infrastructure, database, or frontend code.

## Step 1: Write failing tests first

Create failing tests in these projects:

- `tests/backend/BlogPlatform.Domain.Tests`
- `tests/backend/BlogPlatform.Application.Tests`

Recommended first test cases:

1. Domain creates a category with a normalized valid title.
2. Domain rejects blank or invalid category titles.
3. Domain updates a persisted category title and preserves availability state.
4. Domain deactivates a persisted category and marks it unavailable.
5. Application allows an administrator to create a category with a valid title.
6. Application allows an administrator to update a category with a valid
   non-duplicate title.
7. Application allows an administrator to deactivate a category.
8. Application rejects non-admin create, update, and deactivate attempts.
9. Application rejects duplicate titles on create and update.
10. Application rejects update and deactivate requests for missing categories.

Concrete first test files:

- `tests/backend/BlogPlatform.Domain.Tests/Categories/PostCategoryTests.cs`
- `tests/backend/BlogPlatform.Application.Tests/Categories/CreatePostCategoryHandlerTests.cs`
- `tests/backend/BlogPlatform.Application.Tests/Categories/UpdatePostCategoryHandlerTests.cs`
- `tests/backend/BlogPlatform.Application.Tests/Categories/DeactivatePostCategoryHandlerTests.cs`

## Step 2: Add only compile-minimum production types

Introduce only the types required to make those tests compile:

- `PostCategory`
- `CreatePostCategoryCommand`
- `UpdatePostCategoryCommand`
- `DeactivatePostCategoryCommand`
- category-management result models
- category-management handlers
- the minimum `ICategoryRepository` members needed for admin category use cases

Keep these types inside:

- `src/backend/BlogPlatform.Domain`
- `src/backend/BlogPlatform.Application`

Concrete production files:

- `src/backend/BlogPlatform.Domain/Categories/PostCategory.cs`
- `src/backend/BlogPlatform.Application/Posts/CreatePostCategoryCommand.cs`
- `src/backend/BlogPlatform.Application/Posts/UpdatePostCategoryCommand.cs`
- `src/backend/BlogPlatform.Application/Posts/DeactivatePostCategoryCommand.cs`
- `src/backend/BlogPlatform.Application/Posts/PostCategoryManagementResult.cs`
- `src/backend/BlogPlatform.Application/Posts/CreatePostCategoryHandler.cs`
- `src/backend/BlogPlatform.Application/Posts/UpdatePostCategoryHandler.cs`
- `src/backend/BlogPlatform.Application/Posts/DeactivatePostCategoryHandler.cs`
- `src/backend/BlogPlatform.Application/Abstractions/ICategoryRepository.cs`

## Step 3: Implement behavior in the correct layer

- Keep title normalization and category availability state in Domain.
- Keep administrator authorization, duplicate-title checks, and missing-category
  handling in Application.
- Reuse the existing authenticated-user style input pattern with a minimal admin
  indicator instead of building full role management.
- Use test doubles in Application tests instead of real database access.
- Do not introduce controller logic, SQL, JWT configuration, or frontend
  concerns in this slice.

## Step 4: Verify with focused test runs

Run the core test projects repeatedly during red-green-refactor:

```bash
dotnet test tests/backend/BlogPlatform.Domain.Tests/BlogPlatform.Domain.Tests.csproj
dotnet test tests/backend/BlogPlatform.Application.Tests/BlogPlatform.Application.Tests.csproj
```

Both commands pass for the current implementation slice.

## Step 5: Defer everything else

Do not add or change:

- `src/backend/BlogPlatform.Api`
- `src/backend/BlogPlatform.Infrastructure`
- `src/frontend/blog-web`
- `database/`

Those layers come only after the core admin category-management use cases are
stable and test-covered.
