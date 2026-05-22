import {
  useMemo,
  useState,
  type ReactNode,
} from 'react'
import { extractRoles } from '../../lib/session'
import { clearStoredSession, getStoredSession, storeSession, type StoredSession } from '../../lib/session/session-storage'
import { AuthContext, type AuthContextValue } from './auth-context'

export default function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<StoredSession | null>(() => getStoredSession())

  const value = useMemo<AuthContextValue>(
    () => ({
      user,
      isAuthenticated: Boolean(user?.token),
      isAdministrator: Boolean(user?.roles.includes('Administrator')),
      loginWithResponse: (response) => {
        if (!response.authenticationPayload) {
          throw new Error('Login response did not include an authentication payload.')
        }

        const session: StoredSession = {
          token: response.authenticationPayload,
          userId: response.userId,
          nameOrUsername: response.nameOrUsername,
          email: response.email,
          roles: extractRoles(response.authenticationPayload),
        }

        storeSession(session)
        setUser(session)
      },
      logout: () => {
        clearStoredSession()
        setUser(null)
      },
    }),
    [user],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}
