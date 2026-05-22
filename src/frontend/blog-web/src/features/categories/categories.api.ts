import { requestJson } from '../../lib/api'
import type { CategoryMutationResponse, ManagedCategory } from './category.types'

export function listManagedCategories(token: string) {
  return requestJson<ManagedCategory[]>('/api/categories', { token })
}

export function createCategory(title: string, token: string) {
  return requestJson<CategoryMutationResponse>('/api/categories', {
    method: 'POST',
    token,
    body: { title },
  })
}

export function updateCategory(categoryId: number, title: string, token: string) {
  return requestJson<CategoryMutationResponse>(`/api/categories/${categoryId}`, {
    method: 'PUT',
    token,
    body: { title },
  })
}

export function deactivateCategory(categoryId: number, token: string) {
  return requestJson<CategoryMutationResponse>(`/api/categories/${categoryId}`, {
    method: 'DELETE',
    token,
  })
}
