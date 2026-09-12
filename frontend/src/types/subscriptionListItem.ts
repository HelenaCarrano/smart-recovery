import type { PlanPeriodicity } from './plan'
import type { SubscriptionStatus } from './subscription'

/** Espelha SubscriptionListItemDto de GET /api/subscriptions — customerName e periodicity já resolvidos pelo backend. */
export interface SubscriptionListItem {
  id: string
  customerId: string
  customerName: string
  planName: string
  planPrice: number
  periodicity: PlanPeriodicity
  status: SubscriptionStatus
  nextBillingDate: string
}
