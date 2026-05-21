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
- React Router

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
