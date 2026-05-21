# Quickstart: API Integration-First Implementation

## Goal

Expose the existing backend use cases through ASP.NET Core Web API controllers
by proving the HTTP surface with failing API integration tests first.

## Step 1: Start with failing API integration tests

Create failing tests in:

- `tests/backend/BlogPlatform.Api.Tests`

Recommended first test groups:

1. Auth registration and login.
2. Public post list and detail reads.
3. Public post reaction submission.
4. Authenticated post create, edit, and remove flows.
5. Administrator category management flows.
6. ProblemDetails-style error and authorization outcomes.

## Step 2: Add only the compile-minimum API types

Introduce only the types required to make those tests compile, such as:

- controller classes in `src/backend/BlogPlatform.Api`
- API request and response DTOs
- API composition and dependency registration updates in `Program.cs`
- JWT configuration support
- ProblemDetails-style error translation support

Add only the minimum supporting Infrastructure auth services needed by the API
if the branch does not already contain them.

## Step 3: Keep logic in the correct layer

- Controllers translate HTTP input and authenticated user context only.
- Application handlers remain the source of business validation and ownership
  decisions.
- Infrastructure is wired through dependency injection only.
- Do not introduce Entity Framework, Dapper, MediatR, external identity
  providers, or advanced API versioning.

## Step 4: Validate the slice

Run API integration tests first.

Then rerun:

```bash
dotnet test tests/backend/BlogPlatform.Api.Tests/BlogPlatform.Api.Tests.csproj
dotnet test tests/backend/BlogPlatform.Application.Tests/BlogPlatform.Application.Tests.csproj
dotnet test tests/backend/BlogPlatform.Infrastructure.Tests/BlogPlatform.Infrastructure.Tests.csproj
```

Also verify local OpenAPI discovery after authentication and controller wiring
are in place.
