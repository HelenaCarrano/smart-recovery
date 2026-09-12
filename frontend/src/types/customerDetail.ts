import type { Customer } from './customer'
import type { Payment } from './payment'
import type { RecoveryAnalysis } from './recovery'
import type { Subscription } from './subscription'

/** Uma entrada do histórico de recuperação: o pagamento recusado + a análise correspondente. */
export interface RecoveryHistoryEntry {
  payment: Payment
  analysis: RecoveryAnalysis
}

export interface CustomerDetail {
  customer: Customer
  subscriptions: Subscription[]
  payments: Payment[]
  recoveryHistory: RecoveryHistoryEntry[]
}
