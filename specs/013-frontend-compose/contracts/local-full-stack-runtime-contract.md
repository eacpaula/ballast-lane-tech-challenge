# Local Full-Stack Runtime Contract

## Purpose

Define the observable runtime behavior for the local Compose stack once the
frontend service is added.

## Contract

### 1. Compose startup contract

- A developer can start `postgres`, `api`, and `frontend` together through one
  Docker Compose workflow.
- Compose configuration remains readable and local-development oriented.

### 2. Frontend runtime contract

- The frontend is reachable from a browser on a documented local URL.
- The frontend container runs the existing app in development mode.
- The frontend service does not require production hosting infrastructure.

### 3. Frontend environment contract

- The frontend receives a documented API base URL configuration value.
- That value is suitable for requests originating in the browser, not only
  within the Compose network.

### 4. API compatibility contract

- The API remains reachable on its documented local URL.
- Local CORS configuration allows requests from the frontend URL used in the
  Compose flow.
- No backend business rules or endpoint authorization behavior changes as part
  of this slice.

### 5. Documentation contract

- Repository documentation explains how to start the full stack.
- Repository documentation identifies the frontend URL and API URL used during
  local demo execution.
