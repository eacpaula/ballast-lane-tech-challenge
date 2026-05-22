const defaultApiBaseUrl = 'http://localhost:5034'

export const appConfig = {
  apiBaseUrl: import.meta.env.VITE_API_BASE_URL?.trim() || defaultApiBaseUrl,
}
