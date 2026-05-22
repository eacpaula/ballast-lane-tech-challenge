import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import ErrorMessage from '../../components/ErrorMessage'
import LoadingState from '../../components/LoadingState'
import { useAuth } from '../auth/useAuth'
import { createPost } from './owned-posts.api'
import PostForm from './PostForm'
import { listAvailableCategories } from './public-posts.api'
import type { AvailableCategory, PostEditorDraft } from './post.types'

const initialDraft: PostEditorDraft = {
  categoryId: 0,
  title: '',
  summary: '',
  content: '',
  tags: '',
  publishDate: '',
  expireDate: '',
  isPublic: true,
  isAvailable: true,
}

export default function CreatePostPage() {
  const navigate = useNavigate()
  const { user } = useAuth()
  const [categories, setCategories] = useState<AvailableCategory[] | null>(null)
  const [draft, setDraft] = useState<PostEditorDraft>(initialDraft)
  const [error, setError] = useState<string | null>(null)
  const [isSubmitting, setIsSubmitting] = useState(false)

  useEffect(() => {
    void listAvailableCategories()
      .then(setCategories)
      .catch((caught) =>
        setError(caught instanceof Error ? caught.message : 'Unable to load categories.'),
      )
  }, [])

  async function handleSubmit() {
    if (!user?.token) {
      setError('You must be logged in to create a post.')
      return
    }

    setError(null)
    setIsSubmitting(true)

    try {
      await createPost(draft, user.token)
      navigate('/my-posts')
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : 'Unable to create post.')
    } finally {
      setIsSubmitting(false)
    }
  }

  if (error && !categories) {
    return <ErrorMessage detail={error} />
  }

  if (!categories) {
    return <LoadingState message="Loading categories for your post…" />
  }

  return (
    <section className="post-editor-page">
      <div className="post-editor-page-head">
        <div className="space-y-stack-sm">
          <p className="app-kicker">Author workspace</p>
          <h1 className="post-editor-page-title">Create New Post</h1>
          <p className="post-editor-page-copy">
            Draft your technical masterpiece for the engineering community.
          </p>
        </div>
      </div>

      <PostForm
        value={draft}
        categories={categories}
        isSubmitting={isSubmitting}
        submitLabel="Save Post"
        error={error}
        mode="create"
        onChange={setDraft}
        onSubmit={handleSubmit}
      />
    </section>
  )
}
