import { requestJson } from '../../lib/api'
import type { AvailableCategory, PublicPostDetail, PublicPostSummary } from './post.types'

export function listPublicPosts() {
  return requestJson<PublicPostSummary[]>('/api/posts')
}

export function getPublicPost(postId: number) {
  return requestJson<PublicPostDetail>(`/api/posts/${postId}`)
}

export function listAvailableCategories() {
  return requestJson<AvailableCategory[]>('/api/categories/available')
}
