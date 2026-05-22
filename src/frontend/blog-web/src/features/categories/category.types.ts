export type ManagedCategory = {
  id: number
  title: string
  description: string | null
  isAvailable: boolean
}

export type AvailableManagedCategory = {
  id: number
  title: string
  description: string | null
  isAvailable: boolean
}

export type PaginatedCategoryResponse<TCategory> = {
  items: TCategory[]
  page: number
  pageSize: number
  totalCount: number
  totalPages: number
  hasNextPage: boolean
}

export type CategoryMutationResponse = ManagedCategory
