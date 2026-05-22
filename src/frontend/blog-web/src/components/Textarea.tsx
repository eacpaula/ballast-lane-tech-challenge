import type { TextareaHTMLAttributes } from 'react'

type TextareaProps = TextareaHTMLAttributes<HTMLTextAreaElement> & {
  label: string
  error?: string | null
}

export default function Textarea({ label, error, id, className, ...props }: TextareaProps) {
  const inputId = id ?? props.name ?? label

  return (
    <label className="form-field" htmlFor={inputId}>
      <span className="field-label">{label}</span>
      <textarea
        {...props}
        id={inputId}
        className={['input-control input-textarea', className].filter(Boolean).join(' ')}
      />
      {error ? <span className="field-error">{error}</span> : null}
    </label>
  )
}
