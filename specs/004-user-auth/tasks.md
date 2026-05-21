---

description: "Task list for user registration and login"

---

# Tasks: User Registration and Login

**Input**: Design documents from `/specs/004-user-auth/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/, quickstart.md

**Tests**: Backend tests are mandatory. This feature follows a TDD-first workflow with Domain and Application unit tests written and failing before production code.

**Organization**: Tasks are grouped by user story so registration can be delivered as the MVP and login can be added as a second independently testable increment.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel when the task touches a different file and does not depend on incomplete work
- **[Story]**: Maps work to the feature user story
- Every task includes the exact file path to change

## Path Conventions

- **Domain**: `src/backend/BlogPlatform.Domain/`
- **Application**: `src/backend/BlogPlatform.Application/`
- **Domain tests**: `tests/backend/BlogPlatform.Domain.Tests/`
- **Application tests**: `tests/backend/BlogPlatform.Application.Tests/`
- **Feature docs**: `specs/004-user-auth/`

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Prepare the minimal test and source structure for the auth slice without expanding scope.

- [ ] T001 Create user-domain test folder structure in `tests/backend/BlogPlatform.Domain.Tests/Users/`
- [ ] T002 Create user-application test folder structure in `tests/backend/BlogPlatform.Application.Tests/Users/`
- [ ] T003 Create user source folder structure in `src/backend/BlogPlatform.Domain/Users/` and `src/backend/BlogPlatform.Application/Users/`

---

## Phase 2: Foundational Backend (Blocking Prerequisites)

**Purpose**: Identify the minimum shared auth types and boundaries before story-level TDD work begins.

**CRITICAL**: No registration or login behavior is implemented until the shared Domain/Application scope is clear.

- [ ] T004 Review shared auth model scope in `specs/004-user-auth/data-model.md`
- [ ] T005 Review shared security and repository boundary scope in `specs/004-user-auth/contracts/user-auth-use-cases.md`
- [ ] T006 Review existing Application abstraction patterns in `src/backend/BlogPlatform.Application/Abstractions/IPostRepository.cs`

**Checkpoint**: The project is ready for story-specific TDD work.

---

## Phase 3: User Story 1 - Visitor Registers a New User Account (Priority: P1) 🎯 MVP

**Goal**: Allow a visitor to register a new account with valid data, reject invalid or duplicate email registration, hash the password before persistence, and save the user through abstractions only.

**Independent Test**: The story is complete when Domain and Application tests pass for valid registration, invalid name or username, invalid email, invalid password, duplicate email rejection, password hashing before persistence, and repository-backed user creation.

### Tests for User Story 1 (MANDATORY) ⚠️

> Write these tests first and verify they fail before implementation.

- [ ] T007 [P] [US1] Add Domain tests for valid user creation and invalid name or username, email, and password-hash state in `tests/backend/BlogPlatform.Domain.Tests/Users/UserAccountTests.cs`
- [ ] T008 [P] [US1] Add failing Application test for successful registration with hashed password persistence in `tests/backend/BlogPlatform.Application.Tests/Users/RegisterUserHandlerSuccessTests.cs`
- [ ] T009 [P] [US1] Add failing Application test for duplicate email rejection in `tests/backend/BlogPlatform.Application.Tests/Users/RegisterUserHandlerDuplicateEmailTests.cs`
- [ ] T010 [P] [US1] Add failing Application tests for invalid registration input in `tests/backend/BlogPlatform.Application.Tests/Users/RegisterUserHandlerValidationTests.cs`
- [ ] T011 [P] [US1] Add failing Application assertion coverage for password hashing before repository create in `tests/backend/BlogPlatform.Application.Tests/Users/RegisterUserHandlerSuccessTests.cs`

### Backend Implementation for User Story 1

- [ ] T012 [P] [US1] Create user account Domain model in `src/backend/BlogPlatform.Domain/Users/UserAccount.cs`
- [ ] T013 [P] [US1] Create user repository abstraction in `src/backend/BlogPlatform.Application/Abstractions/IUserRepository.cs`
- [ ] T014 [P] [US1] Create password security abstraction in `src/backend/BlogPlatform.Application/Abstractions/IPasswordSecurityService.cs`
- [ ] T015 [P] [US1] Create register-user command input model in `src/backend/BlogPlatform.Application/Users/RegisterUserCommand.cs`
- [ ] T016 [P] [US1] Create register-user result model in `src/backend/BlogPlatform.Application/Users/RegisterUserResult.cs`
- [ ] T017 [US1] Implement registration orchestration in `src/backend/BlogPlatform.Application/Users/RegisterUserHandler.cs`
- [ ] T018 [US1] Refactor registration validation and result handling in `src/backend/BlogPlatform.Domain/Users/UserAccount.cs`, `src/backend/BlogPlatform.Application/Users/RegisterUserHandler.cs`, and `src/backend/BlogPlatform.Application/Users/RegisterUserResult.cs`
- [ ] T019 [US1] Re-run focused TDD test suite for `tests/backend/BlogPlatform.Domain.Tests/Users/UserAccountTests.cs` and `tests/backend/BlogPlatform.Application.Tests/Users/RegisterUserHandler*.cs`

**Checkpoint**: User Story 1 is functional within Domain and Application and can be demonstrated without API, Infrastructure, database, JWT middleware, or frontend work.

---

## Phase 4: User Story 2 - Registered User Logs In With Valid Credentials (Priority: P2)

**Goal**: Allow a registered user to log in with a valid email and password, reject unknown users and incorrect passwords, verify credentials through abstractions, and return authentication result data suitable for later token-based API authentication.

**Independent Test**: The story is complete when Application tests pass for successful login, unknown-user rejection, incorrect-password rejection, password verification through the security abstraction, and authentication result creation through an abstraction.

### Tests for User Story 2 (MANDATORY) ⚠️

- [ ] T020 [P] [US2] Add failing Application test for successful login with authentication result data in `tests/backend/BlogPlatform.Application.Tests/Users/LoginUserHandlerSuccessTests.cs`
- [ ] T021 [P] [US2] Add failing Application test for unknown-user login rejection in `tests/backend/BlogPlatform.Application.Tests/Users/LoginUserHandlerInvalidCredentialsTests.cs`
- [ ] T022 [P] [US2] Add failing Application test for incorrect-password rejection in `tests/backend/BlogPlatform.Application.Tests/Users/LoginUserHandlerInvalidCredentialsTests.cs`
- [ ] T023 [P] [US2] Add failing Application assertion coverage for password verification and authentication payload generation abstractions in `tests/backend/BlogPlatform.Application.Tests/Users/LoginUserHandlerSuccessTests.cs`

### Backend Implementation for User Story 2

- [ ] T024 [P] [US2] Create authentication payload factory abstraction in `src/backend/BlogPlatform.Application/Abstractions/IAuthenticationPayloadFactory.cs`
- [ ] T025 [P] [US2] Create login-user command input model in `src/backend/BlogPlatform.Application/Users/LoginUserCommand.cs`
- [ ] T026 [P] [US2] Create login-user result model in `src/backend/BlogPlatform.Application/Users/LoginUserResult.cs`
- [ ] T027 [US2] Implement login orchestration in `src/backend/BlogPlatform.Application/Users/LoginUserHandler.cs`
- [ ] T028 [US2] Refactor login validation and authentication result handling in `src/backend/BlogPlatform.Application/Users/LoginUserHandler.cs` and `src/backend/BlogPlatform.Application/Users/LoginUserResult.cs`
- [ ] T029 [US2] Re-run focused TDD test suite for `tests/backend/BlogPlatform.Application.Tests/Users/LoginUserHandler*.cs`

**Checkpoint**: User Stories 1 and 2 both work independently inside Domain and Application without API, Infrastructure, database, JWT middleware, or frontend work.

---

## Final Phase: Polish & Cross-Cutting Concerns

**Purpose**: Confirm the slice remains small, reviewable, and aligned with the plan.

- [ ] T030 [P] Update feature notes if needed in `specs/004-user-auth/quickstart.md`
- [ ] T031 Run the backend core test sweep in `tests/backend/BlogPlatform.Domain.Tests/BlogPlatform.Domain.Tests.csproj` and `tests/backend/BlogPlatform.Application.Tests/BlogPlatform.Application.Tests.csproj`
- [ ] T032 Review final auth core scope in `specs/004-user-auth/plan.md`

---

## Dependencies & Execution Order

### Phase Dependencies

- Setup must complete first.
- Foundational Backend depends on Setup.
- User Story 1 depends on Foundational Backend.
- User Story 2 depends on User Story 1 because it reuses the shared user model and password abstraction.
- Final Polish depends on the selected user stories being complete.

### User Story Dependencies

- **User Story 1 (P1)**: Starts after Phase 2 and is the MVP scope for this feature.
- **User Story 2 (P2)**: Starts after User Story 1 and extends the same auth core with login behavior.

### Within User Story 1

- Tests `T007` through `T011` MUST be written and failing before implementation tasks `T012` through `T018`.
- `T012`, `T013`, `T014`, `T015`, and `T016` provide the compile-minimum Domain/Application types required for `T017`.
- `T017` implements the minimum behavior needed to pass tests.
- `T018` performs cleanup only after the first green implementation exists.
- `T019` verifies the story independently before login work begins.

### Within User Story 2

- Tests `T020` through `T023` MUST be written and failing before implementation tasks `T024` through `T028`.
- `T024`, `T025`, and `T026` provide the compile-minimum login types required for `T027`.
- `T027` implements the minimum behavior needed to pass tests.
- `T028` performs cleanup only after the first green implementation exists.
- `T029` verifies the story independently before final polish.

### Parallel Opportunities

- `T007` through `T011` can run in parallel because they target separate test files or separate assertions in story-specific test files.
- `T012` through `T016` can run in parallel after the failing registration tests exist because they target separate production files.
- `T020` through `T023` can run in parallel because they target separate login test files or separate assertions in story-specific test files.
- `T024` through `T026` can run in parallel after the failing login tests exist because they target separate production files.
- `T030`, `T031`, and `T032` can run in parallel after implementation is stable.

---

## Parallel Example: User Story 1

```bash
# Write failing registration tests in parallel:
Task: "Add Domain tests for valid user creation and invalid name or username, email, and password-hash state in tests/backend/BlogPlatform.Domain.Tests/Users/UserAccountTests.cs"
Task: "Add failing Application test for successful registration with hashed password persistence in tests/backend/BlogPlatform.Application.Tests/Users/RegisterUserHandlerSuccessTests.cs"
Task: "Add failing Application test for duplicate email rejection in tests/backend/BlogPlatform.Application.Tests/Users/RegisterUserHandlerDuplicateEmailTests.cs"
Task: "Add failing Application tests for invalid registration input in tests/backend/BlogPlatform.Application.Tests/Users/RegisterUserHandlerValidationTests.cs"

