import { api } from './api'
import type { DashboardSummary, DeclineReasonCount, PaymentTrendPoint } from '@/types/dashboard'

export async function getDashboardSummary(): Promise<DashboardSummary> {
  const { data } = await api.get<DashboardSummary>('/api/dashboard/summary')
  return data
}

export async function getDeclineReasonBreakdown(): Promise<DeclineReasonCount[]> {
  const { data } = await api.get<DeclineReasonCount[]>('/api/dashboard/decline-reasons')
  return data
}

export async function getPaymentsTrend(days = 90): Promise<PaymentTrendPoint[]> {
  const { data } = await api.get<PaymentTrendPoint[]>('/api/dashboard/trends', { params: { days } })
  return data
}
