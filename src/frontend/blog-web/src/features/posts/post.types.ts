export type PublicPostSummary = {
  id: number
  title: string
  summary: string | null
  tags: string[]
}

export type PublicPostDetail = {
  id: number
  title: string
  summary: string | null
  content: string
  tags: string[]
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
  tags: string[]
  publishDate: string | null
  expirationDate: string | null
}

export type OwnedPostDetail = {
  id: number
  title: string
  summary: string | null
  content: string
  categoryId: number
  isPublic: boolean
  isAvailable: boolean
  tags: string[]
  publishDate: string | null
  expirationDate: string | null
}

export type PostMutationRequest = {
  categoryId: number
  title: string
  summary: string
  content: string
  tags: string[]
  publishDate?: string | null
  expirationDate?: string | null
}

export type PostEditorDraft = Omit<PostMutationRequest, 'tags'> & {
  tags: string
  publishDate: string
  expirationDate: string
  isPublic: boolean
  isAvailable: boolean
}

export type PostMutationResponse = {
  id: number
  authorUserId: number
  title: string
  summary: string | null
  content: string | null
  tags: string[]
  publishDate: string | null
  expirationDate: string | null
}
