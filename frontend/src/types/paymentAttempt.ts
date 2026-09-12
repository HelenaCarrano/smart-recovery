import type { DeclineReason, PaymentStatus } from './payment'

/** Espelha o JSON retornado por GET /api/payments/{id}/attempts. */
export interface PaymentAttempt {
  id: string
  attemptedAt: string
  resultStatus: PaymentStatus
  declineReason: DeclineReason | null
  notes: string | null
}
