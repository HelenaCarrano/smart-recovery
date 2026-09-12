import { RecoveryActionBadge } from '@/components/RecoveryActionBadge'
import { Badge } from '@/components/ui/Badge'
import { Card } from '@/components/ui/Card'
import { formatDateTime } from '@/lib/format'
import { recoveryActionLabel } from '@/lib/labels'
import { explainRecommendedAction } from '@/lib/recoveryExplanations'
import type { DeclineReason, PaymentStatus } from '@/types/payment'
import type { RecoveryAction } from '@/types/recovery'

interface RecommendedActionCardProps {
  action: RecoveryAction
  declineReason: DeclineReason | null
  executedAt: string | null
  paymentStatus: PaymentStatus
}

export function RecommendedActionCard({ action, declineReason, executedAt, paymentStatus }: RecommendedActionCardProps) {
  if (paymentStatus === 'Approved') {
    return (
      <Card>
        <h2 className="mb-3 text-sm font-semibold text-slate-900">Ação recomendada</h2>
        <Badge tone="approved">Recuperado com sucesso</Badge>
        <p className="mt-3 text-sm text-slate-600">
          A estratégia recomendada ({recoveryActionLabel(action).toLowerCase()}) surtiu efeito: a cobrança acabou sendo aprovada.
        </p>
        <p className="mt-2 text-xs text-slate-400">
          {executedAt ? `Estratégia executada em ${formatDateTime(executedAt)}.` : 'Recuperada antes da execução automática da estratégia.'}
        </p>
      </Card>
    )
  }

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