# Add compile-minimum registration types in parallel:
Task: "Create user account Domain model in src/backend/BlogPlatform.Domain/Users/UserAccount.cs"
Task: "Create user repository abstraction in src/backend/BlogPlatform.Application/Abstractions/IUserRepository.cs"
Task: "Create password security abstraction in src/backend/BlogPlatform.Application/Abstractions/IPasswordSecurityService.cs"
Task: "Create register-user command input model in src/backend/BlogPlatform.Application/Users/RegisterUserCommand.cs"
Task: "Create register-user result model in src/backend/BlogPlatform.Application/Users/RegisterUserResult.cs"
```

---

## Parallel Example: User Story 2

```bash
# Write failing login tests in parallel:
Task: "Add failing Application test for successful login with authentication result data in tests/backend/BlogPlatform.Application.Tests/Users/LoginUserHandlerSuccessTests.cs"
Task: "Add failing Application test for unknown-user login rejection in tests/backend/BlogPlatform.Application.Tests/Users/LoginUserHandlerInvalidCredentialsTests.cs"
Task: "Add failing Application assertion coverage for password verification and authentication payload generation abstractions in tests/backend/BlogPlatform.Application.Tests/Users/LoginUserHandlerSuccessTests.cs"

# Add compile-minimum login types in parallel:
Task: "Create authentication payload factory abstraction in src/backend/BlogPlatform.Application/Abstractions/IAuthenticationPayloadFactory.cs"
Task: "Create login-user command input model in src/backend/BlogPlatform.Application/Users/LoginUserCommand.cs"
Task: "Create login-user result model in src/backend/BlogPlatform.Application/Users/LoginUserResult.cs"
```

---

## Implementation Strategy

### MVP First

1. Complete Setup and Foundational Backend.
2. Write failing Domain/Application tests for registration.
3. Add only the minimum shared auth types and abstractions needed to compile.
4. Implement registration to pass tests.
5. Validate User Story 1 independently before adding login.

### Incremental Delivery

1. First increment: Domain/Application registration behavior only.
2. Second increment: Domain/Application login behavior only.
3. Later increment: API wiring for registration and login HTTP flows.
4. Later increment: Infrastructure persistence and concrete security implementations with raw SQL and Npgsql.
5. Final increment: frontend integration only if the feature is expanded.

## Notes

- All tasks follow the required checklist format with IDs, labels, and file paths.
- The suggested MVP scope is `US1` only.
- API, Infrastructure, database, JWT middleware, and frontend implementation are intentionally excluded from this task list.
