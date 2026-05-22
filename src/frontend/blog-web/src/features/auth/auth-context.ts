import { createContext } from 'react'
import type { StoredSession } from '../../lib/session/session-storage'
import type { AuthResponse } from './auth.types'

export type AuthContextValue = {
  user: StoredSession | null
  isAuthenticated: boolean
  isAdministrator: boolean
  loginWithResponse: (response: AuthResponse) => void
  logout: () => void
}

export const AuthContext = createContext<AuthContextValue | undefined>(undefined)
