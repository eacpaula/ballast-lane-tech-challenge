import { requestJson } from '../../lib/api'
import type { AuthResponse, LoginInput, RegisterInput } from './auth.types'

export function login(input: LoginInput) {
  return requestJson<AuthResponse>('/api/auth/login', {
    method: 'POST',
    body: input,
  })
}

export function register(input: RegisterInput) {
  return requestJson<AuthResponse>('/api/auth/register', {
    method: 'POST',
    body: input,
  })
}
