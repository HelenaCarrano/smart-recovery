import { getCustomerById } from '@/services/customerService'
import { getPaymentsByCustomer } from '@/services/paymentService'
import { getRecoveryAnalysisByPayment } from '@/services/recoveryService'
import { getSubscriptionsByCustomer } from '@/services/subscriptionService'
import type { CustomerDetail, RecoveryHistoryEntry } from '@/types/customerDetail'
import { useAsyncData } from './useAsyncData'

async function fetchCustomerDetail(customerId: string): Promise<CustomerDetail> {
  const [customer, subscriptions, payments] = await Promise.all([
    getCustomerById(customerId),
    getSubscriptionsByCustomer(customerId),
    getPaymentsByCustomer(customerId),
  ])

  // Histórico de recuperação só existe para pagamentos que já foram recusados alguma vez.
  const declinedPayments = payments.filter((payment) => payment.declineReason !== null)
  const analyses = await Promise.all(declinedPayments.map((payment) => getRecoveryAnalysisByPayment(payment.id)))

  const recoveryHistory: RecoveryHistoryEntry[] = declinedPayments
    .map((payment, index) => {
      const analysis = analyses[index]
      return analysis ? { payment, analysis } : null
    })
    .filter((entry): entry is RecoveryHistoryEntry => entry !== null)

  return { customer, subscriptions, payments, recoveryHistory }
}

export function useCustomerDetail(customerId: string) {
  return useAsyncData(() => fetchCustomerDetail(customerId), [customerId])
}
