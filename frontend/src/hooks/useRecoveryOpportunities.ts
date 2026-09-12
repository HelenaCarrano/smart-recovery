import { getCustomerById } from '@/services/customerService'
import { getPaymentById } from '@/services/paymentService'
import { getPendingRecoveryAnalyses } from '@/services/recoveryService'
import type { RecoveryOpportunity } from '@/types/recovery'
import { useAsyncData } from './useAsyncData'

/**
 * GET /api/recovery/pending só traz o Recovery Score e a ação recomendada — não o
 * cliente, o valor ou o motivo da recusa. Não existe um endpoint único que já traga
 * tudo junto, então combinamos aqui com Payment (por id) e Customer (por id, só dos
 * clientes realmente referenciados — GET /api/customers agora é paginado e não serve
 * pra montar um mapa de todo mundo).
 */
async function fetchRecoveryOpportunities(): Promise<RecoveryOpportunity[]> {
  const analyses = await getPendingRecoveryAnalyses()
  if (analyses.length === 0) return []

  const payments = await Promise.all(analyses.map((analysis) => getPaymentById(analysis.paymentId)))

  const uniqueCustomerIds = [...new Set(payments.map((payment) => payment.customerId))]
  const customers = await Promise.all(uniqueCustomerIds.map((id) => getCustomerById(id)))
  const customerById = new Map(customers.map((customer) => [customer.id, customer]))

  return analyses.map((analysis, index) => {
    const payment = payments[index]
    const customer = customerById.get(payment.customerId)

    return {
      id: analysis.id,
      paymentId: payment.id,
      customerId: payment.customerId,
      customerName: customer?.name ?? 'Cliente não encontrado',
      amount: payment.amount,
      declineReason: payment.declineReason,
      recoveryScore: analysis.recoveryScore,
      recommendedAction: analysis.recommendedAction,
      scheduledRetryAt: payment.scheduledRetryAt,
      analyzedAt: analysis.analyzedAt,
    }
  })
}

export function useRecoveryOpportunities() {
  return useAsyncData(fetchRecoveryOpportunities, [])
}
