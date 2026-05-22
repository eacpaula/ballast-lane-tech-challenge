export type PublicPostSummary = {
  id: number
  title: string
  summary: string | null
}

export type PublicPostDetail = {
  id: number
  title: string
  summary: string | null
  content: string
}

export type ReactionType = 'like' | 'dislike'

export type ReactionResponse = {
  postId: number
  reactionType: string
  userId: number | null
  visitorIdentifier: string | null
}

export type AvailableCategory = {
  id: number
  title: string
  description: string | null
  isAvailable: boolean
}

export type OwnedPostSummary = {
  id: number
  title: string
  summary: string | null
  categoryId: number
  isPublic: boolean
  isAvailable: boolean
}

export type OwnedPostDetail = {
  id: number
  title: string
  summary: string | null
  content: string
  categoryId: number
  isPublic: boolean
  isAvailable: boolean
}

export type PostMutationRequest = {
  categoryId: number
  title: string
  summary: string
  content: string
}

export type PostEditorDraft = PostMutationRequest & {
  tags: string
  publishDate: string
  expireDate: string
  isPublic: boolean
  isAvailable: boolean
}

export type PostMutationResponse = {
  id: number
  authorUserId: number
  title: string
  summary: string | null
  content: string | null
}
