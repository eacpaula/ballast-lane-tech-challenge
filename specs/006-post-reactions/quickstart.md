# Quickstart: TDD-First Public Post Reactions Core Implementation

## Goal

Implement the smallest possible backend-core slice for public post reactions
without touching API, Infrastructure, database, or frontend code.

## Step 1: Write failing tests first

Create failing tests in these projects:

- `tests/backend/BlogPlatform.Domain.Tests`
- `tests/backend/BlogPlatform.Application.Tests`

Recommended first test cases:

1. Domain accepts `Like` and `Dislike` as valid reaction types.
2. Domain rejects unsupported reaction values.
3. Domain accepts a valid actor identity with a user id or visitor identifier.
4. Domain rejects actor identity input when both forms are missing or invalid.
5. Application accepts a like for a public, available post.
6. Application accepts a dislike for a public, available post.
7. Application rejects missing, non-public, or unavailable posts.
8. Application persists accepted reactions through a reaction repository
   abstraction only.

Concrete first test files:

- `tests/backend/BlogPlatform.Domain.Tests/Reactions/PostReactionTests.cs`
- `tests/backend/BlogPlatform.Application.Tests/Reactions/ReactToPostHandlerSuccessTests.cs`
- `tests/backend/BlogPlatform.Application.Tests/Reactions/ReactToPostHandlerValidationTests.cs`
- `tests/backend/BlogPlatform.Application.Tests/Reactions/ReactToPostHandlerPostAvailabilityTests.cs`

## Step 2: Add only compile-minimum production types

Introduce only the types required to make those tests compile:

- `ReactionType`
- `ReactionActor`
- `PostReaction`
- `ReactToPostCommand`
- `ReactToPostResult`
- `ReactToPostHandler`
- `IPostReactionRepository`
- the minimum reuse or extension needed around `IPostRepository`

Keep these types inside:

- `src/backend/BlogPlatform.Domain`
- `src/backend/BlogPlatform.Application`

Concrete production files:

- `src/backend/BlogPlatform.Domain/Reactions/ReactionType.cs`
- `src/backend/BlogPlatform.Domain/Reactions/ReactionActor.cs`
- `src/backend/BlogPlatform.Domain/Reactions/PostReaction.cs`
- `src/backend/BlogPlatform.Application/Abstractions/IPostReactionRepository.cs`
- `src/backend/BlogPlatform.Application/Posts/ReactToPostCommand.cs`
- `src/backend/BlogPlatform.Application/Posts/ReactToPostResult.cs`
- `src/backend/BlogPlatform.Application/Posts/ReactToPostHandler.cs`

## Step 3: Implement behavior in the correct layer

- Keep reaction-type and actor-identity invariants in Domain.
- Reuse `BlogPost.IsPubliclyReadable` for public-reaction eligibility checks.
- Keep post availability checks and repository orchestration in Application.
- Use test doubles in Application tests instead of real database access.
- Do not introduce controller logic, SQL, authentication middleware, analytics,
  or anti-spam concerns in this slice.

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

Those layers come only after the core reaction use case is stable and
test-covered.
