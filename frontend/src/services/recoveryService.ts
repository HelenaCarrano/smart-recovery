import axios from 'axios'
import { api } from './api'
import type { RecoveryAnalysis } from '@/types/recovery'

export async function getPendingRecoveryAnalyses(): Promise<RecoveryAnalysis[]> {
  const { data } = await api.get<RecoveryAnalysis[]>('/api/recovery/pending')
  return data
}

/**
 * Retorna null (em vez de lançar) quando não existe análise para o pagamento —
 * isso é esperado para qualquer pagamento que nunca foi recusado, não é um erro.
 */
export async function getRecoveryAnalysisByPayment(paymentId: string): Promise<RecoveryAnalysis | null> {
  try {
    const { data } = await api.get<RecoveryAnalysis>(`/api/recovery/payments/${paymentId}`)
    return data
  } catch (error) {
    if (axios.isAxiosError(error) && error.response?.status === 404) return null
    throw error
  }
}
