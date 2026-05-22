import { NavLink } from 'react-router-dom'
import Button from './Button'

type Props = {
  title: string
  message: string
  actionLabel?: string
  actionTo?: string
}

export default function EmptyState({ title, message, actionLabel, actionTo }: Props) {
  return (
    <section className="status-panel">
      <p className="app-kicker">Empty state</p>
      <h1 className="text-3xl font-display font-semibold tracking-headline">{title}</h1>
      <p className="prose-lead">{message}</p>
      {actionLabel && actionTo ? (
        <div>
          <Button asChild>
            <NavLink to={actionTo}>{actionLabel}</NavLink>
          </Button>
        </div>
      ) : null}
    </section>
  )
}
