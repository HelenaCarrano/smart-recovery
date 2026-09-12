import { clsx } from 'clsx'
import type { ReactNode } from 'react'

/**
 * Tons semânticos compartilhados por toda a aplicação: o mesmo tom sempre
 * representa o mesmo significado (aprovado, recusado, pendente, atenção),
 * não importa em qual componente ele apareça.
 */
export type BadgeTone = 'approved' | 'declined' | 'pending' | 'attention' | 'neutral'

const TONE_CLASSES: Record<BadgeTone, string> = {
  approved: 'bg-status-approved-bg text-status-approved border-status-approved-border',
  declined: 'bg-status-declined-bg text-status-declined border-status-declined-border',
  pending: 'bg-status-pending-bg text-status-pending border-status-pending-border',
  attention: 'bg-status-attention-bg text-status-attention border-status-attention-border',
  neutral: 'bg-slate-100 text-slate-600 border-slate-200',
}

const DOT_CLASSES: Record<BadgeTone, string> = {
  approved: 'bg-status-approved',
  declined: 'bg-status-declined',
  pending: 'bg-status-pending',
  attention: 'bg-status-attention',
  neutral: 'bg-slate-400',
}

interface BadgeProps {
  tone?: BadgeTone
  dot?: boolean
  children: ReactNode
  className?: string
}

export function Badge({ tone = 'neutral', dot = false, children, className }: BadgeProps) {
  return (
    <span
      className={clsx(
        'inline-flex items-center gap-1.5 whitespace-nowrap rounded-full border px-2.5 py-0.5 text-xs font-medium',
        TONE_CLASSES[tone],
        className,
      )}
    >
      {dot && <span className={clsx('h-1.5 w-1.5 shrink-0 rounded-full', DOT_CLASSES[tone])} />}
      {children}
    </span>
  )
}
