# Quickstart: Frontend Docker Compose Service

## Goal

Verify that the database, API, and frontend can run together through Docker
Compose for local development and interview-demo usage.

## Preconditions

- Docker and Docker Compose are available locally
- Root environment variables use either defaults or documented overrides

## Steps

1. Review `.env.example` and create `.env` only if overrides are needed.
2. Start the full local stack from the repository root.
3. Confirm the `postgres`, `api`, and `frontend` services are all running.
4. Open the documented frontend URL in a browser.
5. Open the documented Swagger URL in a browser.
6. Confirm the frontend is using the expected local API base URL configuration.
7. Stop the stack when validation is complete.

## Expected Outcome

- Database, API, and frontend run together through one Compose workflow
- The frontend is reachable locally in a browser
- The API remains reachable locally
- The stack is suitable for a simple technical interview demo
