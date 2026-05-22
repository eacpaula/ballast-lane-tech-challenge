import { type FormEvent } from 'react'
import { NavLink, Outlet, useLocation, useNavigate, useSearchParams } from 'react-router-dom'
import { useAuth } from '../features/auth/useAuth'
import Button from '../components/Button'
import AppFooter from './AppFooter'

const publicLinks = [
  { to: '/', label: 'Posts' },
]

export default function AppShell() {
  const { isAuthenticated, isAdministrator, user, logout } = useAuth()
  const [searchParams] = useSearchParams()
  const location = useLocation()
  const navigate = useNavigate()
  const currentQuery = searchParams.get('q') ?? ''

  function handleSearchSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    const formData = new FormData(event.currentTarget)
    const rawQuery = formData.get('q')
    const searchInput = typeof rawQuery === 'string' ? rawQuery : ''

    const trimmedQuery = searchInput.trim()
    const nextSearchParams = new URLSearchParams()

    if (trimmedQuery) {
      nextSearchParams.set('q', trimmedQuery)
    }

    navigate({
      pathname: '/',
      search: nextSearchParams.toString() ? `?${nextSearchParams.toString()}` : '',
    })
  }

  return (
    <main className="page-shell">
      <div className="page-frame">
        <header className="app-header">
          <div className="app-header-main">
            <NavLink to="/" className="app-brand">
              <img
                src="/logo.png"
                alt="Tech Blog Platform logo"
                className="app-brand-mark"
              />
              <span className="app-brand-text">Tech Blog Platform</span>
            </NavLink>

            <nav aria-label="Primary" className="app-nav">
              {publicLinks.map((link) => (
                <NavLink
                  key={link.to}
                  to={link.to}
                  className={({ isActive }) =>
                    isActive && location.pathname === link.to
                      ? 'nav-link nav-link-active'
                      : 'nav-link'
                  }
                >
                  {link.label}
                </NavLink>
              ))}

              {isAuthenticated ? (
                <>
                  <NavLink
                    to="/my-posts"
                    className={({ isActive }) =>
                      isActive ? 'nav-link nav-link-active' : 'nav-link'
                    }
                  >
                    My Posts
                  </NavLink>
                  {isAdministrator ? (
                    <NavLink
                      to="/admin/categories"
                      className={({ isActive }) =>
                        isActive ? 'nav-link nav-link-active' : 'nav-link'
                      }
                    >
                      Categories
                    </NavLink>
                  ) : null}
                </>
              ) : null}
            </nav>
          </div>

          <div className="app-nav-wrap">
            <form className="app-search" role="search" onSubmit={handleSearchSubmit}>
              <button
                type="submit"
                className="app-search-button"
                aria-label="Search posts"
              >
                <svg viewBox="0 0 20 20" aria-hidden="true" className="app-search-icon">
                  <path
                    d="M13.5 12.1h-.7l-.2-.2a5.2 5.2 0 1 0-.7.7l.2.2v.7l4.1 4.1 1.2-1.2-4-4.3Zm-4.8 0a3.6 3.6 0 1 1 0-7.2 3.6 3.6 0 0 1 0 7.2Z"
                    fill="currentColor"
                  />
                </svg>
              </button>
              <input
                key={currentQuery}
                type="search"
                name="q"
                defaultValue={currentQuery}
                placeholder="Search posts..."
                className="app-search-input"
              />
            </form>
            <div className="app-session-panel">
              {isAuthenticated ? (
                <>
                  <div className="app-user-meta">
                    <span className="eyebrow-chip">{isAdministrator ? 'Administrator' : 'Member'}</span>
                    <p className="text-sm text-ink-soft">{user?.nameOrUsername}</p>
                  </div>
                  <Button type="button" variant="secondary" className="app-auth-button" onClick={logout}>
                    Logout
                  </Button>
                </>
              ) : (
                <>
                  <NavLink to="/login" className="app-auth-link">
                    Login
                  </NavLink>
                  <Button asChild className="app-auth-button">
                    <NavLink to="/register">Register</NavLink>
                  </Button>
                </>
              )}
            </div>
          </div>
        </header>

        <div className="content-stack">
          <Outlet />
        </div>

        <AppFooter />
      </div>
    </main>
  )
}
