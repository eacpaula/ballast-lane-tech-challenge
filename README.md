# Ballast Lane Technical Challenge - Blog Platform

Full-stack technical interview project for Ballast Lane Applications. This repository is a blog platform monorepo with an ASP.NET Core Web API backend, a React frontend, PostgreSQL, Docker Compose, and a spec-driven delivery workflow.

The goal of this README is to help a reviewer understand the solution quickly, run it with minimal setup, and verify the main implementation decisions.

## Quick Review Path

For the fastest review:

```bash
cp .env.example .env
docker compose up -d postgres api frontend
docker compose ps
```

Then open:

- Frontend: `http://localhost:5173`
- Swagger UI: `http://localhost:5034/swagger`
- OpenAPI JSON: `http://localhost:5034/swagger/v1/swagger.json`

Demo accounts:

- Administrator: `admin@blogplatform.local` / `Admin123!`
- Regular user: `user@blogplatform.local` / `User123!`

## Project Overview

This project implements a small blog platform intended for technical review rather than production completeness.

- Anonymous users can browse public posts, search posts, open post details, and submit like/dislike reactions.
- Authenticated users can register, log in, and manage their own posts.
- Administrators can manage post categories.

## Challenge Alignment

This implementation is intentionally aligned to the challenge constraints:

- ASP.NET Core Web API backend with thin controllers
- Clean Architecture separation between Domain, Application, Infrastructure, and API
- Raw SQL data access with `Npgsql`
- No Entity Framework
- No Dapper
- No Mediator or MediatR
- Business logic kept outside controllers
- Application layer independent from Infrastructure/data access details
- Automated backend test projects across Domain, Application, Infrastructure, and API layers
- React frontend for end-to-end demonstration
- Seeded demo data and documented GenAI usage

## Architecture Summary

### Backend

- `BlogPlatform.Domain`: core entities and invariants
- `BlogPlatform.Application`: use cases, validation, authorization decisions, and abstractions
- `BlogPlatform.Infrastructure`: PostgreSQL repositories, authentication helpers, and runtime wiring
- `BlogPlatform.Api`: HTTP controllers, DTOs, auth setup, Swagger, and composition root

### Frontend

- React + Vite + TypeScript SPA
- TailwindCSS-based styling guided by [DESIGN.md](./DESIGN.md)
- Consumes the API for authentication, posts, reactions, and category management

### Data Access

- PostgreSQL schema and seed data are managed with explicit SQL scripts under `database/`
- No ORM or micro-ORM is used

## Technology Stack

### Backend

- .NET SDK `10.0.107`
- ASP.NET Core Web API
- JWT Bearer authentication
- Swagger / OpenAPI
- xUnit

### Frontend

- React
- Vite
- TypeScript
- TailwindCSS

### Data and Runtime

- PostgreSQL 17
- Docker Compose

## Repository Structure

```txt
.
├── database/                  # SQL bootstrap and seed scripts
├── docs/                      # challenge docs, architecture notes, GenAI notes
├── specs/                     # spec-driven feature artifacts
├── src/
│   ├── backend/
│   │   ├── BlogPlatform.Domain/
│   │   ├── BlogPlatform.Application/
│   │   ├── BlogPlatform.Infrastructure/
│   │   └── BlogPlatform.Api/
│   └── frontend/
│       └── blog-web/
├── tests/
│   ├── backend/
│   └── frontend/
├── docker-compose.yml
├── DESIGN.md
└── README.md
```

## Requirements

### Recommended

- Docker and Docker Compose

### Optional for non-Docker app execution

- .NET SDK `10.0.107`
- Node.js and npm

### Default local ports

- PostgreSQL: `5432`
- API: `5034`
- Frontend: `5173`

You can override these values through [`.env.example`](./.env.example).

## Running the Stack with Docker Compose

### 1. Configure environment variables

The defaults are usually sufficient:

```bash
cp .env.example .env
```

Only change `.env` if you need different ports or credentials.

### 2. Start the stack

```bash
docker compose up -d postgres api frontend
docker compose ps
```

Current Compose services:

- `postgres`: PostgreSQL database initialized from `database/scripts/` and `database/seeds/`
- `api`: ASP.NET Core Web API
- `frontend`: React development server

### 3. Open the app

- Frontend: `http://localhost:5173`
- Swagger UI: `http://localhost:5034/swagger`
- OpenAPI JSON: `http://localhost:5034/swagger/v1/swagger.json`

## Service Notes

### Database

- PostgreSQL is initialized from ordered SQL scripts on first startup.
- Seed data includes users, roles, categories, posts, tags, post-tag links, and reactions.
- The database data directory is mounted to `database/data/pgdata`.

### API

- The API exposes public, authenticated, and administrator-only endpoints.
- Swagger is enabled for interactive review.
- A sample HTTP file is available at `src/backend/BlogPlatform.Api/BlogPlatform.Api.http`.

### Frontend

- The frontend is a Vite-based SPA.
- Main routes include:
  - `/`
  - `/posts/:postId`
  - `/register`
  - `/login`
  - `/my-posts`
  - `/my-posts/new`
  - `/my-posts/:postId/edit`
  - `/admin/categories`

### Redis

- Redis is not part of the current `docker-compose.yml` runtime.
- There is feature/spec documentation for planned Redis-backed post-list caching under [`specs/016-search-pagination-cache/`](./specs/016-search-pagination-cache/), but that wiring is not currently present in this branch.

