import { useState } from 'react'
import Button from './Button'

type Props = {
  label: string
  confirmLabel?: string
  onConfirm: () => void | Promise<void>
  disabled?: boolean
}

export default function ConfirmAction({
  label,
  confirmLabel = 'Confirm removal',
  onConfirm,
  disabled = false,
}: Props) {
  const [armed, setArmed] = useState(false)

  if (armed) {
    return (
      <div className="inline-flex items-center gap-3">
        <Button type="button" variant="danger" onClick={() => void onConfirm()} disabled={disabled}>
          {confirmLabel}
        </Button>
        <Button type="button" variant="ghost" onClick={() => setArmed(false)}>
          Cancel
        </Button>
      </div>
    )
  }

  return (
    <Button type="button" variant="danger" onClick={() => setArmed(true)} disabled={disabled}>
      {label}
    </Button>
  )
}
