import { api } from './api'
import type { Subscription } from '@/types/subscription'

export async function getSubscriptionById(id: string): Promise<Subscription> {
  const { data } = await api.get<Subscription>(`/api/subscriptions/${id}`)
  return data
}
