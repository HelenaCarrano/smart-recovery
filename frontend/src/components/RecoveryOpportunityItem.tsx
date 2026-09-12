import { RecoveryActionBadge } from '@/components/RecoveryActionBadge'
import { Card } from '@/components/ui/Card'
import { ScoreIndicator } from '@/components/ui/ScoreIndicator'
import { declineReasonLabel } from '@/lib/labels'
import { formatCurrency, formatDateTime } from '@/lib/format'
import type { RecoveryOpportunity } from '@/types/recovery'
import { clsx } from 'clsx'

interface RecoveryOpportunityItemProps {
  opportunity: RecoveryOpportunity
  highlight?: boolean
}

export function RecoveryOpportunityItem({ opportunity, highlight = false }: RecoveryOpportunityItemProps) {
  return (
    <Card
      className={clsx(
        'flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between',
        highlight && 'border-brand-300 ring-1 ring-brand-100',
      )}
    >
      <div className="min-w-0 flex-1">
        <div className="flex flex-wrap items-center gap-2">
          <p className="truncate font-medium text-slate-900">{opportunity.customerName}</p>
          {highlight && (
            <span className="rounded-full bg-brand-50 px-2 py-0.5 text-[11px] font-medium text-brand-700">
              Maior impacto financeiro
            </span>
          )}
        </div>
        <p className="mt-0.5 text-sm text-slate-500">
          {declineReasonLabel(opportunity.declineReason)} · Analisado em {formatDateTime(opportunity.analyzedAt)}
        </p>
        {opportunity.scheduledRetryAt && (
          <p className="mt-0.5 text-xs text-slate-400">
            Próxima tentativa agendada: {formatDateTime(opportunity.scheduledRetryAt)}
          </p>
        )}
      </div>

      <div className="flex shrink-0 flex-wrap items-center gap-6">
        <div className="text-right">
          <p className="text-xs text-slate-400">Valor em risco</p>
          <p className="font-semibold text-slate-900">{formatCurrency(opportunity.amount)}</p>
        </div>
        <ScoreIndicator score={opportunity.recoveryScore} />
        <RecoveryActionBadge action={opportunity.recommendedAction} />
      </div>
    </Card>
  )
}
