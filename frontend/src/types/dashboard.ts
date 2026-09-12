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
  /** Soma do valor de todos os pagamentos aprovados — não é "receita recuperada". */
  totalRevenue: number
  pendingRecoveryActions: number
}
