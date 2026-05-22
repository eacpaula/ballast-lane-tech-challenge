import { useState } from 'react'
import { NavLink, useNavigate } from 'react-router-dom'
import Button from '../../components/Button'
import ErrorMessage from '../../components/ErrorMessage'
import { ApiError } from '../../lib/api'
import { register } from './auth.api'

export default function RegisterPage() {
  const navigate = useNavigate()
  const [nameOrUsername, setNameOrUsername] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState<string | null>(null)
  const [success, setSuccess] = useState<string | null>(null)
  const [isSubmitting, setIsSubmitting] = useState(false)

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setError(null)
    setSuccess(null)
    setIsSubmitting(true)

    try {
      await register({ nameOrUsername, email, password })
      setSuccess('Registration complete. You can now log in.')
      setTimeout(() => navigate('/login'), 750)
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : 'Unable to register.')
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <section className="auth-page">
      <div className="login-card">
        <div className="login-intro">
          <div className="login-security-chip">
            <svg viewBox="0 0 24 24" aria-hidden="true" className="login-security-icon">
              <path
                d="M12 3.5 5.5 6v5.1c0 4 2.7 7.6 6.5 8.9 3.8-1.3 6.5-4.9 6.5-8.9V6L12 3.5Zm0 4.2a2.2 2.2 0 1 1 0 4.4 2.2 2.2 0 0 1 0-4.4Zm0 9.2a6.2 6.2 0 0 1-3.7-1.2c.2-1.7 1.7-3 3.7-3s3.5 1.3 3.7 3A6.2 6.2 0 0 1 12 16.9Z"
                fill="currentColor"
              />
            </svg>
            <span>Create Account</span>
          </div>

          <div className="space-y-stack-sm">
            <h1 className="login-title">Join the platform</h1>
            <p className="login-copy">
              Create your account to publish technical posts and manage your writing workflow.
            </p>
          </div>
        </div>

        <div className="login-notice">
          <svg viewBox="0 0 24 24" aria-hidden="true" className="login-notice-icon">
            <path
              d="M12 3.8a8.2 8.2 0 1 0 0 16.4 8.2 8.2 0 0 0 0-16.4Zm0 4.1a1.1 1.1 0 1 1 0 2.2 1.1 1.1 0 0 1 0-2.2Zm1.1 8.2h-2.2v-5.2h2.2v5.2Z"
              fill="currentColor"
            />
          </svg>
          <p>Registration creates your account. Login remains a separate step.</p>
        </div>

        {error ? <ErrorMessage title="Registration failed" detail={error} /> : null}
        {success ? <p className="success-banner">{success}</p> : null}

        <form className="login-form" onSubmit={handleSubmit}>
          <label className="form-field" htmlFor="register-name">
            <span className="field-label">Name or Username</span>
            <div className="login-input-shell">
              <svg viewBox="0 0 24 24" aria-hidden="true" className="login-input-icon">
                <path
                  d="M12 6.2a3.1 3.1 0 1 1 0 6.2 3.1 3.1 0 0 1 0-6.2Zm0 8.1c3.1 0 5.6 1.9 6.3 4.5H5.7c.7-2.6 3.2-4.5 6.3-4.5Z"
                  fill="none"
                  stroke="currentColor"
                  strokeWidth="1.8"
                  strokeLinecap="round"
                  strokeLinejoin="round"
                />
              </svg>
              <input
                id="register-name"
                name="nameOrUsername"
                autoComplete="username"
                value={nameOrUsername}
                onChange={(event) => setNameOrUsername(event.target.value)}
                placeholder="Evandro Antônio da Costa de Paula"
                className="login-input"
                required
              />
            </div>
          </label>

          <label className="form-field" htmlFor="register-email">
            <span className="field-label">Email Address</span>
            <div className="login-input-shell">
              <svg viewBox="0 0 24 24" aria-hidden="true" className="login-input-icon">
                <path
                  d="M4.5 6.5h15a1 1 0 0 1 1 1v9a1 1 0 0 1-1 1h-15a1 1 0 0 1-1-1v-9a1 1 0 0 1 1-1Zm0 1.7 7.1 5.2a.7.7 0 0 0 .8 0l7.1-5.2"
                  fill="none"
                  stroke="currentColor"
                  strokeWidth="1.8"
                  strokeLinecap="round"
                  strokeLinejoin="round"
                />
              </svg>
              <input
                id="register-email"
                name="email"
                type="email"
                autoComplete="email"
                value={email}
                onChange={(event) => setEmail(event.target.value)}
                placeholder="eacpaula@outlook.com"
                className="login-input"
                required
              />
            </div>
          </label>

          <label className="form-field" htmlFor="register-password">
            <span className="field-label">Password</span>
            <div className="login-input-shell">
              <svg viewBox="0 0 24 24" aria-hidden="true" className="login-input-icon">
                <path
                  d="M8 10V8.3a4 4 0 1 1 8 0V10m-9 0h10a1 1 0 0 1 1 1v7a1 1 0 0 1-1 1H7a1 1 0 0 1-1-1v-7a1 1 0 0 1 1-1Z"
                  fill="none"
                  stroke="currentColor"
                  strokeWidth="1.8"
                  strokeLinecap="round"
                  strokeLinejoin="round"
                />
              </svg>
              <input
                id="register-password"
                name="password"
                type="password"
                autoComplete="new-password"
                value={password}
                onChange={(event) => setPassword(event.target.value)}
                placeholder="Create a strong password"
                className="login-input"
                required
              />
            </div>
          </label>

          <Button type="submit" className="login-submit-button" disabled={isSubmitting}>
            {isSubmitting ? 'Registering…' : 'Create Account'}
          </Button>

          <div className="login-divider" />

          <p className="login-register-row">
            Already have an account?{' '}
            <NavLink to="/login" className="login-register-link">
              Login here
            </NavLink>
          </p>
        </form>

      </div>
    </section>
  )
}
