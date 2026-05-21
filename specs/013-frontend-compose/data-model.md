# Data Model: Frontend Docker Compose Service

This feature does not introduce persistence entities. The relevant model is a
small runtime configuration model for the local stack.

## FrontendRuntimeService

- **Purpose**: Represents the frontend service added to the local Compose stack.
- **Fields**:
  - `serviceName`: frontend service identifier in Compose
  - `buildContext`: location used to build the frontend container
  - `startupCommand`: development server entrypoint
  - `exposedPort`: browser-facing local port
  - `dependsOn`: runtime relationship to existing services
- **Validation rules**:
  - Must remain local-development oriented.
  - Must not require production hosting infrastructure.

## FrontendEnvironmentConfig

- **Purpose**: Defines the environment passed to the frontend for local API
  communication.
- **Fields**:
  - `apiBaseUrl`: browser-valid URL for the local API
  - `frontendPort`: local frontend browser port
  - `frontendHostBinding`: host/interface binding suitable for container usage
- **Validation rules**:
  - Configuration must be minimal and documented.
  - API target must remain usable from the browser.

## ApiLocalCorsConfig

- **Purpose**: Represents the local-origin allowance needed for frontend access
  during Compose-based development.
- **Fields**:
  - `allowedOrigins`: list of permitted local frontend origins
  - `appliesTo`: local development/browser access only
- **Validation rules**:
  - Must remain explicit and narrow.
  - Must not weaken backend authorization behavior.

## Relationships

- `FrontendRuntimeService` consumes `FrontendEnvironmentConfig`.
- `FrontendEnvironmentConfig` must align with `ApiLocalCorsConfig`.
- `ApiLocalCorsConfig` must support the browser-visible frontend origin used by
  `FrontendRuntimeService`.
