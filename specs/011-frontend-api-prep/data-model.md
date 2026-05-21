# Data Model: Prepare API for Frontend Integration

## Overview

This feature does not add new core business entities. It adds one read-oriented
Application model, one API response model, and a small amount of runtime
configuration to support local frontend integration.

## Application Read Model

### AvailablePostCategoryListItem

- **Purpose**: Represent one category that can be selected in a post create or
  edit form.
- **Fields**:
  - `categoryId`
  - `title`
- **Source**: Derived from available `PostCategory` records only.
- **Rules**:
  - Must include only categories with `IsAvailable = true`.
  - Must not include deactivated or unavailable categories.
  - Must remain read-only and separate from administrator management commands.

## API Response Models

### AvailableCategoryResponse

- **Purpose**: Return category selection data to the frontend.
- **Fields**:
  - `id`
  - `title`
- **Maps from**: `AvailablePostCategoryListItem`
- **Rules**:
  - Returns only fields needed for category selection.
  - Does not expose administrator-only management metadata.

## Repository Contract Extension

### Category Availability Query

- **Purpose**: Allow Application to request the available categories that may
  be assigned to posts.
- **Repository impact**:
  - Extend `ICategoryRepository` with one read method for listing available
    categories.
- **Rules**:
  - Filtering belongs in Infrastructure query execution.
  - Business intent and endpoint behavior remain owned by Application and API.

## Runtime Configuration Models

### ApiContainerRuntimeSettings

- **Purpose**: Describe the environment configuration needed when the API runs
  inside Docker Compose.
- **Fields**:
  - database host
  - database port
  - database name
  - database user
  - database password
  - JWT issuer
  - JWT audience
  - JWT signing key
  - JWT expiration
  - API listen URL or port
- **Rules**:
  - Host-based local execution and container-based local execution must both be
    supported through configuration overrides rather than code forks.

### LocalCorsSettings

- **Purpose**: Describe the local frontend origins allowed to call the API in
  browser-based development.
- **Fields**:
  - one or more allowed frontend origins
- **Rules**:
  - Origins should be explicit and local-development focused.
  - Wildcard origin behavior is intentionally excluded from this slice.

## Existing Entities Reused

### PostCategory

- **Purpose in this feature**: The source entity for category selection data.
- **Relevant fields**:
  - `Id`
  - `Title`
  - `IsAvailable`
- **State transitions reused**:
  - Deactivated categories remain excluded from the new read endpoint.

## Mapping Boundaries

- Infrastructure maps raw SQL result rows into Domain `PostCategory` entities or
  direct repository return values consistent with the abstraction.
- Application transforms repository results into a read-only list model for the
  API.
- API maps Application read models into explicit response DTOs.
- Docker and CORS configuration remain in API/runtime configuration, not in
  Application or Domain.
