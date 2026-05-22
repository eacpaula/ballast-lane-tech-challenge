import type { AnchorHTMLAttributes, ButtonHTMLAttributes, ReactNode } from 'react'
import { cloneElement, isValidElement } from 'react'

type Variant = 'primary' | 'secondary' | 'ghost' | 'danger'

type CommonProps = {
  asChild?: boolean
  children: ReactNode
  className?: string
  variant?: Variant
}

type ButtonProps = CommonProps &
  Omit<ButtonHTMLAttributes<HTMLButtonElement>, 'className' | 'children'>

const variants: Record<Variant, string> = {
  primary: 'button-primary',
  secondary: 'button-secondary',
  ghost: 'button-ghost',
  danger: 'button-danger',
}

export default function Button({
  asChild = false,
  children,
  className,
  variant = 'primary',
  ...props
}: ButtonProps) {
  const classes = ['button-base', variants[variant], className].filter(Boolean).join(' ')

  if (asChild && isValidElement<AnchorHTMLAttributes<HTMLAnchorElement>>(children)) {
    return cloneElement(children, {
      className: [classes, children.props.className].filter(Boolean).join(' '),
    })
  }

  return (
    <button {...props} className={classes}>
      {children}
    </button>
  )
}
