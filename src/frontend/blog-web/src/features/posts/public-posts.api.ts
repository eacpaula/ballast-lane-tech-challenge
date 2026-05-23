import { requestJson } from '../../lib/api'
import type { AvailableCategory, PaginatedPublicPostResponse, PublicPostDetail } from './post.types'
import type { PaginatedCategoryResponse } from '../categories/category.types'

type ListPublicPostsOptions = {
  query?: string | null
  page?: number
  pageSize?: number
  token?: string | null
}

export function listPublicPosts(options: ListPublicPostsOptions = {}) {
  const searchParams = new URLSearchParams()
  const trimmedQuery = options.query?.trim()

  if (trimmedQuery) {
    searchParams.set('q', trimmedQuery)
  }

  if (typeof options.page === 'number') {
    searchParams.set('page', String(options.page))
  }

  if (typeof options.pageSize === 'number') {
    searchParams.set('pageSize', String(options.pageSize))
  }

  const path = searchParams.size > 0 ? `/api/posts?${searchParams.toString()}` : '/api/posts'

  return requestJson<PaginatedPublicPostResponse>(path, { token: options.token })
}

export function getPublicPost(postId: number) {
  return requestJson<PublicPostDetail>(`/api/posts/${postId}`)
}

export function listAvailableCategories() {
  return requestJson<PaginatedCategoryResponse<AvailableCategory>>('/api/categories/available?page=1&pageSize=100')
    .then((response) => response.items)
}
