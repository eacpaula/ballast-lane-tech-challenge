import { useEffect, useState } from 'react'
import { NavLink, useParams } from 'react-router-dom'
import ErrorMessage from '../../components/ErrorMessage'
import LoadingState from '../../components/LoadingState'
import { useAuth } from '../auth/useAuth'
import ReactionControls from './ReactionControls'
import { getPublicPost } from './public-posts.api'
import type { PublicPostDetail } from './post.types'

const categoryKeywords = [
  { label: 'Engineering', terms: ['engineering', 'system', 'api', 'backend', 'frontend'] },
  { label: 'Architecture', terms: ['architecture', 'distributed', 'scale', 'scaling', 'service'] },
  { label: 'Product', terms: ['product', 'user', 'workflow', 'roadmap', 'experience'] },
]

function deriveCategoryLabel(post: PublicPostDetail) {
  const haystack = `${post.title} ${post.summary ?? ''} ${post.content}`.toLowerCase()

  return (
    categoryKeywords.find((candidate) =>
      candidate.terms.some((term) => haystack.includes(term)),
    )?.label ?? 'Engineering'
  )
}

function deriveTags(post: PublicPostDetail) {
  const keywordTags = [
    'distributed-systems',
    'architecture',
    'cloud-native',
    'scaling',
    'frontend',
    'backend',
    'product-engineering',
    'performance',
  ]

  const haystack = `${post.title} ${post.summary ?? ''} ${post.content}`.toLowerCase()
  const matchedTags = keywordTags.filter((tag) =>
    haystack.includes(tag.replace(/-/g, ' ')) || haystack.includes(tag.split('-')[0]),
  )

  if (matchedTags.length >= 3) {
    return matchedTags.slice(0, 4)
  }

  const fallbackTitleTags = post.title
    .toLowerCase()
    .split(/[^a-z0-9]+/)
    .filter((word) => word.length > 4)
    .slice(0, 4)
    .map((word) => word.replace(/\s+/g, '-'))

  return [...new Set([...matchedTags, ...fallbackTitleTags, 'engineering'])].slice(0, 4)
}

export default function PublicPostDetailPage() {
  const { postId } = useParams()
  const { user } = useAuth()
  const [post, setPost] = useState<PublicPostDetail | null>(null)
  const [error, setError] = useState<string | null>(null)
  const parsedPostId = Number(postId)
  const invalidPostId = !postId || Number.isNaN(parsedPostId) || parsedPostId <= 0

  useEffect(() => {
    if (invalidPostId) {
      return
    }

    let active = true
    void getPublicPost(parsedPostId)
      .then((response) => {
        if (active) {
          setPost(response)
        }
      })
      .catch((caught) => {
        if (active) {
          setError(caught instanceof Error ? caught.message : 'Unable to load post.')
        }
      })

    return () => {
      active = false
    }
  }, [invalidPostId, parsedPostId])

  if (invalidPostId) {
    return <ErrorMessage title="Unable to load post" detail="The requested post id is missing." />
  }

  if (error) {
    return <ErrorMessage title="Unable to load post" detail={error} />
  }

  if (!post) {
    return <LoadingState message="Loading post details…" />
  }

  const categoryLabel = deriveCategoryLabel(post)
  const tags = deriveTags(post)

  return (
    <section className="post-detail-layout">
      <NavLink to="/" className="post-detail-back-link">
        <svg viewBox="0 0 20 20" aria-hidden="true" className="post-detail-back-icon">
          <path
            d="M10.8 4.2a.75.75 0 0 1 0 1.06L7.06 9H16a.75.75 0 0 1 0 1.5H7.06l3.74 3.74a.75.75 0 1 1-1.06 1.06l-5-5a.75.75 0 0 1 0-1.06l5-5a.75.75 0 0 1 1.06 0Z"
            fill="currentColor"
          />
        </svg>
        Back to posts
      </NavLink>

      <article className="post-detail-article">
        <div className="post-detail-header">
          <span className="post-detail-category">{categoryLabel}</span>
          <div className="space-y-stack-md">
            <h1 className="post-detail-title">{post.title}</h1>
            <p className="post-detail-summary">
              {post.summary || 'No summary provided for this post.'}
            </p>
          </div>
        </div>

        <div className="post-detail-meta-row">
          <div className="post-detail-author">
            <img
              src="/logo.png"
              alt="Tech Blog Platform editorial avatar"
              className="post-detail-author-avatar"
            />
            <div className="post-detail-author-copy">
              <p className="post-detail-author-name">Tech Blog Editorial</p>
              <p className="post-detail-author-role">Platform Contributor</p>
            </div>
          </div>

          <div className="post-detail-meta-block">
            <span className="post-detail-meta-label">Published</span>
            <span className="post-detail-meta-value">May 21, 2026</span>
          </div>
        </div>

        <div className="post-detail-divider" />

        <div className="post-detail-image-wrap">
          <img
            src="/post-placeholder.svg"
            alt="Placeholder illustration for a technical post"
            className="post-detail-image"
          />
        </div>

        <div className="post-detail-divider" />

        <div className="article-body post-detail-body">
          {post.content.split(/\n+/).map((paragraph) => (
            <p key={paragraph}>{paragraph}</p>
          ))}
        </div>

        <div className="post-detail-divider" />

        <div className="post-detail-tags">
          {tags.map((tag) => (
            <span key={tag} className="post-detail-tag">
              #{tag}
            </span>
          ))}
        </div>
      </article>

      <ReactionControls postId={post.id} token={user?.token} />
    </section>
  )
}
