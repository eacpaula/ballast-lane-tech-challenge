---
name: Studio Engineering Aesthetic
colors:
  surface: '#f8f9fa'
  surface-dim: '#d9dadb'
  surface-bright: '#f8f9fa'
  surface-container-lowest: '#ffffff'
  surface-container-low: '#f3f4f5'
  surface-container: '#edeeef'
  surface-container-high: '#e7e8e9'
  surface-container-highest: '#e1e3e4'
  on-surface: '#191c1d'
  on-surface-variant: '#584237'
  inverse-surface: '#2e3132'
  inverse-on-surface: '#f0f1f2'
  outline: '#8c7164'
  outline-variant: '#e0c0b1'
  surface-tint: '#9d4300'
  primary: '#9d4300'
  on-primary: '#ffffff'
  primary-container: '#f97316'
  on-primary-container: '#582200'
  inverse-primary: '#ffb690'
  secondary: '#565e74'
  on-secondary: '#ffffff'
  secondary-container: '#dae2fd'
  on-secondary-container: '#5c647a'
  tertiary: '#505f76'
  on-tertiary: '#ffffff'
  tertiary-container: '#8c9cb4'
  on-tertiary-container: '#243348'
  error: '#ba1a1a'
  on-error: '#ffffff'
  error-container: '#ffdad6'
  on-error-container: '#93000a'
  primary-fixed: '#ffdbca'
  primary-fixed-dim: '#ffb690'
  on-primary-fixed: '#341100'
  on-primary-fixed-variant: '#783200'
  secondary-fixed: '#dae2fd'
  secondary-fixed-dim: '#bec6e0'
  on-secondary-fixed: '#131b2e'
  on-secondary-fixed-variant: '#3f465c'
  tertiary-fixed: '#d3e4fe'
  tertiary-fixed-dim: '#b7c8e1'
  on-tertiary-fixed: '#0b1c30'
  on-tertiary-fixed-variant: '#38485d'
  background: '#f8f9fa'
  on-background: '#191c1d'
  surface-variant: '#e1e3e4'
typography:
  display:
    fontFamily: Geist
    fontSize: 48px
    fontWeight: '700'
    lineHeight: 56px
    letterSpacing: -0.02em
  headline-lg:
    fontFamily: Geist
    fontSize: 32px
    fontWeight: '600'
    lineHeight: 40px
    letterSpacing: -0.01em
  headline-lg-mobile:
    fontFamily: Geist
    fontSize: 24px
    fontWeight: '600'
    lineHeight: 32px
  headline-md:
    fontFamily: Geist
    fontSize: 24px
    fontWeight: '600'
    lineHeight: 32px
  body-lg:
    fontFamily: Inter
    fontSize: 18px
    fontWeight: '400'
    lineHeight: 30px
  body-md:
    fontFamily: Inter
    fontSize: 16px
    fontWeight: '400'
    lineHeight: 26px
  label-md:
    fontFamily: JetBrains Mono
    fontSize: 14px
    fontWeight: '500'
    lineHeight: 20px
  code-sm:
    fontFamily: JetBrains Mono
    fontSize: 13px
    fontWeight: '400'
    lineHeight: 18px
rounded:
  sm: 0.25rem
  DEFAULT: 0.5rem
  md: 0.75rem
  lg: 1rem
  xl: 1.5rem
  full: 9999px
spacing:
  base: 4px
  container-max: 1200px
  gutter: 24px
  margin-mobile: 16px
  stack-sm: 8px
  stack-md: 16px
  stack-lg: 32px
  section-gap: 80px
---

## Brand & Style
The design system is built for a technical blog platform that prioritizes clarity, authority, and developer-centric utility. The brand personality is that of a high-end product engineering studio: confident, precise, and devoid of unnecessary flourish. It targets an audience of engineers, architects, and technical leaders who value information density balanced with high legibility.

