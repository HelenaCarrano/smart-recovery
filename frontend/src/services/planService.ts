import { api } from './api'
import type { Plan } from '@/types/plan'

export async function getActivePlans(): Promise<Plan[]> {
  const { data } = await api.get<Plan[]>('/api/plans')
  return data
}

export async function getPlanById(id: string): Promise<Plan> {
  const { data } = await api.get<Plan>(`/api/plans/${id}`)
  return data
}
