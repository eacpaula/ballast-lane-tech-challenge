type DecodedJwt = {
  sub?: string
  email?: string
  name?: string
  role?: string | string[]
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'?: string | string[]
}

export function decodeJwt(token: string): DecodedJwt | null {
  const parts = token.split('.')
  if (parts.length < 2) {
    return null
  }

  try {
    const normalized = parts[1].replace(/-/g, '+').replace(/_/g, '/')
    const payload = JSON.parse(atob(normalized))
    return payload as DecodedJwt
  } catch {
    return null
  }
}

export function extractRoles(token: string) {
  const payload = decodeJwt(token)
  if (!payload) {
    return []
  }

  const roleClaim =
    payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ?? payload.role

  if (!roleClaim) {
    return []
  }

  return Array.isArray(roleClaim) ? roleClaim : [roleClaim]
}
