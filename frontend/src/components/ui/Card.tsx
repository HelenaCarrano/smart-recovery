import { clsx } from 'clsx'
import type { ReactNode } from 'react'

interface CardProps {
  children: ReactNode
  className?: string
}

export function Card({ children, className }: CardProps) {
  return <div className={clsx('rounded-xl border border-slate-200 bg-white p-5 shadow-sm', className)}>{children}</div>
}
