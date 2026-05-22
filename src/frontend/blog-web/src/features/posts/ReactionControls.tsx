import { useMemo, useState } from 'react'
import ErrorMessage from '../../components/ErrorMessage'
import { getVisitorId } from '../../lib/session'
import { reactToPost } from './reactions.api'
import type { ReactionType } from './post.types'

type Props = {
  postId: number
  token?: string | null
}

export default function ReactionControls({ postId, token }: Props) {
  const [status, setStatus] = useState<string | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [isSubmitting, setIsSubmitting] = useState<ReactionType | null>(null)
  const visitorIdentifier = useMemo(() => (token ? null : getVisitorId()), [token])

  async function handleReaction(reactionType: ReactionType) {
    setError(null)
    setStatus(null)
    setIsSubmitting(reactionType)

    try {
      await reactToPost(postId, reactionType, visitorIdentifier, token)
      setStatus(`Stored ${reactionType} reaction.`)
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : 'Unable to save reaction.')
    } finally {
      setIsSubmitting(null)
    }
  }

  return (
    <div className="reaction-panel">
      <div className="reaction-panel-copy">
        <h2 className="reaction-panel-title">Was this article helpful?</h2>
        <p className="reaction-panel-subtitle">
          Anonymous visitors can react with a stable visitor identifier. Logged-in members react
          using their account identity.
        </p>
      </div>

      <div className="reaction-options">
        <button
          type="button"
          className="reaction-option-card"
          onClick={() => void handleReaction('like')}
          disabled={Boolean(isSubmitting)}
        >
          <svg viewBox="0 0 24 24" aria-hidden="true" className="reaction-option-icon">
            <path
              d="M10 21H6.8a1.8 1.8 0 0 1-1.8-1.8v-7.4A1.8 1.8 0 0 1 6.8 10H10v11Zm1.5 0V10.7l3.1-6.1a1.6 1.6 0 0 1 2.9 1l-.5 4.4h3.2a1.8 1.8 0 0 1 1.8 2l-1 7.2a2.3 2.3 0 0 1-2.3 1.8H11.5Z"
              fill="none"
              stroke="currentColor"
              strokeWidth="1.8"
              strokeLinecap="round"
              strokeLinejoin="round"
            />
          </svg>
          <span className="reaction-option-label">
            {isSubmitting === 'like' ? 'Saving…' : 'Interesting'}
          </span>
        </button>
        <button
          type="button"
          className="reaction-option-card"
          onClick={() => void handleReaction('dislike')}
          disabled={Boolean(isSubmitting)}
        >
          <svg viewBox="0 0 24 24" aria-hidden="true" className="reaction-option-icon">
            <path
              d="M14 3h3.2A1.8 1.8 0 0 1 19 4.8v7.4a1.8 1.8 0 0 1-1.8 1.8H14V3Zm-1.5 0v10.3l-3.1 6.1a1.6 1.6 0 0 1-2.9-1l.5-4.4H3.8a1.8 1.8 0 0 1-1.8-2l1-7.2A2.3 2.3 0 0 1 5.3 3h7.2Z"
              fill="none"
              stroke="currentColor"
              strokeWidth="1.8"
              strokeLinecap="round"
              strokeLinejoin="round"
            />
          </svg>
          <span className="reaction-option-label">
            {isSubmitting === 'dislike' ? 'Saving…' : 'Not for me'}
          </span>
        </button>
      </div>

      {status ? <p className="reaction-status-line">{status}</p> : null}
      {!status && !error ? (
        <p className="reaction-status-line">
          Reactions are stored anonymously or linked to your signed-in account.
        </p>
      ) : null}
      {error ? <ErrorMessage title="Reaction failed" detail={error} /> : null}
    </div>
  )
}
