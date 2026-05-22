import { useEffect, useMemo, useState } from 'react'
import { NavLink, useNavigate, useParams } from 'react-router-dom'
import ErrorMessage from '../../components/ErrorMessage'
import LoadingState from '../../components/LoadingState'
import { useAuth } from '../auth/useAuth'
import { getOwnedPost, updatePost } from './owned-posts.api'
import PostForm from './PostForm'
import { listAvailableCategories } from './public-posts.api'
import { parseTags } from './tags'
import type { AvailableCategory, OwnedPostDetail, PostEditorDraft } from './post.types'

export default function EditPostPage() {
  const { postId } = useParams()
  const navigate = useNavigate()
  const { user } = useAuth()
  const [post, setPost] = useState<OwnedPostDetail | null>(null)
  const [categories, setCategories] = useState<AvailableCategory[] | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [isSubmitting, setIsSubmitting] = useState(false)

  useEffect(() => {
    if (!user?.token || !postId) {
      return
    }

    let active = true
    void Promise.all([getOwnedPost(Number(postId), user.token), listAvailableCategories()])
      .then(([loadedPost, loadedCategories]) => {
        if (active) {
          setPost(loadedPost)
          setCategories(loadedCategories)
        }
      })
      .catch((caught) => {
        if (active) {
          setError(caught instanceof Error ? caught.message : 'Unable to load the post.')
        }
      })

    return () => {
      active = false
    }
  }, [postId, user?.token])

  const draft = useMemo<PostEditorDraft>(
    () => ({
      categoryId: post?.categoryId ?? 0,
      title: post?.title ?? '',
      summary: post?.summary ?? '',
      content: post?.content ?? '',
      tags: (post?.tags ?? []).join(', '),
      publishDate: '',
      expireDate: '',
      isPublic: post?.isPublic ?? true,
      isAvailable: post?.isAvailable ?? true,
    }),
    [post],
  )

  async function handleSubmit(value: PostEditorDraft) {
    if (!user?.token || !postId) {
      setError('You must be logged in to edit a post.')
      return
    }

    setError(null)
    setIsSubmitting(true)
    try {
      await updatePost(
        Number(postId),
        {
          title: value.title,
          summary: value.summary,
          content: value.content,
          tags: parseTags(value.tags),
        },
        user.token,
      )
      navigate('/my-posts')
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : 'Unable to update post.')
    } finally {
      setIsSubmitting(false)
    }
  }

  if (error && (!post || !categories)) {
    return <ErrorMessage detail={error} />
  }

  if (!post || !categories) {
    return <LoadingState message="Loading the selected post…" />
  }

  return (
    <section className="post-editor-page">
      <NavLink to="/my-posts" className="post-detail-back-link">
        Back to My Posts
      </NavLink>

      <div className="post-editor-page-head">
        <div className="space-y-stack-sm">
          <p className="app-kicker">Author workspace</p>
          <h1 className="post-editor-page-title">Edit Post</h1>
          <p className="post-editor-page-copy">
            Refine your technical publication before it goes back into the feed.
          </p>
        </div>
      </div>

      {error ? <ErrorMessage detail={error} /> : null}
      <EditPostForm
        key={post.id}
        initialValue={draft}
        categories={categories}
        isSubmitting={isSubmitting}
        onSubmit={handleSubmit}
      />
    </section>
  )
}

function EditPostForm({
  initialValue,
  categories,
  isSubmitting,
  onSubmit,
}: {
  initialValue: PostEditorDraft
  categories: AvailableCategory[]
  isSubmitting: boolean
  onSubmit: (value: PostEditorDraft) => void | Promise<void>
}) {
  const [draft, setDraft] = useState(initialValue)

  return (
    <PostForm
      value={draft}
      categories={categories}
      isSubmitting={isSubmitting}
      submitLabel="Save Post"
      mode="edit"
      disableCategorySelection
      onChange={setDraft}
      onSubmit={() => onSubmit(draft)}
    />
  )
}
