# Quickstart: Frontend MVP

## Goal

Validate the React frontend MVP against the existing full-stack local runtime
while keeping the flow small enough for an interview demo.

## Preconditions

- Docker and Docker Compose are available locally
- The PostgreSQL, API, and frontend runtime from the previous infrastructure
  slices can start successfully
- The prerequisite backend read endpoints for owned-post and admin-category
  screens are implemented

## Validation Steps

1. Start the backend stack from the repository root:

```bash
docker compose up -d postgres api frontend
docker compose ps
```

2. Install frontend dependencies if needed and start local frontend development
   when working outside the Compose-based frontend container:

```bash
cd src/frontend/blog-web
npm install
npm run dev
```

3. Validate the public flow:
   - open `http://localhost:5173`
   - confirm the landing page loads
   - confirm public posts load
   - open a public post detail page
   - submit a like or dislike reaction

4. Validate the auth flow:
   - register a new user or log in with a seeded user
   - confirm protected navigation appears
   - confirm logout returns the shell to the anonymous state

5. Validate the author flow:
   - log in as `user@blogplatform.local` / `User123!`
   - open `My Posts`
   - create a post
   - edit an owned post
   - remove an owned post

6. Validate the administrator flow:
   - log in as `admin@blogplatform.local` / `Admin123!`
   - open category management
   - create a category
   - update a category
   - deactivate a category

7. Run the lightweight frontend validation commands from `src/frontend/blog-web`:

```bash
npm run lint
npm run build
```

8. Optional API spot checks for the demo runtime:

```bash
curl http://localhost:5034/swagger/v1/swagger.json
curl http://localhost:5034/api/posts
curl http://localhost:5034/api/categories/available
```

## Expected Outcome

- Public, authenticated, and administrator flows are reachable through the
  frontend without violating backend authorization rules.
- The UI remains visually aligned with `DESIGN.md`.
- Loading, empty, success, and error states are visible and understandable.
- The MVP remains small, explainable, and runnable through the documented local
  stack.
