## Database Implementation Strategy

The project uses PostgreSQL running through Docker Compose for local development and demo purposes.

The database schema is based on the existing `Database-Schema-Diagram.md`, but the first implementation will focus only on the tables required by the current use cases: users, roles, user roles, posts, post categories, tags, post tags, and post reactions.

Schema creation and seed data will be handled through SQL scripts under the `database/` folder. This keeps the setup explicit and aligned with the challenge restriction against Entity Framework and Dapper.