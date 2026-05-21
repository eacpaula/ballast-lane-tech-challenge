# Data Model: Implement Raw SQL Repositories for Existing Application Use Cases

## Overview

This feature does not introduce new product entities. Instead, it translates
the existing Domain/Application models into Infrastructure persistence
responsibilities and integration-test expectations against PostgreSQL.

## Repository Boundaries

### UserRepositoryPersistence

- **Purpose**: Supports registration and login persistence through
  `IUserRepository`.
- **Repository methods in scope**:
  - `CreateAsync`
  - `GetByEmailAsync`
- **Persistence responsibilities**:
  - insert user records
  - load users by normalized email
- **Validation boundary**:
  - email validity, duplicate-email business decisions, and password hashing
    stay outside Infrastructure

### PostRepositoryPersistence

- **Purpose**: Supports post mutation and public-read persistence through
  `IPostRepository`.
- **Repository methods in scope**:
  - `CreateAsync`
  - `DeleteAsync`
  - `GetByIdAsync`
  - `GetPublicReadByIdAsync`
  - `ListPublicReadAsync`
  - `UpdateAsync`
- **Persistence responsibilities**:
  - insert, update, and delete post rows
  - load posts by id for ownership workflows
  - filter public-read queries by public/available state
- **Validation boundary**:
  - ownership, authentication, and content validation stay in Application

### CategoryRepositoryPersistence

- **Purpose**: Supports category-management persistence through
  `ICategoryRepository`.
- **Repository methods in scope**:
  - `ExistsAndAvailableAsync`
  - `TitleExistsAsync`
  - `CreateAsync`
  - `GetByIdAsync`
  - `UpdateAsync`
  - `DeactivateAsync`
- **Persistence responsibilities**:
  - check category existence and availability
  - check duplicate titles by current stored state
  - create, load, update, and deactivate category rows
- **Validation boundary**:
  - admin authorization and duplicate-title business handling stay in
    Application

### PostReactionRepositoryPersistence

- **Purpose**: Supports reaction persistence through
  `IPostReactionRepository`.
- **Repository methods in scope**:
  - `CreateAsync`
- **Persistence responsibilities**:
  - insert reaction rows for user or visitor actors
- **Validation boundary**:
  - reaction-type validation, actor validation, and post availability checks
    stay in Application

## Infrastructure Support Artifacts

### DatabaseConnectionSettings

- **Purpose**: Represents the Infrastructure-owned PostgreSQL connection
  configuration used by repositories and integration tests.
- **Core fields**:
  - `Host`
  - `Port`
  - `Database`
  - `Username`
  - `Password`

### DatabaseConnectionFactory

- **Purpose**: Produces PostgreSQL connections for repository operations while
  keeping `Npgsql` ownership in Infrastructure.

### RepositoryIntegrationTestEnvironment

- **Purpose**: Represents the test-only database setup and reset workflow used
  by Infrastructure integration tests.
- **Core responsibilities**:
  - open a PostgreSQL connection
  - reset or clean relevant tables deterministically
  - prepare known test state for repository assertions

## Persistence Mapping Scope

### UserAccount Mapping

- **Mapped fields**:
  - `Id`
  - `NameOrUsername`
  - `Email`
  - `PasswordHash`

### BlogPost Mapping

- **Mapped fields**:
  - `Id`
  - `AuthorUserId`
  - `CategoryId`
  - `Title`
  - `Summary` from persisted description
  - `Content`
  - `IsPublic`
  - `IsAvailable`

### PostCategory Mapping

- **Mapped fields**:
  - `Id`
  - `Title`
  - `IsAvailable`

### PostReaction Mapping

- **Mapped fields**:
  - `Id`
  - `PostId`
  - reaction type value
  - actor identity values derived from `user_id` or `visitor_identifier`

## Integration-Test Scenarios to Prove

- User creation persists a hashed password and returns a rehydrated user model.
- Email lookup returns the correct user or no result.
- Post create, update, and delete operations persist correctly.
- Public-post queries exclude non-public or unavailable rows.
- Category duplicate-title and availability checks reflect database state.
- Category create, update, load, and deactivate operations persist correctly.
- Reaction persistence writes the expected actor and reaction type fields.
