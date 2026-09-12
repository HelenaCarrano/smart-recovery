import { api } from './api'

export interface HealthStatus {
  status: string
  service: string
  version: string
  timestamp: string
}

export async function checkHealth(): Promise<HealthStatus> {
  const { data } = await api.get<HealthStatus>('/api/health')
  return data
}
