import { requestJson } from '../../lib/api'
import type { ReactionResponse, ReactionType } from './post.types'

export function reactToPost(
  postId: number,
  reactionType: ReactionType,
  visitorIdentifier: string | null,
  token?: string | null,
) {
  return requestJson<ReactionResponse>(`/api/posts/${postId}/reactions`, {
    method: 'POST',
    token,
    body: {
      reactionType,
      visitorIdentifier,
    },
  })
}
