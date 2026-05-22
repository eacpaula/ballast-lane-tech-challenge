export default function LoadingState({ message = 'Loading…' }: { message?: string }) {
  return (
    <div className="status-panel" role="status" aria-live="polite">
      <p className="app-kicker">Loading</p>
      <p className="text-base text-ink-soft">{message}</p>
    </div>
  )
}
