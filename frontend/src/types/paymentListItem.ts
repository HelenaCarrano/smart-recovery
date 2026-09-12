import type { RecoveryAction } from './recovery'
import type { DeclineReason, PaymentStatus } from './payment'

/**
 * Espelha PaymentListItemDto de GET /api/payments — customerName já vem resolvido pelo backend.
 * recoveryScore/recommendedAction são enriquecidos à parte pelo hook (via GET /api/recovery/payments/{id},
 * só para os itens Declined da página atual) e ficam null quando não há análise.
 */
export interface PaymentListItem {
  id: string
  customerId: string
  customerName: string
  subscriptionId: string
  amount: number
  status: PaymentStatus
  declineReason: DeclineReason | null
  attemptCount: number
  scheduledRetryAt: string | null
  createdAt: string
  recoveryScore: number | null
  recommendedAction: RecoveryAction | null
}
