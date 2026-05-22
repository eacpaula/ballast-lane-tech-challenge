import { requestJson } from '../../lib/api'
import type { CategoryMutationResponse, ManagedCategory, PaginatedCategoryResponse } from './category.types'

type ListManagedCategoriesOptions = {
  page?: number
  pageSize?: number
}

export function listManagedCategories(token: string, options: ListManagedCategoriesOptions = {}) {
  const searchParams = new URLSearchParams()
  searchParams.set('page', String(options.page ?? 1))
  searchParams.set('pageSize', String(options.pageSize ?? 10))

  return requestJson<PaginatedCategoryResponse<ManagedCategory>>(`/api/categories?${searchParams.toString()}`, { token })
}

export function createCategory(title: string, description: string, token: string) {
  return requestJson<CategoryMutationResponse>('/api/categories', {
    method: 'POST',
    token,
    body: { title, description },
  })
}

export function updateCategory(categoryId: number, title: string, description: string, token: string) {
  return requestJson<CategoryMutationResponse>(`/api/categories/${categoryId}`, {
    method: 'PUT',
    token,
    body: { title, description },
  })
}

export function deactivateCategory(categoryId: number, token: string) {
  return requestJson<CategoryMutationResponse>(`/api/categories/${categoryId}`, {
    method: 'DELETE',
    token,
  })
}
