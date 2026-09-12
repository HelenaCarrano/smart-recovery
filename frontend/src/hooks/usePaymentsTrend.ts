import { getPaymentsTrend } from '@/services/dashboardService'
import { useAsyncData } from './useAsyncData'

export function usePaymentsTrend(days = 90) {
  return useAsyncData(() => getPaymentsTrend(days), [days])
}
