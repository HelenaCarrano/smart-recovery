import { getDashboardSummary } from '@/services/dashboardService'
import { useAsyncData } from './useAsyncData'

export function useDashboardSummary() {
  return useAsyncData(getDashboardSummary, [])
}
