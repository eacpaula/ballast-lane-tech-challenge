export type AuthResponse = {
  userId: number
  nameOrUsername: string
  email: string
  authenticationPayload: string | null
}

export type LoginInput = {
  email: string
  password: string
}

export type RegisterInput = {
  nameOrUsername: string
  email: string
  password: string
}
