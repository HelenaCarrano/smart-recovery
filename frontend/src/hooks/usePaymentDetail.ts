import { getCustomerById } from '@/services/customerService'
import { getPaymentAttempts, getPaymentById } from '@/services/paymentService'
import { getRecoveryAnalysisByPayment } from '@/services/recoveryService'
import { getSubscriptionById } from '@/services/subscriptionService'
import type { PaymentDetail } from '@/types/paymentDetail'
import { useAsyncData } from './useAsyncData'

async function fetchPaymentDetail(paymentId: string): Promise<PaymentDetail> {
  const payment = await getPaymentById(paymentId)

  const [customer, subscription, attempts, recoveryAnalysis] = await Promise.all([
    getCustomerById(payment.customerId),
    getSubscriptionById(payment.subscriptionId),
    getPaymentAttempts(payment.id),
    getRecoveryAnalysisByPayment(payment.id),
  ])

  return {
    payment,
    customerName: customer.name,
    planName: subscription.planName,
    attempts,
    recoveryAnalysis,
  }
}

export function usePaymentDetail(paymentId: string) {
  return useAsyncData(() => fetchPaymentDetail(paymentId), [paymentId])
}
