# Data Model: Configure PostgreSQL Database Environment and Schema Initialization

## Overview

This feature prepares the executable database slice needed by the current blog
platform use cases. The focus is not repository behavior yet, but the
structure, seed content, and operational bootstrap rules that later
Infrastructure code will rely on.

## Operational Artifacts

### DatabaseEnvironment

- **Purpose**: Represents the local PostgreSQL runtime configuration used for
  development and interview demos.
- **Core fields**:
  - `DatabaseName`
  - `Username`
  - `Password`
  - `HostPort`
  - `ContainerPort`
- **Validation rules**:
  - Values must be documented and repeatable for local startup.
  - Demo configuration must be safe for local use and clearly non-production.

### SchemaInitializationScript

- **Purpose**: Represents an ordered SQL artifact that creates part of the
  initial schema.
- **Core fields**:
  - `Order`
  - `FileName`
  - `Purpose`
  - `ObjectsCreated`
- **Validation rules**:
  - Scripts must be executable in order from a clean database state.
  - Scripts must align with the current-use-case subset of the schema diagram.

### SeedDataScript

- **Purpose**: Represents an ordered SQL artifact that inserts demo data for
  current feature flows.
- **Core fields**:
  - `Order`
  - `FileName`
  - `Purpose`
  - `EntitiesSeeded`
- **Validation rules**:
  - Seed scripts must be repeatable against the initialized schema.
  - Seed content must support public, authenticated, and admin demo paths.

## Initial Schema Slice

### Users

- **Purpose**: Stores registered user accounts for login, ownership, and admin
  assignment.
- **Key fields from current scope**:
  - `id`
  - `fullname`
  - `email`
  - `username`
  - `password`
  - `available`
- **Relationships**:
  - linked to `posts`
  - linked to `user_roles`
  - optionally linked to `post_reactions`

### Roles

- **Purpose**: Distinguishes administrator and regular-user role assignments
  without implementing full RBAC yet.
- **Key fields from current scope**:
  - `id`
  - `title`
  - `description`
- **Relationships**:
  - linked to `user_roles`

### UserRoles

- **Purpose**: Connects users to roles so an administrator account can be
  seeded and later read by Infrastructure code.

### PostCategories

- **Purpose**: Supports current category-management behavior and post
  classification.
- **Key fields from current scope**:
  - `id`
  - `title`
  - `description`
  - `available`

### Posts

- **Purpose**: Supports post ownership, public visibility, availability, and
  category assignment.
- **Key fields from current scope**:
  - `id`
  - `user_id`
  - `post_category_id`
  - `title`
  - `description`
  - `content`
  - `available`
  - `public_post`

### Tags

- **Purpose**: Supports authenticated-user tag creation and later post-tag
  association.

### PostTags

- **Purpose**: Connects posts and tags for the current tagging use case.

### PostReactions

- **Purpose**: Supports like/dislike behavior for public posts by user or
  visitor identity.
- **Key fields from current scope**:
  - `id`
  - `post_id`
  - `user_id`
  - `visitor_identifier`
  - `reaction_type`

## Demo Dataset Model

### DemoPrincipal

- **Purpose**: Represents a seeded local/demo account used for walkthroughs.
- **Core fields**:
  - `DisplayName`
  - `Username`
  - `Email`
  - `Password`
  - `Role`
- **Validation rules**:
  - At least one administrator and one regular user must exist.
  - Credentials must be documented as local/demo-only.

### DemoContentSet

- **Purpose**: Represents the minimum demo posts, categories, tags, and
  reactions needed to exercise current flows.
- **Core fields**:
  - `Categories`
  - `Posts`
  - `Tags`
  - `Reactions`
- **Validation rules**:
  - The dataset must support public reading and admin-category scenarios.
  - The dataset should remain small enough for manual inspection.

## Deferred Schema Groups

- `permissions`
- `modules`
- `settings`
- `role_permissions`

These groups are intentionally deferred because the current implemented use
cases do not require a full RBAC or configuration surface yet.
