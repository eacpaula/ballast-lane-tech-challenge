type Props = {
  title?: string
  detail: string
}

export default function ErrorMessage({ title = 'Unable to complete request', detail }: Props) {
  return (
    <div className="error-banner" role="alert">
      <p className="font-display text-lg font-semibold text-red-900">{title}</p>
      <p className="text-sm text-red-800">{detail}</p>
    </div>
  )
}
