import { getDeclineReasonBreakdown } from '@/services/dashboardService'
import { useAsyncData } from './useAsyncData'

export function useDeclineReasonBreakdown() {
  return useAsyncData(getDeclineReasonBreakdown, [])
}
