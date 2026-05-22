import { useEffect, useState } from 'react'
import { NavLink, useSearchParams } from 'react-router-dom'
import EmptyState from '../../components/EmptyState'
import ErrorMessage from '../../components/ErrorMessage'
import LoadingState from '../../components/LoadingState'
import PostCard from './PostCard'
import { listPublicPosts } from './public-posts.api'
import type { PublicPostSummary } from './post.types'
import { useAuth } from '../auth/useAuth'

export default function PublicPostListPage() {
  const { isAuthenticated } = useAuth()
  const [searchParams] = useSearchParams()
  const [posts, setPosts] = useState<PublicPostSummary[] | null>(null)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    let active = true
    void listPublicPosts()
      .then((response) => {
        if (active) {
          setPosts(response)
        }
      })
      .catch((caught) => {
        if (active) {
          setError(caught instanceof Error ? caught.message : 'Unable to load posts.')
        }
      })

    return () => {
      active = false
    }
  }, [])

  if (error) {
    return <ErrorMessage detail={error} />
  }

  if (!posts) {
    return <LoadingState message="Loading public posts…" />
  }

  if (posts.length === 0) {
    return (
      <EmptyState
        title="No public posts yet"
        message="The platform is ready, but there are no public posts available right now."
      />
    )
  }

  const query = searchParams.get('q')?.trim().toLowerCase() ?? ''
  const filteredPosts = query
    ? posts.filter((post) => {
        const haystack = `${post.title} ${post.summary ?? ''}`.toLowerCase()
        return haystack.includes(query)
      })
    : posts

  if (filteredPosts.length === 0) {
    return (
      <section className="content-stack">
        <section className="hero-panel hero-panel-centered">
          <span className="eyebrow">No results found</span>
          <h1 className="page-title">No posts matched “{searchParams.get('q')}”.</h1>
          <p className="prose-lead">
            Try a different keyword from the search field above to explore the latest writing.
          </p>
        </section>
      </section>
    )
  }

  const featuredPost = filteredPosts[0]
  const latestPosts = filteredPosts.slice(1)

  return (
    <section className="content-stack">
      <section className="hero-panel hero-panel-centered">
        <span className="eyebrow">New edition available</span>
        <h1 className="page-title hero-title">Technical Posts for Product-Minded Engineers</h1>
        <p className="prose-lead hero-copy">
          Deep dives into architecture, frontend precision, and practical product engineering.
          Read public posts, react instantly, and jump into the platform when you are ready to publish.
        </p>
        <div className="hero-actions">
          <a href="#featured-post" className="button-base button-primary hero-action-button">
            Browse Latest Posts
          </a>
          <NavLink
            to={isAuthenticated ? '/my-posts/new' : '/register'}
            className="button-base button-secondary hero-action-button"
          >
            {isAuthenticated ? 'Write Your Next Post' : 'Create an Account'}
          </NavLink>
        </div>
      </section>

      <section id="featured-post" className="featured-post-card">
        <div className="featured-post-media">
          <img
            src="/post-placeholder.svg"
            alt="Placeholder illustration for a technical post"
            className="featured-post-image"
          />
        </div>
        <div className="featured-post-content">
          <span className="eyebrow">Featured article</span>
          <div className="space-y-stack-sm">
            <h2 className="featured-post-title">
              <NavLink
                to={`/posts/${featuredPost.id}`}
                className="text-current no-underline hover:text-brand"
              >
                {featuredPost.title}
              </NavLink>
            </h2>
            <p className="featured-post-summary">
              {featuredPost.summary || 'This featured post has no summary yet, but it is ready to read.'}
            </p>
          </div>
          <div className="meta-row">
            <span>Public post</span>
            <span>Live on the platform</span>
            {query ? <span>Filtered by “{searchParams.get('q')}”</span> : null}
          </div>
          <div>
            <NavLink to={`/posts/${featuredPost.id}`} className="button-base button-primary hero-action-button">
              Read Featured Post
            </NavLink>
          </div>
        </div>
      </section>

      {latestPosts.length > 0 ? (
        <section className="section-stack">
          <div className="section-heading">
            <div className="space-y-2">
              <span className="eyebrow">Latest posts</span>
              <h2 className="text-3xl font-display font-semibold tracking-headline text-navy">
                More technical writing from the feed
              </h2>
            </div>
            <p className="section-copy">
              Explore the newest public entries across architecture, APIs, product workflows, and implementation details.
            </p>
          </div>

          <section className="post-grid post-grid-compact">
            {latestPosts.map((post) => (
              <PostCard key={post.id} post={post} />
            ))}
          </section>
        </section>
      ) : null}

      <section className="home-promo-panel">
        <div className="home-promo-copy">
          <h2 className="home-promo-title">Built for the modern stack.</h2>
          <ul className="home-promo-list">
            <li>Optimized for fast reading with clear technical writing.</li>
            <li>Structured for public posts, account flows, and editorial growth.</li>
            <li>A focused platform for product-minded engineering teams.</li>
            <li>Designed to scale from interview MVP to real product surface.</li>
          </ul>
        </div>

        <div className="home-promo-code">
          <div className="home-promo-code-window">
            <div className="home-promo-code-dots" aria-hidden="true">
              <span className="home-promo-code-dot home-promo-code-dot-red" />
              <span className="home-promo-code-dot home-promo-code-dot-yellow" />
              <span className="home-promo-code-dot home-promo-code-dot-green" />
            </div>
            <pre className="home-promo-code-block">
              <code>{`const engineer = {
  mindset: 'product',
  priority: 'clarity',
  action: () => subscribe(),
};`}</code>
            </pre>
          </div>
        </div>
      </section>
    </section>
  )
}
