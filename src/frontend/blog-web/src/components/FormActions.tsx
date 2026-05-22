import type { ReactNode } from 'react'

export default function FormActions({ children }: { children: ReactNode }) {
  return <div className="form-actions">{children}</div>
}
