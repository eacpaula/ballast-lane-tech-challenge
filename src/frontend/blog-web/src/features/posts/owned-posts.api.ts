import { requestJson } from '../../lib/api'
import type {
  OwnedPostDetail,
  OwnedPostSummary,
  PostMutationRequest,
  PostMutationResponse,
} from './post.types'

export function listOwnedPosts(token: string) {
  return requestJson<OwnedPostSummary[]>('/api/posts/mine', { token })
}

export function getOwnedPost(postId: number, token: string) {
  return requestJson<OwnedPostDetail>(`/api/posts/mine/${postId}`, { token })
}

export function createPost(input: PostMutationRequest, token: string) {
  return requestJson<PostMutationResponse>('/api/posts', {
    method: 'POST',
    body: input,
    token,
  })
}

export function updatePost(postId: number, input: Omit<PostMutationRequest, 'categoryId'> & { publishDate?: string | null; expirationDate?: string | null }, token: string) {
  return requestJson<PostMutationResponse>(`/api/posts/${postId}`, {
    method: 'PUT',
    body: input,
    token,
  })
}

export function removePost(postId: number, token: string) {
  return requestJson<PostMutationResponse>(`/api/posts/${postId}`, {
    method: 'DELETE',
    token,
  })
}
