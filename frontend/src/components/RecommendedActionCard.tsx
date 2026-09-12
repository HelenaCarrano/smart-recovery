import { RecoveryActionBadge } from '@/components/RecoveryActionBadge'
import { Card } from '@/components/ui/Card'
import { formatDateTime } from '@/lib/format'
import { explainRecommendedAction } from '@/lib/recoveryExplanations'
import type { DeclineReason } from '@/types/payment'
import type { RecoveryAction } from '@/types/recovery'

interface RecommendedActionCardProps {
  action: RecoveryAction
  declineReason: DeclineReason | null
  executedAt: string | null
}

export function RecommendedActionCard({ action, declineReason, executedAt }: RecommendedActionCardProps) {
  return (
    <Card>
      <h2 className="mb-3 text-sm font-semibold text-slate-900">Ação recomendada</h2>
      <RecoveryActionBadge action={action} />
      <p className="mt-3 text-sm text-slate-600">{explainRecommendedAction(action, declineReason)}</p>
      <p className="mt-2 text-xs text-slate-400">
        {executedAt
          ? `Executada automaticamente em ${formatDateTime(executedAt)}.`
          : 'Aguardando execução — depende de uma ação do cliente ou de um analista.'}
      </p>
    </Card>
  )
}
