# Data Model: User Registration and Login

## Overview

The first increment needs only the Domain and Application data required to
validate registration input, persist a user account with a password hash,
verify login credentials, and return authentication result data. Persistence-
specific details and HTTP-specific contracts stay out of this model.

## Entities

### UserAccount

- **Purpose**: Represents a registered user account that can authenticate with
  an email address and stored password hash.
- **Core fields**:
  - `Id`: existing persisted identifier
  - `NameOrUsername`: required display or login name
  - `Email`: required normalized email address
  - `PasswordHash`: required stored password hash only
- **Relationships**:
  - One user account can later own many blog posts
- **Validation rules**:
  - `NameOrUsername` must contain non-whitespace content
  - `Email` must be in a valid format for the application's rules
  - `PasswordHash` must contain non-whitespace content when a user is persisted
- **State transitions**:
  - `Registration input` -> `Validated new account with password hash`
  - `Persisted account` -> `Authenticated account result`

## Application Boundary Models

### RegisterUserCommand

- **Purpose**: Carries the use case input for registering a new user.
- **Fields**:
  - `NameOrUsername`
  - `Email`
  - `Password`

### RegisterUserResult

- **Purpose**: Returns either the successful registration outcome or a business
  failure outcome without leaking persistence details.
- **Fields**:
  - `UserId`
  - `NameOrUsername`
  - `Email`
  - `ErrorCode`
  - `ErrorMessage`

### LoginUserCommand

- **Purpose**: Carries the use case input for authenticating an existing user.
- **Fields**:
  - `Email`
  - `Password`

### LoginUserResult

- **Purpose**: Returns either the successful authentication outcome or an
  invalid-credentials business failure without leaking transport details.
- **Fields**:
  - `UserId`
  - `NameOrUsername`
  - `Email`
  - `AuthenticationPayload`
  - `ErrorCode`
  - `ErrorMessage`

### AuthenticationPayload

- **Purpose**: Represents the authentication success data produced by an
  abstraction and later consumable by outer layers to issue or return a token.
- **Fields**:
  - `AccessToken` or equivalent opaque authentication value
  - optional expiration or metadata fields if needed by the abstraction

## Repository Abstractions

### IUserRepository

- **Purpose**: Supports duplicate-email checks, user creation, and login lookup.
- **Minimum behavior needed**:
  - Read a user by email
  - Create a new user account

## Security Abstractions

### IPasswordSecurityService

- **Purpose**: Supports password hashing for registration and password
  verification for login.
- **Minimum behavior needed**:
  - Hash a raw password
  - Verify a raw password against a stored hash

### IAuthenticationPayloadFactory

- **Purpose**: Produces authentication success data for a valid login without
  binding Application to JWT or HTTP concerns.
- **Minimum behavior needed**:
  - Build an authentication payload for an authenticated user

## Invariants to Prove with Tests

- Registration rejects blank name or username values.
- Registration rejects invalid email values.
- Registration rejects invalid password values.
- Registration rejects duplicate email addresses.
- Registration persists a password hash rather than the raw password.
- Login rejects unknown email addresses.
- Login rejects incorrect passwords.
- Successful login returns authentication result data from the payload
  abstraction.
