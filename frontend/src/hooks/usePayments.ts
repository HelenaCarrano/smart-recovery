import { getCustomers } from '@/services/customerService'
import { getPaymentsByCustomer } from '@/services/paymentService'
import { getRecoveryAnalysisByPayment } from '@/services/recoveryService'
import { toPaymentListItem, type PaymentListItem } from '@/types/paymentListItem'
import { useAsyncData } from './useAsyncData'

/**
 * Não existe GET /api/payments (lista geral) — só por cliente. Buscamos todos os
 * clientes e, para cada um, seus pagamentos (endpoints que já existem), e juntamos
 * tudo em uma única lista. Para os pagamentos recusados, buscamos também a análise
 * de recovery correspondente (score + ação recomendada).
 */
async function fetchAllPayments(): Promise<PaymentListItem[]> {
  const customers = await getCustomers()

  const paymentsByCustomer = await Promise.all(
    customers.map(async (customer) => {
      const payments = await getPaymentsByCustomer(customer.id)
      return payments.map((payment) => ({ payment, customerName: customer.name }))
    }),
  )

  const flattened = paymentsByCustomer.flat()

  return Promise.all(
    flattened.map(async ({ payment, customerName }) => {
      const analysis =
        payment.status === 'Declined' ? await getRecoveryAnalysisByPayment(payment.id) : null

      return toPaymentListItem(
        payment,
        customerName,
        analysis && { recoveryScore: analysis.recoveryScore, recommendedAction: analysis.recommendedAction },
      )
    }),
  )
}

export function usePayments() {
  return useAsyncData(fetchAllPayments, [])
}
