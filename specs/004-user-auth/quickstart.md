# Quickstart: TDD-First User Registration and Login Core Implementation

## Goal

Implement the smallest possible Domain/Application slice for user registration
and login without touching API, Infrastructure, database, JWT middleware, or
frontend code.

## Step 1: Write failing tests first

Create failing tests in these projects:

- `tests/backend/BlogPlatform.Domain.Tests`
- `tests/backend/BlogPlatform.Application.Tests`

Recommended first test cases:

1. Domain creates a user account with valid name or username, email, and stored
   password hash values.
2. Domain rejects blank name or username values.
3. Domain rejects invalid email values.
4. Domain rejects blank stored password hash values.
5. Application rejects duplicate email registration.
6. Application hashes a valid registration password before persisting a new
   user.
7. Application rejects invalid registration input.
8. Application rejects login for unknown email addresses.
9. Application rejects login for incorrect passwords.
10. Application returns authentication result data for valid login.

Concrete first test files:

- `tests/backend/BlogPlatform.Domain.Tests/Users/UserAccountTests.cs`
- `tests/backend/BlogPlatform.Application.Tests/Users/RegisterUserHandlerSuccessTests.cs`
- `tests/backend/BlogPlatform.Application.Tests/Users/RegisterUserHandlerDuplicateEmailTests.cs`
- `tests/backend/BlogPlatform.Application.Tests/Users/RegisterUserHandlerValidationTests.cs`
- `tests/backend/BlogPlatform.Application.Tests/Users/LoginUserHandlerSuccessTests.cs`
- `tests/backend/BlogPlatform.Application.Tests/Users/LoginUserHandlerInvalidCredentialsTests.cs`

## Step 2: Add only compile-minimum production types

Introduce only the types required to make those tests compile:

- `UserAccount`
- `RegisterUserCommand`
- `RegisterUserResult`
- `RegisterUserHandler`
- `LoginUserCommand`
- `LoginUserResult`
- `LoginUserHandler`
- `IUserRepository`
- `IPasswordSecurityService`
- `IAuthenticationPayloadFactory`

Keep these types inside:

- `src/backend/BlogPlatform.Domain`
- `src/backend/BlogPlatform.Application`

Concrete production files:

- `src/backend/BlogPlatform.Domain/Users/UserAccount.cs`
- `src/backend/BlogPlatform.Application/Users/RegisterUserCommand.cs`
- `src/backend/BlogPlatform.Application/Users/RegisterUserResult.cs`
- `src/backend/BlogPlatform.Application/Users/RegisterUserHandler.cs`
- `src/backend/BlogPlatform.Application/Users/LoginUserCommand.cs`
- `src/backend/BlogPlatform.Application/Users/LoginUserResult.cs`
- `src/backend/BlogPlatform.Application/Users/LoginUserHandler.cs`
- `src/backend/BlogPlatform.Application/Abstractions/IUserRepository.cs`
- `src/backend/BlogPlatform.Application/Abstractions/IPasswordSecurityService.cs`
- `src/backend/BlogPlatform.Application/Abstractions/IAuthenticationPayloadFactory.cs`

## Step 3: Implement behavior in the correct layer

- Keep user-account validation and stored-password-hash safety in `UserAccount`.
- Keep duplicate-email checks, password hashing and verification orchestration,
  and authentication result shaping in the registration and login handlers.
- Use test doubles in Application tests instead of real database or token
  generation implementations.
- Do not introduce controller logic, SQL, JWT middleware, or transport concerns
  in this slice.

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

Those layers come only after the core registration and login use cases are
stable and test-covered.

## Current implementation note

The current core slice is satisfied by Domain and Application tests and
production code only. JWT middleware, API endpoints, persistence
implementations, and frontend behavior remain deferred.
