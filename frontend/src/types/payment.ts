export type PaymentStatus = 'Pending' | 'Approved' | 'Declined' | 'Cancelled' | 'Refunded'

export type DeclineReason =
  | 'InsufficientFunds'
  | 'ExpiredCard'
  | 'InvalidCard'
  | 'TemporaryError'
  | 'BlockedCard'
  | 'Unknown'

/** Espelha o JSON retornado por GET /api/payments/{id} e GET /api/payments/by-customer/{id}. */
export interface Payment {
  id: string
  customerId: string
  subscriptionId: string
  amount: number
  status: PaymentStatus
  declineReason: DeclineReason | null
  attemptCount: number
  scheduledRetryAt: string | null
  createdAt: string
}
