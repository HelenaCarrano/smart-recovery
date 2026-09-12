import { api } from './api'
import type { DashboardSummary } from '@/types/dashboard'

export async function getDashboardSummary(): Promise<DashboardSummary> {
  const { data } = await api.get<DashboardSummary>('/api/dashboard/summary')
  return data
}
