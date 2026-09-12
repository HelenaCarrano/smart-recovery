import { getSubscriptions } from '@/services/subscriptionService'
import type { SubscriptionStatus } from '@/types/subscription'
import { useAsyncData } from './useAsyncData'

/** GET /api/subscriptions já traz customerName e a periodicidade do plano resolvidos pelo backend. */
export function useSubscriptions(page: number, pageSize: number, status: SubscriptionStatus | undefined) {
  return useAsyncData(
    () => getSubscriptions({ page, pageSize, status }),
    [page, pageSize, status],
  )
}
