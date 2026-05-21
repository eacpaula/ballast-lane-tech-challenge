# Manage Post Categories Use Cases Contract

## Purpose

This contract defines the Application-layer boundaries for the first
implementation increment. It stays independent of HTTP so administrator-only
category-management rules can be implemented and tested before API wiring.

## Create Category Contract

### Input

- `AuthenticatedUserId`: required authenticated actor id
- `IsAdministrator`: required administrator indicator
- `Title`: required category title

### Success Contract

On success, the use case returns a category-management result containing:

- `CategoryId`
- `Title`
- `IsAvailable`

### Failure Contract

The use case can fail with one of these business outcomes:

- `AuthorizationRequired`: the actor is not authorized to manage categories
- `ValidationError`: the title is missing or invalid
- `DuplicateCategoryTitle`: the title is already used by another category

## Update Category Contract

### Input

- `AuthenticatedUserId`
- `IsAdministrator`
- `CategoryId`
- `Title`

### Failure Contract

The use case can fail with:

- `AuthorizationRequired`
- `ValidationError`
- `DuplicateCategoryTitle`
- `CategoryNotFound`

## Deactivate Category Contract

### Input

- `AuthenticatedUserId`
- `IsAdministrator`
- `CategoryId`

### Success Contract

On success, the category-management result reflects that the category is no
longer available for future use.

### Failure Contract

The use case can fail with:

- `AuthorizationRequired`
- `CategoryNotFound`

## Deferred Outer-Boundary Behavior

The following behavior is required by the overall feature but is intentionally
deferred to a later API or persistence increment:

- HTTP endpoint handling for admin-only category management
- Translating category-management failures into HTTP responses
- Executing category reads and writes with raw SQL and Npgsql
- JWT claim mapping, policy configuration, or admin UI workflows
