import { NavLink } from 'react-router-dom'
import Badge from '../../components/Badge'
import Card from '../../components/Card'
import type { PublicPostSummary } from './post.types'

export default function PostCard({ post }: { post: PublicPostSummary }) {
  return (
    <Card className="post-card">
      <Badge>Latest post</Badge>
      <div className="space-y-stack-sm">
        <h2 className="text-2xl font-display font-semibold tracking-headline text-navy">
          <NavLink to={`/posts/${post.id}`} className="text-current no-underline hover:text-brand">
            {post.title}
          </NavLink>
        </h2>
        <p className="post-card-summary">
          {post.summary || 'This post has no summary yet.'}
        </p>
      </div>
      <div className="post-card-footer">
        <span className="post-card-meta">Public article</span>
        <NavLink to={`/posts/${post.id}`} className="post-card-link">
          Read article
        </NavLink>
      </div>
    </Card>
  )
}
