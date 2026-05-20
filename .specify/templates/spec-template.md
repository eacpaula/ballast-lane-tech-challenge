# Feature Specification: [FEATURE NAME]

**Feature Branch**: `[###-feature-name]`

**Created**: [DATE]

**Status**: Draft

**Input**: User description: "$ARGUMENTS"

## User Scenarios & Testing *(mandatory)*

User stories MUST be prioritized by delivery order and independently testable.
For this repository, backend behavior is the primary unit of delivery: each
story MUST state the business rules it exercises and the tests that will be
written before implementation begins.

### User Story 1 - [Brief Title] (Priority: P1)

[Describe this user journey in plain language]

**Why this priority**: [Explain why this story is first]

**Business Rules Covered**: [List the validation, authorization, ownership, or
other rules this story must enforce]

**Independent Test**: [Describe how this story is proven end to end]

**Required Failing Tests First**:

- Unit/Application: [Describe the first failing test for the core rule]
- API Integration: [Describe the failing endpoint test]
- Repository: [Describe any PostgreSQL-backed repository test if applicable]

**Acceptance Scenarios**:

1. **Given** [initial state], **When** [action], **Then** [expected outcome]
2. **Given** [initial state], **When** [action], **Then** [expected outcome]

---

### User Story 2 - [Brief Title] (Priority: P2)

[Describe this user journey in plain language]

**Why this priority**: [Explain why this story follows P1]

**Business Rules Covered**: [List the important rules]

**Independent Test**: [Describe how this story is proven independently]

**Required Failing Tests First**:

- Unit/Application: [Describe the first failing test]
- API Integration: [Describe the failing endpoint test]
- Repository: [Describe any repository test if applicable]

**Acceptance Scenarios**:

1. **Given** [initial state], **When** [action], **Then** [expected outcome]

---

### User Story 3 - [Brief Title] (Priority: P3)

[Describe this user journey in plain language]

**Why this priority**: [Explain why this story is later]

**Business Rules Covered**: [List the important rules]

**Independent Test**: [Describe how this story is proven independently]

**Required Failing Tests First**:

- Unit/Application: [Describe the first failing test]
- API Integration: [Describe the failing endpoint test]
- Repository: [Describe any repository test if applicable]

**Acceptance Scenarios**:

1. **Given** [initial state], **When** [action], **Then** [expected outcome]

---

[Add more user stories as needed, each with an assigned priority]

## Edge Cases

- How does the backend reject unauthenticated access where protection is
  required?
- How does the backend reject non-admin access to admin-only behavior?
- How does the backend reject ownership violations?
- What happens when validation fails or related data does not exist?
- What error contract should the API return for each failure mode?

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST [specific capability]
- **FR-002**: System MUST enforce [specific business rule]
- **FR-003**: System MUST return ProblemDetails-style responses for
  [specific failure mode]
- **FR-004**: System MUST persist [specific data] using the approved
  architecture boundaries
- **FR-005**: Frontend behavior MUST depend on backend-enforced rules rather
  than replace them

### Key Entities *(include if feature involves data)*

- **[Entity 1]**: [What it represents, key attributes without implementation]
- **[Entity 2]**: [What it represents, relationships to other entities]

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: [Measurable user or API outcome]
- **SC-002**: [Measurable validation, authorization, or workflow outcome]
- **SC-003**: [Measurable end-to-end story outcome]

## Backend-First Delivery Plan

- Backend API changes required for this feature: [list endpoints/services first]
- Domain/Application rules required before UI work: [list rules]
- Persistence changes required before UI work: [list tables/queries]
- Frontend work that depends on stable backend behavior: [list pages/components]

## Out of Scope

- [Explicitly list nearby ideas that are intentionally excluded]

## Assumptions

- [Assumption about target users or usage]
- [Assumption about scope boundaries]
- [Assumption about data or environment]
- [Assumption about existing dependencies]
