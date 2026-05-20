Documentation about the Generative AI Usage and prompt engineering for this project.

## Spec Kit Framework

Spec Kit is an AI framework for defining and applying engineering principles to software development. It is based on the [OpenAI](https://openai.com) GPT-3.5 (ChatGPT) model.

## Spec Kit Constitution Prompt

I used Spec Kit to define project-level engineering principles before implementation. The goal was to keep the project aligned with the technical challenge constraints and avoid vague AI-assisted development.

The constitution was used to guide decisions around:
- Clean Architecture boundaries
- Custom data access without Entity Framework, Dapper, or Mediator
- Testability
- Small and stable implementation scope
- Clear documentation
- Responsible GenAI usage

For the prompt related to the constitution I used the "Codex" with **reasoning effort** set to "high" since the constitution is a complex document with multiple parts that need to be aligned.

### Prompt Used

```txt
[$speckit-constitution](../.agents/skills/speckit-constitution/SKILL.md) Create or update the Spec-Kit constitution for this repository.

Context:
This is a technical interview challenge for a simple full-stack blog platform. The goal is to demonstrate a clean, maintainable, testable implementation, not to build a full CMS.

Use the existing project documentation as source material:
- [DotNET - Technical Test - Informal User Story.md](docs/DotNET - Technical Test - Informal User Story.md) 
- [DotNET - Technical Test - Architectural Decisions.md](docs/DotNET - Technical Test - Architectural Decisions.md) 

Project summary:
- Backend: .NET 10 LTS
- API: ASP.NET Core Web API with controllers
- Frontend: React + Vite + TypeScript
- Database: PostgreSQL
- Data access: Raw SQL with Npgsql and custom repositories/gateways
- Authentication: JWT Bearer authentication
- Authorization: ASP.NET Core roles/policies plus business-level ownership rules
- Tests: xUnit, unit tests, API integration tests, and PostgreSQL-backed repository tests where practical
- Architecture: lightweight Clean Architecture with Domain, Application, Infrastructure/Data, API, Tests, and Frontend layers

Define the constitution with these principles:

1. Test-first development is mandatory
- Every backend feature must start with failing tests before implementation.
- Tests should describe expected behavior from the user story and business rules.
- Do not implement production code before defining the relevant tests.
- Prefer testing business rules first, especially ownership, authorization, validation, and error cases.

2. Backend-first delivery
- Initial specs and tasks should prioritize the backend API, domain rules, authentication, authorization, persistence, and tests.
- Frontend work should come after core backend flows are stable and test-covered.

3. Lightweight Clean Architecture
- Keep clear boundaries between Domain, Application, Infrastructure/Data, API, and Tests.
- Avoid unnecessary enterprise patterns such as MediatR, CQRS, generic repositories, or excessive abstractions.
- Business validation must live outside API controllers.
- Data access must be abstracted from the application layer.

4. Custom data access discipline
- Use raw SQL with Npgsql only.
- Do not use Entity Framework, Dapper, or other ORM/micro-ORM libraries.
- All SQL must be parameterized.
- SQL must stay isolated in Infrastructure/Data repositories or gateways.
- Application and Domain layers must not depend on Npgsql or SQL details.

5. Security and authorization
- Passwords must be securely hashed and never stored in plain text.
- JWT authentication must be implemented with explicit issuer, audience, signing key, and expiration configuration.
- Protected endpoints must reject unauthenticated requests.
- Admin endpoints must reject non-admin users.
- Users can edit or delete only posts they own.
- Authorization must be enforced in the backend, not only in the frontend.

6. Scope control
- Keep the MVP intentionally small.
- Include registration, login, public post listing/details, post CRUD, ownership validation, like/dislike, tag creation, and admin category management.
- Do not add comments, image uploads, rich text editing, password recovery, external identity providers, notifications, analytics, scheduling, or full CMS behavior unless explicitly requested later.

7. Reviewable interview-quality code
- Favor readable, explicit code over clever abstractions.
- Each feature should be easy to explain in a technical interview.
- Generated code must be manually reviewed.
- Important decisions should remain aligned with the architecture documents.

8. Consistent API behavior
- Use ProblemDetails-style responses for validation, not found, unauthorized, forbidden, conflict, and unexpected errors.
- Keep DTOs explicit.
- Keep OpenAPI documentation generated from the API implementation.

9. Practical test strategy
- Unit tests should cover domain/application business rules.
- API integration tests should cover public/protected/admin endpoints.
- Repository tests should cover critical raw SQL behavior using PostgreSQL where practical.
- Prioritize high-risk behavior over superficial coverage.

Output:
- Produce a complete constitution.md suitable for Spec-Kit.
- Keep it concise, professional, and enforceable.
- Use MUST/SHOULD language where appropriate.
- Include a section explaining how future specs and tasks must respect test-first development.
```

The final constitution is stored in `.specify/memory/constitution.md`.

