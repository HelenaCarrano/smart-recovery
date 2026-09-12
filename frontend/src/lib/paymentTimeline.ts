import type { BadgeTone } from '@/components/ui/Badge'
import { declineReasonLabel, recoveryActionLabel } from './labels'
import type { Payment } from '@/types/payment'
import type { PaymentAttempt } from '@/types/paymentAttempt'
import type { RecoveryAnalysis } from '@/types/recovery'

export interface TimelineStep {
  id: string
  label: string
  timestamp: string | null
  tone: BadgeTone
}

/** Monta a linha do tempo real do pagamento a partir dos eventos que a API de fato registrou. */
export function buildPaymentTimeline(
  payment: Payment,
  attempts: PaymentAttempt[],
  analysis: RecoveryAnalysis | null,
): TimelineStep[] {
  const steps: TimelineStep[] = [
    { id: 'created', label: 'Cobrança criada', timestamp: payment.createdAt, tone: 'neutral' },
  ]

  const sortedAttempts = [...attempts].sort((a, b) => a.attemptedAt.localeCompare(b.attemptedAt))

  sortedAttempts.forEach((attempt, index) => {
    steps.push({
      id: `attempt-${attempt.id}`,
      label: `Tentativa #${index + 1}`,
      timestamp: attempt.attemptedAt,
      tone: 'neutral',
    })

    if (attempt.resultStatus === 'Approved') {
      const label = index > 0 ? 'Recuperado com sucesso ✓' : 'Aprovado'
      steps.push({ id: `result-${attempt.id}`, label, timestamp: attempt.attemptedAt, tone: 'approved' })
    } else if (attempt.resultStatus === 'Declined') {
      steps.push({
        id: `result-${attempt.id}`,
        label: `Recusado — ${declineReasonLabel(attempt.declineReason)}`,
        timestamp: attempt.attemptedAt,
        tone: 'declined',
      })
    }
  })

  if (analysis) {
    steps.push({ id: 'analysis', label: 'Análise de recuperação', timestamp: analysis.analyzedAt, tone: 'attention' })
    steps.push({
      id: 'action',
      label: recoveryActionLabel(analysis.recommendedAction),
      timestamp: analysis.executedAt,
      tone: 'attention',
    })
  }

  return steps
}
