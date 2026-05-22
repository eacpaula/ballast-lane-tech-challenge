export default function FieldMessage({
  message,
  tone = 'neutral',
}: {
  message?: string | null
  tone?: 'neutral' | 'error'
}) {
  if (!message) {
    return null
  }

  return <p className={tone === 'error' ? 'field-error' : 'field-help'}>{message}</p>
}
