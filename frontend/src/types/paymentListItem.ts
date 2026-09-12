import type { RecoveryAction } from './recovery'
import type { DeclineReason, Payment, PaymentStatus } from './payment'

/**
 * View model da tabela de Pagamentos: Payment + nome do cliente (via /api/customers)
 * + Recovery Score/ação (via /api/recovery/payments/{id}, só existe para recusados).
 * Nenhum campo aqui é inventado — recoveryScore/recommendedAction ficam null quando
 * a API não retorna uma análise para aquele pagamento.
 */
export interface PaymentListItem {
  id: string
  customerId: string
  customerName: string
  amount: number
  status: PaymentStatus
  declineReason: DeclineReason | null
  createdAt: string
  recoveryScore: number | null
  recommendedAction: RecoveryAction | null
}

export function toPaymentListItem(
  payment: Payment,
  customerName: string,
  recovery: { recoveryScore: number; recommendedAction: RecoveryAction } | null,
): PaymentListItem {
  return {
    id: payment.id,
    customerId: payment.customerId,
    customerName,
    amount: payment.amount,
    status: payment.status,
    declineReason: payment.declineReason,
    createdAt: payment.createdAt,
    recoveryScore: recovery?.recoveryScore ?? null,
    recommendedAction: recovery?.recommendedAction ?? null,
  }
}