## Optional Local Execution Without Full Compose

If you prefer to run the app processes locally but still use Docker for PostgreSQL:

### Start PostgreSQL

```bash
docker compose up -d postgres
docker compose ps
```

### Run the API

```bash
dotnet run --project src/backend/BlogPlatform.Api/BlogPlatform.Api.csproj
```

### Run the frontend

```bash
cd src/frontend/blog-web
npm install
npm run dev
```

## Testing

### Backend tests

Run the backend unit and integration test projects from the repository root:

```bash
dotnet test tests/backend/BlogPlatform.Domain.Tests/BlogPlatform.Domain.Tests.csproj
dotnet test tests/backend/BlogPlatform.Application.Tests/BlogPlatform.Application.Tests.csproj
dotnet test tests/backend/BlogPlatform.Infrastructure.Tests/BlogPlatform.Infrastructure.Tests.csproj
dotnet test tests/backend/BlogPlatform.Api.Tests/BlogPlatform.Api.Tests.csproj
```

Notes:

- `BlogPlatform.Infrastructure.Tests` requires PostgreSQL to be running and initialized.
- `BlogPlatform.Api.Tests` also requires PostgreSQL to be running and initialized.
- Starting `postgres` with Docker Compose is the expected local test prerequisite.

### Frontend validation

```bash
cd src/frontend/blog-web
npm install
npm run lint
npm run build
```

There are currently no automated frontend test suites in `tests/frontend`.

## Swagger and API Review

Use Swagger for the quickest API review:

- Swagger UI: `http://localhost:5034/swagger`
- OpenAPI JSON: `http://localhost:5034/swagger/v1/swagger.json`

Suggested API review flow:

1. `POST /api/auth/login`
2. `GET /api/posts`
3. `GET /api/posts/{postId}`
4. `POST /api/posts/{postId}/reactions`
5. Use the Swagger `Authorize` button with the returned bearer token
6. Exercise protected post endpoints and admin category endpoints

## Demo Credentials

These accounts are seeded for local review only:

- Administrator
  - Email: `admin@blogplatform.local`
  - Password: `Admin123!`
- Regular user
  - Email: `user@blogplatform.local`
  - Password: `User123!`

## Suggested Demo Flow

### Frontend

1. Open `http://localhost:5173`
2. Browse public posts and open a post detail page
3. Submit a reaction anonymously
4. Log in as `user@blogplatform.local`
5. Open `My Posts` and create, edit, or delete an owned post
6. Log in as `admin@blogplatform.local`
7. Open `Categories` and manage categories

### Swagger

1. Log in through `POST /api/auth/login`
2. Review public post endpoints without authentication
3. Add the JWT token through Swagger authorization
4. Review post create/update/delete behavior
5. Review admin-only category endpoints

## Main Features Implemented

- User registration and login with JWT authentication
- Public post listing and post detail endpoints
- Backend-powered post search through `GET /api/posts?q=...`
- Like/dislike reactions for public posts
- Authenticated “My Posts” workflow
- Create, edit, and delete owned posts
- Support for post tags in post create/update flows
- Administrator-only category management
- Category descriptions
- Paginated category listing for admin and public available-category reads
- Swagger/OpenAPI documentation
- SQL-based database bootstrap and seed data

## Documentation

Use the root README for setup, then these files for deeper detail:

- [Challenge brief](./docs/DotNET%20-%20Technical%20Test.md)
- [Architectural decisions](./docs/DotNET%20-%20Technical%20Test%20-%20Architectural%20Decisions.md)
- [Database implementation notes](./docs/Database-Implementation-Strategy.md)
- [ERD notes](./docs/ERD-Diagram.md)
- [UML notes](./docs/UML-Diagram.md)
- [GenAI usage documentation](./docs/genai-usage.md)
- [Frontend-specific notes](./src/frontend/blog-web/README.md)

## Spec-Driven and GenAI Workflow

This repository uses a spec-driven workflow to track feature work.

- Feature artifacts live under `specs/<feature-id>-<feature-name>/`
- Typical files include `spec.md`, `plan.md`, `tasks.md`, and `quickstart.md`
- The repository also includes Spec Kit support files used to guide delivery and keep decisions consistent with the challenge constraints

GenAI usage is documented separately in [docs/genai-usage.md](./docs/genai-usage.md), including the project constitution prompt and the role GenAI played in planning.

## Known Limitations and Trade-offs

- The project is intentionally MVP-scoped and does not aim to be a full CMS.
- Public post listing and search are implemented, but they are not currently paginated.
- Redis caching is documented in feature specs but is not wired into the current runtime or Compose stack.
- Frontend tag management beyond post create/edit usage is out of scope.
- `tests/frontend` does not currently contain automated frontend tests.
- Category management is implemented, but broader admin tooling is intentionally limited.

## Troubleshooting

### Port already in use

Update the relevant values in `.env`, then restart the stack.

### PostgreSQL tests fail because schema is missing

Start the database first:

```bash
docker compose up -d postgres
docker compose ps
```

### Seed data does not reset after `docker compose down -v`

The PostgreSQL service uses a bind mount at `database/data/pgdata`, so `down -v` alone does not remove existing database files.

If you need a full reset:

1. Stop the stack with `docker compose down`
2. Remove `database/data/pgdata`
3. Start PostgreSQL again with `docker compose up -d postgres`

Depending on your Docker setup, deleting that directory may require elevated permissions because it is created by the container.
