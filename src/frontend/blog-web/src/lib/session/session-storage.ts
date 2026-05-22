export type StoredSession = {
  token: string
  userId: number
  nameOrUsername: string
  email: string
  roles: string[]
}

const sessionKey = 'blog-platform.session'

export function getStoredSession(): StoredSession | null {
  const raw = localStorage.getItem(sessionKey)
  if (!raw) {
    return null
  }

  try {
    return JSON.parse(raw) as StoredSession
  } catch {
    localStorage.removeItem(sessionKey)
    return null
  }
}

export function storeSession(session: StoredSession) {
  localStorage.setItem(sessionKey, JSON.stringify(session))
}

export function clearStoredSession() {
  localStorage.removeItem(sessionKey)
}
