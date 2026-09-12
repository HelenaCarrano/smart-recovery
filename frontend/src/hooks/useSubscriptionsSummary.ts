import { getSubscriptionsSummary } from '@/services/subscriptionService'
import { useAsyncData } from './useAsyncData'

export function useSubscriptionsSummary() {
  return useAsyncData(getSubscriptionsSummary, [])
}
