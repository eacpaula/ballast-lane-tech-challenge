# Research: User Registration and Login

## Decision 1: Start with Domain and Application tests only

- **Decision**: The first increment will be implemented with failing tests in
  `BlogPlatform.Domain.Tests` and `BlogPlatform.Application.Tests` only.
- **Rationale**: The constitution requires test-first development and the user
  explicitly asked to keep API, Infrastructure, and frontend out of the first
  implementation unless strictly necessary. Registration and login rules can be
  proven at the Domain/Application boundary first.
- **Alternatives considered**:
  - Start with API integration tests: rejected because it would pull HTTP and
    middleware concerns into the first increment.
  - Start with repository tests: rejected because raw SQL behavior is not
    needed yet to prove validation, hashing, duplicate-email, and login rules.

## Decision 2: Introduce a small user account Domain concept

- **Decision**: Add a small `UserAccount` Domain concept for persisted user
  identity and password-hash state, instead of embedding all registration and
  login data rules inside Application handlers.
- **Rationale**: Required field rules and the guarantee that only hashed
  passwords are persisted belong in a business model that is easy to test and
  explain.
- **Alternatives considered**:
  - Keep all validation in Application only: rejected because it would weaken
    Domain responsibility for core user data invariants.
  - Introduce a full identity framework model: rejected as unnecessary
    complexity for an interview-scale feature.

## Decision 3: Keep duplicate-email checks and credential verification in Application

- **Decision**: Registration handlers will query for existing users by email
  before creating a new account, and login handlers will load a user by email
  before verifying the submitted password hash.
- **Rationale**: Duplicate-email checks and credential verification depend on
  both caller input and persisted state, making them Application concerns rather
  than controller or repository-owned rules.
- **Alternatives considered**:
  - Validate duplicates in the API layer: rejected because the constitution
    requires business validation outside controllers.
  - Hide duplicate checks in persistence only: rejected because it obscures a
    core business rule inside data-access details.

## Decision 4: Use a single password abstraction for hash and verify behavior

- **Decision**: Use one password security abstraction with the minimum behavior
  needed to hash a registration password and verify a login password.
- **Rationale**: Hashing and verification are logically paired and should stay
  behind a simple boundary in the first increment.
- **Alternatives considered**:
  - Separate hasher and verifier abstractions: rejected because it adds
    unnecessary moving parts without improving clarity in this MVP slice.
  - Perform hashing directly in Application: rejected because it couples core
    business behavior to implementation details.

## Decision 5: Use an authentication payload abstraction instead of JWT details

- **Decision**: Successful login will rely on an abstraction that produces
  authentication success data without binding Application to JWT or HTTP
  transport details.
- **Rationale**: The feature needs a login success result that later outer
  layers can issue or return as a token, but the first increment must avoid full
  JWT middleware setup.
- **Alternatives considered**:
  - Generate JWTs directly in Application: rejected because it brings API and
    infrastructure security details into the wrong layer.
  - Return only user data with no auth payload abstraction: rejected because the
    feature explicitly requires token or auth result generation through an
    abstraction.

## Decision 6: Model duplicate-email and invalid-credential failures explicitly

- **Decision**: Duplicate email and invalid credentials will be expressed as
  explicit registration/login business outcomes in Application.
- **Rationale**: These are the critical negative paths and are easier to test,
  reason about, and map later than exception-driven control flow.
- **Alternatives considered**:
  - Throw exceptions for all failure cases: rejected because it makes expected
    business outcomes noisier in tests.
  - Expose repository-specific failure behavior directly: rejected because it
    blurs business rules with storage concerns.
