import type { DeclineReason } from './payment'

/** Espelha exatamente o JSON retornado por GET /api/dashboard/summary. */
export interface DashboardSummary {
  totalCustomers: number
  activeSubscriptions: number
  totalPayments: number
  approvedPayments: number
  declinedPayments: number
  pendingPayments: number
  recoveredPayments: number
  /** Fração entre 0 e 1 (ex: 0.42 = 42%), não um percentual pronto. */
  recoveryRate: number
  /** Soma do valor de todos os pagamentos aprovados. */
  totalRevenue: number
  /** Soma do valor dos pagamentos aprovados que precisaram de mais de uma tentativa. */
  recoveredRevenue: number
  pendingRecoveryActions: number
}

export interface DeclineReasonCount {
  declineReason: DeclineReason
  count: number
}

/** Um ponto do gráfico de tendência: aprovados x recusados em um dia. */
export interface PaymentTrendPoint {
  date: string
  approved: number
  declined: number
}
