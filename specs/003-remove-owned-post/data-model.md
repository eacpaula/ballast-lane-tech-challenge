# Data Model: Authenticated User Removes Only Their Own Blog Post

## Overview

The first increment needs only the Domain and Application data required to load
an existing post, verify ownership, request removal, and return a business
result. Persistence-specific details and HTTP-specific contracts stay out of
this model.

## Entities

### BlogPost

- **Purpose**: Represents an existing persisted post whose ownership is checked
  before removal.
- **Core fields**:
  - `Id`: existing persisted identifier
  - `AuthorUserId`: identifies the owner of the post
  - `CategoryId`: existing category reference; not the focus of this slice
  - `Title`: existing title retained only as part of the loaded post shape
  - `Summary`: optional existing summary
  - `Content`: existing body retained only as part of the loaded post shape
- **Relationships**:
  - One post belongs to one user
- **Validation rules**:
  - `Id` must correspond to an existing post for removal to proceed
  - `AuthorUserId` must match the authenticated user id
- **State transitions**:
  - `Persisted post` -> `Removed post` (business outcome; persistence semantics
    are deferred)

## Application Boundary Models

### RemoveBlogPostCommand

- **Purpose**: Carries the use case input for removing an existing post.
- **Fields**:
  - `AuthenticatedUserId`
  - `PostId`

### RemoveBlogPostResult

- **Purpose**: Returns either the successful removal outcome or a business
  failure outcome without leaking persistence details.
- **Fields**:
  - `PostId`
  - `AuthorUserId`
  - `ErrorCode`
  - `ErrorMessage`

## Repository Abstractions

### IPostRepository

- **Purpose**: Supports loading the target post and removing it.
- **Minimum behavior needed**:
  - Read a post by id
  - Remove a post

## Invariants to Prove with Tests

- A missing authenticated user cannot remove a post.
- A missing target post cannot be removed.
- A user cannot remove a post owned by another user.
- A successful removal deletes the owned post through the repository
  abstraction.
- The successful result identifies the removed post and owner for later
  outer-layer mapping.
