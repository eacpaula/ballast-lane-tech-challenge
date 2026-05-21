# Data Model: Authenticated User Edits Only Their Own Blog Post

## Overview

The first increment needs only the Domain and Application data required to load
an existing post, verify ownership, apply valid updates, and return a business
result. Persistence-specific details and HTTP-specific contracts stay out of
this model.

## Entities

### BlogPost

- **Purpose**: Represents an existing post that may be edited by its owner.
- **Core fields**:
  - `Id`: existing persisted identifier
  - `AuthorUserId`: identifies the owner of the post
  - `CategoryId`: existing category reference; not the focus of this slice
  - `Title`: required, non-blank after update
  - `Summary`: optional descriptive text
  - `Content`: required, non-blank after update
- **Relationships**:
  - One post belongs to one user
- **Validation rules**:
  - `Id` must exist for edit flows
  - `Title` must contain non-whitespace content
  - `Content` must contain non-whitespace content
- **State transitions**:
  - `Persisted post` -> `Validated updated post`

## Application Boundary Models

### EditBlogPostCommand

- **Purpose**: Carries the use case input for editing an existing post.
- **Fields**:
  - `AuthenticatedUserId`
  - `PostId`
  - `Title`
  - `Summary`
  - `Content`

### EditBlogPostResult

- **Purpose**: Returns either the successful edited-post outcome or a business
  failure outcome without leaking persistence details.
- **Fields**:
  - `PostId`
  - `AuthorUserId`
  - `Title`
  - `Summary`
  - `Content`
  - `ErrorCode`
  - `ErrorMessage`

## Repository Abstractions

### IPostRepository

- **Purpose**: Supports loading a target post and saving a valid edited post.
- **Minimum behavior needed**:
  - Read a post by id
  - Persist an updated post

## Invariants to Prove with Tests

- A missing authenticated user cannot edit a post.
- A missing target post cannot be edited.
- A user cannot edit a post owned by another user.
- An owned post cannot be updated with a blank title.
- An owned post cannot be updated with blank content.
- A successful edit preserves ownership and returns the updated values.