The visual style follows a **Modern Corporate** approach with **Minimalist** influences. It utilizes a structured grid, ample whitespace to reduce cognitive load during long-form reading, and a focus on functional aesthetics. The emotional response should be one of reliability and technical sophistication, favoring a "tools-not-toys" philosophy.

## Colors
The palette is anchored by a high-contrast foundation to ensure technical content is the focus. 

- **Primary (#F97316):** Used sparingly for primary calls-to-action, active states, and key highlights. It provides a warm, energetic contrast to the cooler neutral tones.
- **Secondary/Deep Navy (#0F172A):** Applied to high-hierarchy elements such as navigation bars, footers, and primary headings to establish a grounded, professional structure.
- **Neutral/Background (#F9FAFB):** A clean, off-white surface that reduces glare during extended reading sessions.
- **Surface & Borders (#E2E8F0):** Used for card backgrounds and subtle UI partitioning. This low-contrast border approach maintains a clean look without the heaviness of dark lines.

## Typography
The typography system uses a tri-font approach to differentiate between branding, content, and data.

- **Headlines:** Uses **Geist** for a modern, technical feel with precise geometric construction.
- **Body:** Uses **Inter** for its exceptional readability in long-form prose and technical documentation. Line heights are intentionally generous (1.6x) to facilitate scanning.
- **Data & Labels:** Uses **JetBrains Mono** for metadata, tags, and code snippets to reinforce the developer-focused nature of the platform.

Mobile adjustments reduce the scale of large display type to ensure headers do not push primary content off-screen.

## Layout & Spacing
This design system utilizes a **Fixed Grid** model for desktop to maintain optimal line lengths for reading (60-80 characters). 

- **Desktop:** 12-column grid, 1200px max-width, centered.
- **Tablet:** 8-column fluid grid with 32px side margins.
- **Mobile:** 4-column fluid grid with 16px side margins.

The spacing rhythm is based on a 4px baseline. Vertical rhythm (stacking) is prioritized to separate content blocks, with a standard 80px gap between major page sections to maintain a clean, airy aesthetic.

## Elevation & Depth
Depth is conveyed through **Tonal Layers** and **Low-Contrast Outlines**. 

- **Level 0 (Background):** #F9FAFB.
- **Level 1 (Cards/Surface):** #FFFFFF with a 1px solid border of #E2E8F0.
- **Shadows:** Only used on Level 1 elements. Shadows must be ambient and diffused: `0 4px 6px -1px rgb(0 0 0 / 0.05), 0 2px 4px -2px rgb(0 0 0 / 0.05)`. 
- **Hover States:** Elements should lift slightly using a more pronounced shadow or a subtle background shift to #F8FAFC, never a heavy drop shadow.

## Shapes
The shape language is consistently **Rounded**, reflecting a modern software interface. 

- Standard components (Buttons, Inputs, Cards) use a **0.5rem (8px)** radius.
- Large containers or featured sections use **1rem (16px)**.
- Interaction indicators (Focus rings) should follow the radius of the element they surround with a 2px offset.

## Components
- **Buttons:** Primary buttons use the Orange #F97316 background with white text. Secondary buttons use a White background with #E2E8F0 borders and Navy text. All buttons use 8px rounding and semi-bold labels.
- **Input Fields:** Use a white background, 8px rounding, and a 1px border. Focus state must use a 2px solid Navy ring or a Primary Orange glow with 0% offset.
- **Cards:** White background, 1px #E2E8F0 border. For technical posts, cards should feature a JetBrains Mono "tag" at the top for category classification.
- **Chips/Tags:** Small, 4px rounded elements with a light gray background (#F1F5F9) and Navy text.
- **Code Blocks:** Dark background (#0F172A) with 8px rounding and 16px internal padding, utilizing JetBrains Mono for the content.
- **Lists:** Unordered lists in articles should use custom square markers in Primary Orange to differentiate from standard browser styling.