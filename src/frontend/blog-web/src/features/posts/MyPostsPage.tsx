import { useEffect, useState } from 'react'
import { NavLink } from 'react-router-dom'
import Badge from '../../components/Badge'
import Button from '../../components/Button'
import Card from '../../components/Card'
import EmptyState from '../../components/EmptyState'
import ErrorMessage from '../../components/ErrorMessage'
import LoadingState from '../../components/LoadingState'
import { useAuth } from '../auth/useAuth'
import { listOwnedPosts, removePost } from './owned-posts.api'
import RemovePostAction from './RemovePostAction'
import type { OwnedPostSummary } from './post.types'

export default function MyPostsPage() {
  const { user } = useAuth()
  const [posts, setPosts] = useState<OwnedPostSummary[] | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [busyPostId, setBusyPostId] = useState<number | null>(null)

  useEffect(() => {
    if (!user?.token) {
      return
    }

    let active = true
    void listOwnedPosts(user.token)
      .then((response) => {
        if (active) {
          setPosts(response)
        }
      })
      .catch((caught) => {
        if (active) {
          setError(caught instanceof Error ? caught.message : 'Unable to load your posts.')
        }
      })

    return () => {
      active = false
    }
  }, [user?.token])

  async function handleRemove(postId: number) {
    if (!user?.token) {
      return
    }

    setBusyPostId(postId)
    setError(null)

    try {
      await removePost(postId, user.token)
      setPosts((current) => current?.filter((post) => post.id !== postId) ?? [])
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : 'Unable to remove post.')
    } finally {
      setBusyPostId(null)
    }
  }

  if (error && !posts) {
    return <ErrorMessage detail={error} />
  }

  if (!posts) {
    return <LoadingState message="Loading your posts…" />
  }

  if (posts.length === 0) {
    return (
      <EmptyState
        title="You have not published anything yet"
        message="Create your first post to start building your author workspace."
        actionLabel="Create a post"
        actionTo="/my-posts/new"
      />
    )
  }

  return (
    <section className="content-stack">
      <div className="flex flex-wrap items-end justify-between gap-4">
        <div className="space-y-stack-sm">
          <p className="app-kicker">Author workspace</p>
          <h1 className="page-title">My Posts</h1>
        </div>
        <Button asChild>
          <NavLink to="/my-posts/new">Create post</NavLink>
        </Button>
      </div>

      {error ? <ErrorMessage detail={error} /> : null}

      <section className="post-grid">
        {posts.map((post) => (
          <Card key={post.id} className="post-card">
            <div className="space-y-stack-sm">
              <div className="flex flex-wrap gap-3">
                <Badge>{post.isPublic ? 'Public' : 'Private'}</Badge>
                <Badge>{post.isAvailable ? 'Available' : 'Unavailable'}</Badge>
              </div>
              <h2 className="text-2xl font-display font-semibold tracking-headline">{post.title}</h2>
              <p className="text-base leading-[1.625] text-ink-soft">
                {post.summary || 'No summary has been added yet.'}
              </p>
            </div>
            <div className="flex flex-wrap gap-3">
              <Button asChild variant="secondary">
                <NavLink to={`/my-posts/${post.id}/edit`}>Edit</NavLink>
              </Button>
              <RemovePostAction
                disabled={busyPostId === post.id}
                onRemove={() => handleRemove(post.id)}
              />
            </div>
          </Card>
        ))}
      </section>
    </section>
  )
}
