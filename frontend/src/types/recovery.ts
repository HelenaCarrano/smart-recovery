import type { DeclineReason } from './payment'

export type RecoveryAction =
  | 'RetryIn2Hours'
  | 'RetryIn24Hours'
  | 'RetryIn72Hours'
  | 'RequestPaymentMethodUpdate'
  | 'CancelSubscription'
  | 'ManualReview'

/**
 * Espelha o JSON retornado por GET /api/recovery/pending e GET /api/recovery/payments/{id}.
 * baseScore/historyAdjustment/recoveryTrackRecordBonus/recentDeclinesAdjustment/recentAttemptsAdjustment
 * já vêm com o sinal correto — recoveryScore é a soma de todos eles (clampada em 0-100).
 * O frontend não recalcula nada disso; só apresenta o que a API já decidiu.
 */
export interface RecoveryAnalysis {
  id: string
  paymentId: string
  recoveryScore: number
  recommendedAction: RecoveryAction
  analyzedAt: string
  executedAt: string | null
  totalPayments: number
  successfulPayments: number
  previouslyRecoveredPayments: number
  recentDeclines: number
  recentAttempts: number
  baseScore: number
  historyAdjustment: number
  recoveryTrackRecordBonus: number
  recentDeclinesAdjustment: number
  recentAttemptsAdjustment: number
}

/**
 * View model montado no frontend combinando RecoveryAnalysis + Payment + Customer
 * (a API não tem um endpoint único que já traga tudo isso junto).
 */
export interface RecoveryOpportunity {
  id: string
  paymentId: string
  customerId: string
  customerName: string
  amount: number
  declineReason: DeclineReason | null
  recoveryScore: number
  recommendedAction: RecoveryAction
  scheduledRetryAt: string | null
  analyzedAt: string
}
