# Quickstart: TDD-First Remove-Owned-Post Core Implementation

## Goal

Implement the smallest possible Domain/Application slice for removing an
existing owned blog post without touching API, Infrastructure, database, or
frontend code.

## Step 1: Write failing tests first

Create failing tests in this project:

- `tests/backend/BlogPlatform.Application.Tests`

Recommended first test cases:

1. Application rejects a missing authenticated user.
2. Application rejects a missing target post.
3. Application rejects removing a post owned by another user.
4. Application removes and returns an owned post successfully.
5. Application invokes the repository delete behavior only after ownership and
   existence checks pass.

Concrete first test files:

- `tests/backend/BlogPlatform.Application.Tests/Posts/RemoveBlogPostHandlerSuccessTests.cs`
- `tests/backend/BlogPlatform.Application.Tests/Posts/RemoveBlogPostHandlerMissingPostTests.cs`
- `tests/backend/BlogPlatform.Application.Tests/Posts/RemoveBlogPostHandlerOwnershipTests.cs`
- `tests/backend/BlogPlatform.Application.Tests/Posts/RemoveBlogPostHandlerAuthenticationTests.cs`

Add Domain tests only if implementation reveals a real removal-specific Domain
behavior worth protecting.

## Step 2: Add only compile-minimum production types

Introduce only the types required to make those tests compile:

- `RemoveBlogPostCommand`
- `RemoveBlogPostResult`
- `RemoveBlogPostHandler`
- minimal `IPostRepository` delete/read members

Keep these types inside:

- `src/backend/BlogPlatform.Application`

Concrete production files:

- `src/backend/BlogPlatform.Application/Posts/RemoveBlogPostCommand.cs`
- `src/backend/BlogPlatform.Application/Posts/RemoveBlogPostResult.cs`
- `src/backend/BlogPlatform.Application/Posts/RemoveBlogPostHandler.cs`
- `src/backend/BlogPlatform.Application/Abstractions/IPostRepository.cs`

## Step 3: Implement behavior in the correct layer

- Reuse the existing `BlogPost` concept as the loaded post record.
- Keep post lookup, ownership checks, and result shaping in
  `RemoveBlogPostHandler`.
- Use test doubles in Application tests instead of real database access.
- Do not introduce controller logic, SQL, or transport concerns in this slice.

## Step 4: Verify with focused test runs

Run the core test project repeatedly during red-green-refactor:

```bash
dotnet test tests/backend/BlogPlatform.Application.Tests/BlogPlatform.Application.Tests.csproj
```

If Domain tests are added later for a justified reason, run:

```bash
dotnet test tests/backend/BlogPlatform.Domain.Tests/BlogPlatform.Domain.Tests.csproj
```

## Step 5: Defer everything else

Do not add or change:

- `src/backend/BlogPlatform.Api`
- `src/backend/BlogPlatform.Infrastructure`
- `src/frontend/blog-web`
- `database/`

Those layers come only after the core remove-post use case is stable and
test-covered.

## Current implementation note

The current core slice is satisfied by Application-layer tests and production
code only. No remove-post-specific Domain behavior was required.
