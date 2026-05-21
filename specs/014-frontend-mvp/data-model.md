# Data Model: Frontend MVP

## SessionState

- **Purpose**: Represents the browser-side authenticated session.
- **Fields**:
  - `token`
  - `userId`
  - `nameOrUsername`
  - `email`
  - `roles`
  - `isAuthenticated`
- **Rules**:
  - Exists only after successful login.
  - Is cleared on logout or unrecoverable unauthorized responses.
  - Role data is used for UI guidance only, not backend authorization.
- **State transitions**:
  - `anonymous -> authenticated`
  - `authenticated -> restored`
  - `authenticated/restored -> expired`
  - `authenticated/restored -> logged_out`

## AnonymousVisitor

- **Purpose**: Stable anonymous actor identity for public reactions.
- **Fields**:
  - `visitorIdentifier`
- **Rules**:
  - Created once per browser context and stored locally.
  - Used only when the user is not authenticated.

## ApiProblem

- **Purpose**: Normalized frontend representation of backend ProblemDetails
  failures.
- **Fields**:
  - `status`
  - `title`
  - `detail`
  - `errors` (optional field-level collection)
- **Rules**:
  - Can drive page-level or form-level feedback.
  - Must not be treated as domain state.

## PublicPostSummary

- **Purpose**: List item for the public landing page.
- **Fields**:
  - `id`
  - `title`
  - `summary`
- **Relationships**:
  - Links to `PublicPostDetail`.

## PublicPostDetail

- **Purpose**: Public post detail view model.
- **Fields**:
  - `id`
  - `title`
  - `summary`
  - `content`
- **Relationships**:
  - Accepts reactions through `ReactionSubmission`.

## ReactionSubmission

- **Purpose**: Browser-side action model for like/dislike requests.
- **Fields**:
  - `postId`
  - `reactionType`
  - `userId` (optional)
  - `visitorIdentifier` (optional)
- **Rules**:
  - Uses `userId` when authenticated.
  - Uses `visitorIdentifier` when anonymous.
  - Accepts only supported reaction values.

## PostEditorDraft

- **Purpose**: Shared form model for create and edit post flows.
- **Fields**:
  - `categoryId`
  - `title`
  - `summary`
  - `content`
- **Rules**:
  - Requires available categories for selection.
  - Uses the same field model for create and edit.

## OwnedPostSummary

- **Purpose**: Authenticated author view model for the My Posts workspace.
- **Fields**:
  - `id`
  - `title`
  - `summary`
  - `isPublic`
  - `isAvailable`
  - `categoryId`
- **Rules**:
  - Depends on a backend prerequisite endpoint not currently implemented.
  - Must include enough state to drive edit and remove actions.

## AvailableCategory

- **Purpose**: Category option for post create and edit forms.
- **Fields**:
  - `id`
  - `title`
- **Rules**:
  - Comes from the existing public available-categories endpoint.
  - Excludes unavailable categories.

## ManagedCategory

- **Purpose**: Administrator category management item.
- **Fields**:
  - `id`
  - `title`
  - `isAvailable`
- **Rules**:
  - Depends on a backend prerequisite admin list endpoint not currently
    implemented.
  - Supports create, update, and deactivate flows.
- **State transitions**:
  - `available -> unavailable`

## RequestState

- **Purpose**: Shared UI-state model for async pages and mutations.
- **Fields**:
  - `status` (`idle`, `loading`, `success`, `empty`, `error`, `submitting`)
  - `message` (optional)
- **Rules**:
  - Applied consistently across pages, lists, and forms.
