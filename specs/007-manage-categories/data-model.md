# Data Model: Administrator Manages Post Categories

## Overview

The first increment needs only the Domain and Application data required to
validate category titles, enforce administrator-only access, check duplicate
titles, and make a category unavailable for future use. Persistence-specific
details and HTTP-specific contracts stay out of this model.

## Entities

### PostCategory

- **Purpose**: Represents a category used to organize blog posts.
- **Core fields**:
  - `Id`
  - `Title`
  - `IsAvailable`
- **Relationships**:
  - One category may be referenced by many posts, but post behavior is outside
    this slice.
- **Validation rules**:
  - `Title` must be present and valid.
  - A deactivated category is not available for future use.
- **State transitions**:
  - `New category` -> `Active category`
  - `Active category` -> `Updated category`
  - `Active category` -> `Inactive category`

## Authorization Boundary Concept

### CategoryManagementActor

- **Purpose**: Represents the authenticated actor context passed into
  Application for category management authorization checks.
- **Fields**:
  - `AuthenticatedUserId`
  - `IsAdministrator`
- **Validation rules**:
  - Category management requires a valid authenticated user id.
  - Category management requires `IsAdministrator` to be true.

## Application Boundary Models

### CreatePostCategoryCommand

- **Purpose**: Represents the input required to create a category.
- **Fields**:
  - `AuthenticatedUserId`
  - `IsAdministrator`
  - `Title`

### UpdatePostCategoryCommand

- **Purpose**: Represents the input required to update a category.
- **Fields**:
  - `AuthenticatedUserId`
  - `IsAdministrator`
  - `CategoryId`
  - `Title`

### DeactivatePostCategoryCommand

- **Purpose**: Represents the input required to deactivate a category.
- **Fields**:
  - `AuthenticatedUserId`
  - `IsAdministrator`
  - `CategoryId`

### CategoryManagementResult

- **Purpose**: Represents either a successful category-management outcome or a
  business failure outcome without leaking persistence details.
- **Fields**:
  - `IsSuccess`
  - `CategoryId`
  - `Title`
  - `IsAvailable`
  - `ErrorCode`
  - `ErrorMessage`

## Repository Abstractions

### ICategoryRepository

- **Purpose**: Supports category creation, loading, update, deactivation, and
  duplicate-title checks.
- **Minimum behavior needed**:
  - Check whether a title already exists
  - Create a category
  - Load a category by id
  - Update a category
  - Deactivate a category

## Invariants to Prove with Tests

- A valid administrator can create a category with a valid title.
- A valid administrator can update a category with a valid non-duplicate title.
- A valid administrator can deactivate a category and make it unavailable.
- Non-admin users are rejected for create, update, and deactivate operations.
- Invalid titles are rejected.
- Duplicate titles are rejected.
- Missing categories are rejected for update or deactivation.
- Category persistence occurs only through repository abstractions.
