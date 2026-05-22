import type { HTMLAttributes, ReactNode } from 'react'

type CardProps = HTMLAttributes<HTMLDivElement> & {
  children: ReactNode
}

export default function Card({ children, className, ...props }: CardProps) {
  return (
    <div {...props} className={['surface-card', className].filter(Boolean).join(' ')}>
      {children}
    </div>
  )
}
