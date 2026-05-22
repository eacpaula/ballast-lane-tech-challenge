# Frontend MVP Consumption Contract

## Runtime Contract

- Frontend runtime base URL is read from `VITE_API_BASE_URL`.
- The frontend must remain runnable locally and through Docker Compose.
- The frontend consumes explicit JSON DTOs and ProblemDetails-style failures
  from the backend API.

## Route Contract

| Route | Access | Primary UI Purpose | Backend Contract |
|---|---|---|---|
| `/` | Public | List public posts | `GET /api/posts` |
| `/posts/:postId` | Public | Show public post details | `GET /api/posts/{postId}` |
| `/login` | Public | Log in an existing user | `POST /api/auth/login` |
| `/register` | Public | Register a new user | `POST /api/auth/register` |
| `/my-posts` | Authenticated | Show the signed-in author workspace | **Prerequisite:** protected list-own-posts endpoint |
| `/my-posts/new` | Authenticated | Create a new post | `GET /api/categories/available`, `POST /api/posts` |
| `/my-posts/:postId/edit` | Authenticated | Edit an owned post | **Prerequisite:** protected get-own-post endpoint, `GET /api/categories/available`, `PUT /api/posts/{postId}` |
| `/admin/categories` | Administrator | Manage categories | **Prerequisite:** admin list-categories endpoint, `POST /api/categories`, `PUT /api/categories/{categoryId}`, `DELETE /api/categories/{categoryId}` |

## Existing Backend Contracts the Frontend Can Use Immediately

- `POST /api/auth/register`
- `POST /api/auth/login`
- `GET /api/posts`
- `GET /api/posts/{postId}`
- `POST /api/posts/{postId}/reactions`
- `POST /api/posts`
- `PUT /api/posts/{postId}`
- `DELETE /api/posts/{postId}`
- `GET /api/categories/available`
- `POST /api/categories`
- `PUT /api/categories/{categoryId}`
- `DELETE /api/categories/{categoryId}`

## Confirmed Backend Gaps Blocking Full MVP UI

The current backend does **not** expose the following read contracts needed by
the specified frontend pages:

- protected author post list endpoint
- protected author post detail endpoint for editing
- administrator category list endpoint that includes unavailable categories

These must be planned and implemented before the dependent frontend pages can be
completed without workarounds.

## Session Contract

- Successful login returns:
  - `userId`
  - `nameOrUsername`
  - `email`
  - `authenticationPayload`
- Registration returns user information but not an authentication payload.
- Frontend session restoration depends on the stored authentication payload and
  locally decoded JWT claims.

## Error Contract

- Validation, authorization, not-found, conflict, and unexpected API failures
  are consumed as ProblemDetails-style responses.
- Frontend pages must map these into readable form or page feedback without
  reimplementing backend business validation.
