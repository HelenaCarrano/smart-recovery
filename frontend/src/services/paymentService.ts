import { api } from './api'
import type { DeclineReason, Payment, PaymentStatus } from '@/types/payment'
import type { PaymentAttempt } from '@/types/paymentAttempt'
import type { PagedResult } from '@/types/pagedResult'
import type { PaymentListItem } from '@/types/paymentListItem'

export interface PaymentQueryParams {
  page: number
  pageSize: number
  status?: PaymentStatus
  declineReason?: DeclineReason
  customerId?: string
  dateFrom?: string
  dateTo?: string
}

export async function getPayments(params: PaymentQueryParams): Promise<PagedResult<PaymentListItem>> {
  const { data } = await api.get<PagedResult<PaymentListItem>>('/api/payments', { params })
  return data
}

export async function getPaymentById(id: string): Promise<Payment> {
  const { data } = await api.get<Payment>(`/api/payments/${id}`)
  return data
}

/** Histórico de pagamentos de um cliente — usado na página de detalhe do cliente. */
export async function getPaymentsByCustomer(customerId: string): Promise<Payment[]> {
  const { data } = await api.get<Payment[]>(`/api/payments/by-customer/${customerId}`)
  return data
}

export async function getPaymentAttempts(paymentId: string): Promise<PaymentAttempt[]> {
  const { data } = await api.get<PaymentAttempt[]>(`/api/payments/${paymentId}/attempts`)
  return data
}
