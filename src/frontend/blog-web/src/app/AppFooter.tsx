import { useEffect, useState } from 'react'
import { NavLink } from 'react-router-dom'

type FooterModalKey = 'documentation' | 'privacy' | 'terms'

const modalContent: Record<FooterModalKey, { title: string; body: string[] }> = {
  documentation: {
    title: 'Documentation',
    body: [
      'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus suscipit, lorem in efficitur gravida, turpis massa fermentum est, ac tincidunt nibh augue id justo.',
      'Suspendisse potenti. Integer accumsan, nisl in malesuada malesuada, lectus velit tincidunt nibh, in posuere libero nunc at odio. Donec volutpat neque vitae tellus tincidunt, sit amet luctus lacus congue.',
    ],
  },
  privacy: {
    title: 'Privacy Policy',
    body: [
      'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla facilisi. Sed pretium, augue ac egestas pulvinar, lectus libero gravida purus, vel convallis sem mauris sed velit.',
      'Mauris non porta augue. Phasellus in velit non sapien aliquam tincidunt. Proin iaculis lacus at nunc efficitur, a aliquet est faucibus.',
    ],
  },
  terms: {
    title: 'Terms of Service',
    body: [
      'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Curabitur fermentum mi sed justo imperdiet, a vestibulum sapien volutpat. Aliquam erat volutpat.',
      'Etiam malesuada neque sit amet ipsum posuere, nec consequat risus dignissim. Integer volutpat luctus lacus, nec dictum justo lobortis vel.',
    ],
  },
}

export default function AppFooter() {
  const [activeModal, setActiveModal] = useState<FooterModalKey | null>(null)

  useEffect(() => {
    if (!activeModal) {
      return
    }

    function handleEscape(event: KeyboardEvent) {
      if (event.key === 'Escape') {
        setActiveModal(null)
      }
    }

    window.addEventListener('keydown', handleEscape)
    return () => window.removeEventListener('keydown', handleEscape)
  }, [activeModal])

  return (
    <>
      <footer className="app-footer">
        <div className="app-footer-brand">
          <NavLink to="/" className="app-footer-brand-link">
            <img
              src="/logo.png"
              alt="Tech Blog Platform logo"
              className="app-footer-brand-mark"
            />
            <span>Tech Blog Platform</span>
          </NavLink>
          <p className="app-footer-copy">
            &copy; 2026 Blog Platform. Built for product-minded engineers.
          </p>
        </div>

        <nav aria-label="Footer" className="app-footer-nav">
          <button
            type="button"
            className="app-footer-link"
            onClick={() => setActiveModal('documentation')}
          >
            Documentation
          </button>
          <button
            type="button"
            className="app-footer-link"
            onClick={() => setActiveModal('privacy')}
          >
            Privacy Policy
          </button>
          <button
            type="button"
            className="app-footer-link"
            onClick={() => setActiveModal('terms')}
          >
            Terms of Service
          </button>
          <a
            href="https://github.com/eacpaula/"
            target="_blank"
            rel="noreferrer"
            className="app-footer-link"
          >
            Github
          </a>
        </nav>
      </footer>

      {activeModal ? (
        <div
          className="app-modal-overlay"
          role="presentation"
          onClick={() => setActiveModal(null)}
        >
          <div
            role="dialog"
            aria-modal="true"
            aria-labelledby="app-footer-modal-title"
            className="app-modal"
            onClick={(event) => event.stopPropagation()}
          >
            <div className="app-modal-header">
              <h2 id="app-footer-modal-title" className="app-modal-title">
                {modalContent[activeModal].title}
              </h2>
              <button
                type="button"
                className="app-modal-close"
                aria-label="Close modal"
                onClick={() => setActiveModal(null)}
              >
                Close
              </button>
            </div>

            <div className="app-modal-body">
              {modalContent[activeModal].body.map((paragraph) => (
                <p key={paragraph}>{paragraph}</p>
              ))}
            </div>
          </div>
        </div>
      ) : null}
    </>
  )
}
