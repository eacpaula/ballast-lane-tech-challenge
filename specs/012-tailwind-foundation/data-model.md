# Data Model: Tailwind Frontend Foundation

This feature does not introduce persistence entities. The relevant "model" is a
small set of frontend foundation concepts that future UI work will reuse.

## DesignTokenSet

- **Purpose**: Represents the centralized visual tokens derived from
  `DESIGN.md`.
- **Fields**:
  - `colorTokens`: background, surface, text, accent, outline, and semantic
    surface values
  - `fontTokens`: headline, body, and mono/data font families
  - `spacingTokens`: baseline spacing units, gutters, and section gaps
  - `radiusTokens`: shared rounding values for controls and containers
  - `shadowTokens`: ambient surface elevation values
  - `layoutTokens`: centered content width and responsive spacing constraints
- **Validation rules**:
  - Tokens must map to the approved design direction rather than unrelated
    branding.
  - Tokens must be shared from a centralized source, not scattered through
    individual UI files.

## GlobalStyleFoundation

- **Purpose**: Defines the shared base styling rules that every screen will
  inherit.
- **Fields**:
  - `rootStyles`: body/background/text defaults
  - `typeScale`: headline, body, label, and code defaults
  - `focusStyles`: visible interactive focus treatment
  - `layoutDefaults`: page/container behavior and responsive gutters
  - `surfaceDefaults`: border, shadow, and background patterns for cards and
    sections
- **Validation rules**:
  - Must remain small and global by intent.
  - Must preserve readable typography and accessible defaults.

## ValidationShell

- **Purpose**: Minimal UI proof that the Tailwind foundation is active.
- **Fields**:
  - `pageFrame`: centered shell or layout wrapper
  - `heroContent`: small heading/body proof of typography and spacing
  - `surfaceExample`: one card-like container proving surface tokens
  - `actionExample`: one button-like control proving interaction styling
  - `metaExample`: one tag or label proving mono/metadata treatment
- **Validation rules**:
  - Must stay static and scope-limited.
  - Must not become a disguised first page implementation.

## Relationships

- `DesignTokenSet` feeds `GlobalStyleFoundation`.
- `GlobalStyleFoundation` styles `ValidationShell`.
- `ValidationShell` provides visible confirmation that `DesignTokenSet` was
  mapped correctly.
