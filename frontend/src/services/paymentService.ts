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

/**
 * Não existe um endpoint que liste todos os pagamentos da plataforma — só por
 * cliente. A listagem geral de Pagamentos é montada combinando esta chamada por
 * cliente com GET /api/customers (ver hooks/usePayments.ts).
 */
export async function getPaymentsByCustomer(customerId: string): Promise<Payment[]> {
  const { data } = await api.get<Payment[]>(`/api/payments/by-customer/${customerId}`)
  return data
}

export async function getPaymentAttempts(paymentId: string): Promise<PaymentAttempt[]> {
  const { data } = await api.get<PaymentAttempt[]>(`/api/payments/${paymentId}/attempts`)
  return data
}
