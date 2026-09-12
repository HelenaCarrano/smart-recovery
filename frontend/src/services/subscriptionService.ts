import { api } from './api'
import type { PagedResult } from '@/types/pagedResult'
import type { Subscription, SubscriptionStatus } from '@/types/subscription'
import type { SubscriptionListItem } from '@/types/subscriptionListItem'
import type { SubscriptionsSummary } from '@/types/subscriptionsSummary'

export interface SubscriptionQueryParams {
  page: number
  pageSize: number
  status?: SubscriptionStatus
}

export async function getSubscriptions(params: SubscriptionQueryParams): Promise<PagedResult<SubscriptionListItem>> {
  const { data } = await api.get<PagedResult<SubscriptionListItem>>('/api/subscriptions', { params })
  return data
}

export async function getSubscriptionById(id: string): Promise<Subscription> {
  const { data } = await api.get<Subscription>(`/api/subscriptions/${id}`)
  return data
}

export async function getSubscriptionsByCustomer(customerId: string): Promise<Subscription[]> {
  const { data } = await api.get<Subscription[]>(`/api/subscriptions/by-customer/${customerId}`)
  return data
}

export async function getSubscriptionsSummary(): Promise<SubscriptionsSummary> {
  const { data } = await api.get<SubscriptionsSummary>('/api/subscriptions/summary')
  return data
}
