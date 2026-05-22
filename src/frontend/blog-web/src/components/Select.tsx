import type { ReactNode, SelectHTMLAttributes } from 'react'

type SelectProps = SelectHTMLAttributes<HTMLSelectElement> & {
  label: string
  error?: string | null
  children: ReactNode
}

export default function Select({ label, error, id, className, children, ...props }: SelectProps) {
  const inputId = id ?? props.name ?? label

  return (
    <label className="form-field" htmlFor={inputId}>
      <span className="field-label">{label}</span>
      <select
        {...props}
        id={inputId}
        className={['input-control', className].filter(Boolean).join(' ')}
      >
        {children}
      </select>
      {error ? <span className="field-error">{error}</span> : null}
    </label>
  )
}
