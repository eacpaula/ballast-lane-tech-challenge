function App() {
  return (
    <main className="page-shell">
      <div className="page-frame">
        <section className="section-stack border-b border-surface-border pb-10 lg:pb-section">
          <span className="eyebrow">Tailwind foundation active</span>
          <div className="space-y-stack-md">
            <p className="font-mono text-sm font-medium uppercase tracking-[0.2em] text-brand-strong">
              Blog platform design system
            </p>
            <h1 className="max-w-3xl font-display text-4xl font-bold tracking-display text-navy sm:text-5xl sm:leading-14">
              A clean frontend base for a technical interview-ready product.
            </h1>
            <p className="prose-lead">
              This shell proves the styling foundation is working before any real
              pages are built. The layout, typography, color, spacing, surface,
              and interaction patterns come from centralized tokens guided by
              <code>DESIGN.md</code>.
            </p>
          </div>
          <div className="flex flex-wrap gap-3">
            <button type="button" className="primary-action">
              Primary action
            </button>
            <a href="#token-preview" className="secondary-action">
              Review tokens
            </a>
          </div>
        </section>

        <section
          id="token-preview"
          className="grid gap-6 py-10 lg:grid-cols-[minmax(0,1.6fr)_minmax(18rem,1fr)] lg:py-section"
        >
          <article className="surface-card section-stack">
            <div className="space-y-stack-sm">
              <span className="eyebrow-chip">Engineering notes</span>
              <h2 className="font-display text-2xl font-semibold tracking-headline text-navy">
                Surface, hierarchy, and readable density
              </h2>
              <p className="text-base leading-[1.625] text-ink-soft">
                The first-pass shell keeps the interface restrained: warm accent
                color for emphasis, deep navy for structural emphasis, soft
                surfaces for scanning, and enough whitespace to make long-form
                technical content comfortable to read.
              </p>
            </div>

            <dl className="meta-row">
              <div>
                <dt className="sr-only">Typeface</dt>
                <dd>Geist + Inter + JetBrains Mono</dd>
              </div>
              <div>
                <dt className="sr-only">Container</dt>
                <dd>1200px centered frame</dd>
              </div>
              <div>
                <dt className="sr-only">Rhythm</dt>
                <dd>4px baseline spacing</dd>
              </div>
            </dl>
          </article>

          <aside className="surface-card justify-between bg-surface-tint">
            <div className="space-y-stack-sm">
              <p className="font-mono text-sm font-medium uppercase tracking-[0.18em] text-brand-strong">
                Token preview
              </p>
              <h2 className="font-display text-2xl font-semibold tracking-headline text-navy">
                Shared primitives, not one-off styling.
              </h2>
            </div>
            <ul className="space-y-stack-sm text-sm leading-6 text-ink-soft">
              <li>Responsive gutters are built into the page frame.</li>
              <li>Cards use low-contrast borders and ambient depth.</li>
              <li>Interactive elements expose visible keyboard focus.</li>
              <li>Mono labels are ready for tags, metadata, and code cues.</li>
            </ul>
          </aside>
        </section>
      </div>
    </main>
  )
}

export default App
