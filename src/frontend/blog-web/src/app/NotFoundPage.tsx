import EmptyState from '../components/EmptyState'

export default function NotFoundPage() {
  return (
    <EmptyState
      title="Page not found"
      message="The page you requested does not exist or is no longer available."
      actionLabel="Back to posts"
      actionTo="/"
    />
  )
}
