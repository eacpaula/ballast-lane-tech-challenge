# Ballast Lane Technical Challenge - Blog Platform

## Overview

This repository contains a full-stack technical interview project for the Ballast Lane .NET Engineer challenge.

The application is a simple blog platform designed to demonstrate a clean and testable full-stack implementation using .NET, ASP.NET Core Web API, custom data access, authentication, authorization, CRUD operations, React, and responsible GenAI usage.

The goal is not to build a full CMS, but to provide a small, realistic application that can be presented and reviewed during a technical interview.

## Challenge Alignment

This project is based on the provided technical exercise, which asks for:

- A .NET C# web application with API and data layer
- ASP.NET MVC / Web API
- Clean Architecture principles
- TDD or strong test coverage
- CRUD operations through API endpoints
- User creation and login
- Stored user data
- Authorized and non-authorized endpoints
- Custom data access without Entity Framework, Dapper, or Mediator
- Business logic separated from API and data access layers
- Unit tests for data access, business logic, and API endpoints
- Frontend integration using a framework such as React
- Seeded data and demo credentials
- GenAI usage documentation

## Product Scope

The selected product is a simple blog platform.

Anonymous visitors can read public posts and react with likes or dislikes. Authenticated users can create posts, edit or remove only their own posts, and create tags. Administrators can manage post categories.

## Technology Stack

### Backend

- .NET 10
- ASP.NET Core Web API
- Clean Architecture
- Raw SQL data access
- PostgreSQL or SQL Server
- JWT authentication
- Swagger/OpenAPI
- xUnit

### Frontend

- React
- TypeScript
- Vite
- TailwindCSS

### Tooling

- Docker Compose
- Commitlint
- Husky
- Spec Kit
- GenAI-assisted research and planning

## Repository Structure

```txt
.
├── .agents/
├── .husky/
├── .specify/
├── database/
│   ├── migrations/
│   ├── scripts/
│   └── seeds/
├── docs/
├── src/
│   ├── backend/
│   └── frontend/
├── tests/
│   ├── backend/
│   └── frontend/
├── tools/
├── AGENTS.md
├── commitlint.config.cjs
├── package.json
└── README.md
```

## Local Database Setup

The repository includes a Docker Compose-based PostgreSQL environment for local
development and interview demos.

### Quick Start

1. Copy `.env.example` to `.env` if you want to override the defaults.
   Set `BLOG_PLATFORM_DB_PORT` if local port `5432` is already in use.
2. Start PostgreSQL:

```bash
docker compose up -d postgres
```

3. Check container health:

```bash
docker compose ps
```

The container initializes the current-use-case schema and demo seed data on the
first startup.

### Demo Credentials

- Administrator: `admin@blogplatform.local` / `Admin123!`
- Regular user: `user@blogplatform.local` / `User123!`

These credentials are for local/demo usage only.

### Reset the Database

```bash
docker compose down -v
docker compose up -d postgres
```

## Local API Setup

Run the backend API from the repository root after PostgreSQL is healthy:

```bash
dotnet run --project src/backend/BlogPlatform.Api/BlogPlatform.Api.csproj
```

Default local API URLs:

- Swagger UI: `http://localhost:5034/swagger`
- OpenAPI JSON: `http://localhost:5034/swagger/v1/swagger.json`

### API Demo Flow

1. Start PostgreSQL with `docker compose up -d postgres`.
2. Start the API with `dotnet run --project src/backend/BlogPlatform.Api/BlogPlatform.Api.csproj`.
3. Open Swagger and call `POST /api/auth/login` with:
   - `admin@blogplatform.local` / `Admin123!`
   - `user@blogplatform.local` / `User123!`
4. Use the returned bearer token in Swagger for protected endpoints:
   - `POST /api/posts`
   - `PUT /api/posts/{postId}`
   - `DELETE /api/posts/{postId}`
   - `POST /api/categories`
   - `PUT /api/categories/{categoryId}`
   - `DELETE /api/categories/{categoryId}`
5. Verify public endpoints anonymously:
   - `GET /api/posts`
   - `GET /api/posts?q=architecture`
   - `GET /api/posts/{postId}`
   - `POST /api/posts/{postId}/reactions`

The sample request flow is also documented in
`src/backend/BlogPlatform.Api/BlogPlatform.Api.http`.

## Local API Setup With Docker Compose

Run the database and API together from the repository root:

```bash
docker compose up -d postgres api
docker compose ps
```

Default local Docker-based API URLs:

- Swagger UI: `http://localhost:5034/swagger`
- OpenAPI JSON: `http://localhost:5034/swagger/v1/swagger.json`

The API container connects to PostgreSQL through the Compose network and uses
the configured local frontend origins for browser-based development:

- `http://localhost:5173`
- `http://127.0.0.1:5173`

### Frontend Integration Notes

- Frontend post forms can load available categories from
  `GET /api/categories/available`.
- The available-categories endpoint is public because category names are not
  sensitive and the form needs them before any admin workflow.
- Category create, update, and deactivate endpoints remain administrator-only.
- Tag listing is intentionally deferred in this slice; the frontend should not
  assume a tag-selection endpoint exists yet.

## Local Frontend Setup

Run the frontend app from `src/frontend/blog-web`:

```bash
cd src/frontend/blog-web
npm install
npm run dev
```

Validation commands:

```bash
npm run lint
npm run build
```

### Frontend MVP Routes

- `/` public post listing
- `/posts/:postId` public post detail with like and dislike reactions
- `/register` user registration
- `/login` user login
- `/my-posts` authenticated author post list
- `/my-posts/new` authenticated post creation
- `/my-posts/:postId/edit` authenticated post editing
- `/admin/categories` administrator-only category management

### Search Notes

- The public listing route now supports backend-powered search through
  `GET /api/posts?q=...`.
- Anonymous searches return only public and available posts.
- Authenticated searches can also return the authenticated user’s own matching
  private posts, while still excluding private posts owned by other users.
- Clearing the search field returns the same default listing behavior as the
  original public feed.

### Frontend Demo Flow

1. Start the full stack with `docker compose up -d postgres api frontend`.
2. Open `http://localhost:5173`.
3. Browse public posts and open a post detail page.
4. Submit a public reaction.
5. Register a new account or log in with a seeded user:
   - `user@blogplatform.local` / `User123!`
   - `admin@blogplatform.local` / `Admin123!`
6. As a regular user, open `My Posts`, create a post, edit it, and remove it.
7. As the administrator, open `Categories` and create, update, or deactivate a category.

### Frontend Notes

- TailwindCSS remains the styling foundation, with shared tokens in
  `src/frontend/blog-web/tailwind.config.ts`.
- Global base styles live in `src/frontend/blog-web/src/index.css`.
- Reusable component styles live in
  `src/frontend/blog-web/src/styles/components.css`.
- Frontend design direction follows `DESIGN.md` and does not copy Ballast Lane
  branding directly.
- The current MVP intentionally avoids rich text editing, advanced client-side
  caching, and broader CMS-style workflows.
