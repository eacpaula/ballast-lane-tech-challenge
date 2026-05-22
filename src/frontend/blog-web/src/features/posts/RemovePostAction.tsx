import ConfirmAction from '../../components/ConfirmAction'

export default function RemovePostAction({
  onRemove,
  disabled = false,
}: {
  onRemove: () => void | Promise<void>
  disabled?: boolean
}) {
  return (
    <ConfirmAction
      label="Remove post"
      confirmLabel="Confirm remove"
      onConfirm={onRemove}
      disabled={disabled}
    />
  )
}
