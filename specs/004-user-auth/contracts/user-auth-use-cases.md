# User Registration and Login Use Case Contract

## Purpose

This contract defines the Application-layer boundaries for the first
implementation increment. It stays independent of HTTP and JWT middleware so
registration and login rules can be implemented and tested before API wiring.

## Registration Input Contract

### RegisterUserCommand

- `NameOrUsername`: required non-blank user name value
- `Email`: required valid email address
- `Password`: required valid password input

## Registration Success Contract

On success, the use case returns a `RegisterUserResult` containing:

- `UserId`
- `NameOrUsername`
- `Email`

## Registration Failure Contract

The use case can fail with one of these business outcomes:

- `ValidationError`: registration input is invalid
- `DuplicateEmail`: the submitted email address is already in use

## Login Input Contract

### LoginUserCommand

- `Email`: required login email address
- `Password`: required login password input

## Login Success Contract

On success, the use case returns a `LoginUserResult` containing:

- `UserId`
- `NameOrUsername`
- `Email`
- `AuthenticationPayload`

## Login Failure Contract

The use case can fail with one of these business outcomes:

- `InvalidCredentials`: the email address does not exist or the password is
  incorrect
- `ValidationError`: login input is invalid

## Deferred Outer-Boundary Behavior

The following behavior is required by the overall feature but is intentionally
deferred to a later API or persistence increment:

- HTTP endpoint handling for registration and login
- JWT middleware and bearer authentication setup
- Translating registration/login failures into HTTP responses
- Executing persistence with raw SQL and Npgsql
