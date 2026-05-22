import type { InputHTMLAttributes } from 'react'

type TextInputProps = InputHTMLAttributes<HTMLInputElement> & {
  label: string
  error?: string | null
}

export default function TextInput({ label, error, id, className, ...props }: TextInputProps) {
  const inputId = id ?? props.name ?? label

  return (
    <label className="form-field" htmlFor={inputId}>
      <span className="field-label">{label}</span>
      <input
        {...props}
        id={inputId}
        className={['input-control', className].filter(Boolean).join(' ')}
      />
      {error ? <span className="field-error">{error}</span> : null}
    </label>
  )
}
