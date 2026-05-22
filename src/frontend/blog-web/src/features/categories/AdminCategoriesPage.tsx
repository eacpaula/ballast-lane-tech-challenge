import { useEffect, useState } from 'react'
import Button from '../../components/Button'
import EmptyState from '../../components/EmptyState'
import ErrorMessage from '../../components/ErrorMessage'
import LoadingState from '../../components/LoadingState'
import { useAuth } from '../auth/useAuth'
import CategoryForm from './CategoryForm'
import CategoryListItem from './CategoryListItem'
import { createCategory, deactivateCategory, listManagedCategories, updateCategory } from './categories.api'
import type { ManagedCategory, PaginatedCategoryResponse } from './category.types'

export default function AdminCategoriesPage() {
  const { user } = useAuth()
  const [categories, setCategories] = useState<PaginatedCategoryResponse<ManagedCategory> | null>(null)
  const [editing, setEditing] = useState<ManagedCategory | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [busyCategoryId, setBusyCategoryId] = useState<number | null>(null)
  const [page, setPage] = useState(1)
  const pageSize = 10

  useEffect(() => {
    if (!user?.token) {
      return
    }

    let active = true
    void listManagedCategories(user.token, { page, pageSize })
      .then((response) => {
        if (active) {
          setCategories(response)
          setError(null)
        }
      })
      .catch((caught) => {
        if (active) {
          setError(caught instanceof Error ? caught.message : 'Unable to load categories.')
        }
      })

    return () => {
      active = false
    }
  }, [page, pageSize, user?.token])

  async function handleCreate(title: string, description: string) {
    if (!user?.token) {
      return
    }

    setError(null)
    setIsSubmitting(true)
    try {
      await createCategory(title, description, user.token)
      setEditing(null)
      setPage(1)
      const response = await listManagedCategories(user.token, { page: 1, pageSize })
      setCategories(response)
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : 'Unable to create category.')
    } finally {
      setIsSubmitting(false)
    }
  }

  async function handleUpdate(title: string, description: string) {
    if (!user?.token || !editing) {
      return
    }

    setError(null)
    setIsSubmitting(true)
    try {
      await updateCategory(editing.id, title, description, user.token)
      const response = await listManagedCategories(user.token, { page, pageSize })
      setCategories(response)
      setEditing(null)
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : 'Unable to update category.')
    } finally {
      setIsSubmitting(false)
    }
  }

  async function handleDeactivate(categoryId: number) {
    if (!user?.token) {
      return
    }

    setError(null)
    try {
      const updated = await deactivateCategory(categoryId, user.token)
      setCategories((current) =>
        current
          ? {
              ...current,
              items: current.items.map((category) => (category.id === updated.id ? updated : category)),
            }
          : current,
      )
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : 'Unable to deactivate category.')
    }
  }

  if (error && !categories) {
    return <ErrorMessage detail={error} />
  }

  if (!categories) {
    return <LoadingState message="Loading all categories…" />
  }

  return (
    <section className="category-admin-page">
      <div className="space-y-stack-sm">
        <p className="app-kicker">Administrator tools</p>
        <h1 className="page-title">Category Management</h1>
        <p className="section-copy">
          Organize and manage post taxonomies to improve site discoverability.
        </p>
      </div>

      {error ? <ErrorMessage detail={error} /> : null}

      <section className="category-admin-layout">
        <section className="category-admin-panel">
          <div className="space-y-stack-sm">
            <h2 className="category-admin-panel-title">
              {editing ? 'Update category' : 'Create category'}
            </h2>
            <p className="text-base leading-[1.625] text-ink-soft">
              Keep category titles short, clear, and reusable across posts.
            </p>
          </div>
          <CategoryForm
            key={editing?.id ?? 'create'}
            initialTitle={editing?.title ?? ''}
            initialDescription={editing?.description ?? ''}
            submitLabel={editing ? 'Save Category' : 'Add Category'}
            isSubmitting={isSubmitting}
            secondaryActionLabel={editing ? 'Cancel edit' : undefined}
            onSecondaryAction={editing ? () => setEditing(null) : undefined}
            onSubmit={editing ? handleUpdate : handleCreate}
          />
        </section>

        <section className="category-admin-table-panel">
          <div className="category-admin-table-head">
            <p className="category-admin-table-kicker">Existing Categories</p>
          </div>
          {categories.items.length === 0 ? (
            <EmptyState
              title="No categories created yet"
              message="Create the first category to make post organization available to authors."
            />
          ) : (
            <>
              <div className="category-admin-table-wrap">
                <table className="category-admin-table">
                  <thead>
                    <tr>
                      <th>Category Name</th>
                      <th>Internal Id</th>
                      <th>Status</th>
                      <th>Actions</th>
                    </tr>
                  </thead>
                  <tbody>
                    {categories.items.map((category) => (
                      <CategoryListItem
                        key={category.id}
                        category={category}
                        isBusy={busyCategoryId === category.id}
                        onEdit={setEditing}
                        onDeactivate={async (categoryId) => {
                          setBusyCategoryId(categoryId)
                          try {
                            await handleDeactivate(categoryId)
                          } finally {
                            setBusyCategoryId(null)
                          }
                        }}
                      />
                    ))}
                  </tbody>
                </table>
              </div>

              <div className="category-admin-table-footer">
                <span>
                  Showing page {categories.page} of {Math.max(categories.totalPages, 1)} · {categories.totalCount}{' '}
                  categor{categories.totalCount === 1 ? 'y' : 'ies'}
                </span>
                <div className="category-admin-row-actions">
                  <Button
                    type="button"
                    variant="secondary"
                    disabled={categories.page <= 1}
                    onClick={() => setPage((current) => Math.max(1, current - 1))}
                  >
                    Previous
                  </Button>
                  <Button
                    type="button"
                    variant="secondary"
                    disabled={!categories.hasNextPage}
                    onClick={() => setPage((current) => current + 1)}
                  >
                    Next
                  </Button>
                </div>
              </div>
            </>
          )}
        </section>
      </section>
    </section>
  )
}
