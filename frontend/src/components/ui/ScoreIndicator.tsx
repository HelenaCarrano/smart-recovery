import type { BadgeTone } from '@/components/ui/Badge'
import { clsx } from 'clsx'

/**
 * Mesmas faixas usadas pelo RecoveryDecisionEngine no backend (score >= 80 → ação
 * de retry rápido, 50-79 → retry mais espaçado, 30-49 → precisa de ação do cliente,
 * < 30 → cancelamento). Reaproveitar essas faixas aqui é só uma leitura visual da
 * mesma regra já aplicada pelo backend — não é um novo cálculo de negócio.
 */
function toneForScore(score: number): BadgeTone {
  if (score >= 80) return 'approved'
  if (score >= 50) return 'pending'
  if (score >= 30) return 'attention'
  return 'declined'
}

const TEXT_COLOR: Record<BadgeTone, string> = {
  approved: 'text-status-approved',
  declined: 'text-status-declined',
  pending: 'text-status-pending',
  attention: 'text-status-attention',
  neutral: 'text-slate-600',
}

const BAR_COLOR: Record<BadgeTone, string> = {
  approved: 'bg-status-approved',
  declined: 'bg-status-declined',
  pending: 'bg-status-pending',
  attention: 'bg-status-attention',
  neutral: 'bg-slate-400',
}

interface ScoreIndicatorProps {
  score: number
  size?: 'sm' | 'lg'
}

export function ScoreIndicator({ score, size = 'sm' }: ScoreIndicatorProps) {
  const tone = toneForScore(score)
  const clamped = Math.max(0, Math.min(100, score))

  return (
    <div className={clsx('flex items-center gap-2', size === 'lg' && 'w-full flex-col items-start gap-2')}>
      <span className={clsx('font-semibold tabular-nums', size === 'lg' ? 'text-3xl' : 'text-sm', TEXT_COLOR[tone])}>
        {score}
        <span className="text-[0.6em] font-normal text-slate-400">/100</span>
      </span>
      <div className={clsx('h-1.5 overflow-hidden rounded-full bg-slate-100', size === 'lg' ? 'w-full' : 'w-16')}>
        <div className={clsx('h-full rounded-full', BAR_COLOR[tone])} style={{ width: `${clamped}%` }} />
      </div>
    </div>
  )
}
