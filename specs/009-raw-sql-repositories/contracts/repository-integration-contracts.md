# Contract: Repository Integration Contracts

## Purpose

This contract defines the Infrastructure persistence behaviors that must be
proven by PostgreSQL-backed integration tests before repository implementation
is considered complete.

## User Repository Contract

### Methods in Scope

- `CreateAsync`
- `GetByEmailAsync`

### Required Test Outcomes

- Creating a user persists the supplied user data and returns a stored result
  with a database identity.
- Looking up by email returns the matching stored user when present.
- Looking up by email returns no result when the email is absent.

## Post Repository Contract

### Methods in Scope

- `CreateAsync`
- `DeleteAsync`
- `GetByIdAsync`
- `GetPublicReadByIdAsync`
- `ListPublicReadAsync`
- `UpdateAsync`

### Required Test Outcomes

- Creating a post persists the stored post record and returns the stored result.
- Loading by id returns the stored post for mutation workflows.
- Updating a post persists changed values.
- Deleting a post removes or makes unavailable the stored row according to the
  existing abstraction contract.
- Public-read list and detail queries return only public and available posts.

## Category Repository Contract

### Methods in Scope

- `ExistsAndAvailableAsync`
- `TitleExistsAsync`
- `CreateAsync`
- `GetByIdAsync`
- `UpdateAsync`
- `DeactivateAsync`

### Required Test Outcomes

- Availability checks reflect the stored category state.
- Duplicate-title checks reflect stored title usage and ignore the current
  category when appropriate.
- Category create, load, update, and deactivate operations persist correctly.

## Post Reaction Repository Contract

### Methods in Scope

- `CreateAsync`

### Required Test Outcomes

- Creating a reaction persists the stored reaction with either a user actor or
  visitor actor.
- Stored reaction data can be mapped back into the expected Domain model
  surface required by the existing abstraction.

## Validation Boundary Contract

The following behaviors are intentionally outside repository responsibility and
must remain outside Infrastructure:

- authentication decisions
- authorization decisions
- ownership checks
- duplicate-email business handling
- duplicate-category-title business handling
- post availability decisions beyond the query filters explicitly required by
  the repository contract
- reaction-type and actor validation
