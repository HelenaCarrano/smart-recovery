import { api } from './api'
import type { Subscription } from '@/types/subscription'

export async function getSubscriptionById(id: string): Promise<Subscription> {
  const { data } = await api.get<Subscription>(`/api/subscriptions/${id}`)
  return data
}

export async function getSubscriptionsByCustomer(customerId: string): Promise<Subscription[]> {
  const { data } = await api.get<Subscription[]>(`/api/subscriptions/by-customer/${customerId}`)
  return data
}
