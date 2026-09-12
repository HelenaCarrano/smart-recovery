export type SubscriptionStatus = 'Active' | 'Cancelled' | 'Paused'

/** Espelha o JSON retornado por GET /api/subscriptions/{id} e /api/subscriptions/by-customer/{id}. */
export interface Subscription {
  id: string
  customerId: string
  planId: string
  planName: string
  planPrice: number
  startDate: string
  nextBillingDate: string
  endDate: string | null
  status: SubscriptionStatus
}
