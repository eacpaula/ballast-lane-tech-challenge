# Quickstart: Raw SQL Repository Integration-First Implementation

## Goal

Implement PostgreSQL-backed Infrastructure repositories for the existing
Application abstractions by proving repository behavior with real-database
integration tests first.

## Step 1: Start with failing Infrastructure integration tests

Create failing tests in:

- `tests/backend/BlogPlatform.Infrastructure.Tests`

Recommended first test groups:

1. User repository create and lookup-by-email behavior.
2. Post repository create, load, update, delete, list-public, and
   get-public-by-id behavior.
3. Category repository availability, duplicate-title, create, load, update,
   and deactivate behavior.
4. Post reaction repository create behavior.

## Step 2: Add only the compile-minimum Infrastructure types

Introduce only the types required to make those tests compile, such as:

- a small PostgreSQL connection/settings helper in
  `src/backend/BlogPlatform.Infrastructure`
- repository classes implementing:
  - `IUserRepository`
  - `IPostRepository`
  - `ICategoryRepository`
  - `IPostReactionRepository`
- optional Infrastructure DI registration if it materially helps test setup or
  later composition

## Step 3: Reuse the existing database environment

Use the existing Docker Compose PostgreSQL environment and the current schema
and seed scripts under `database/`.

Repository tests should use a deterministic reset or cleanup strategy so data
changes from one test do not leak into the next.

## Step 4: Keep logic in the correct layer

- Repositories persist and retrieve data only.
- Business validation remains in Application.
- Keep SQL parameterized, explicit, and readable.
- Do not introduce Entity Framework, Dapper, Mediator, MediatR, or a migration
  framework.

## Step 5: Validate the slice

Run Infrastructure integration tests against PostgreSQL first.

Then rerun:

```bash
dotnet test tests/backend/BlogPlatform.Infrastructure.Tests/BlogPlatform.Infrastructure.Tests.csproj
dotnet test tests/backend/BlogPlatform.Application.Tests/BlogPlatform.Application.Tests.csproj
```

The Application test rerun is only to confirm that shared abstractions still
support the existing handlers after Infrastructure work is added.
