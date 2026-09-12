import type { Payment } from './payment'
import type { PaymentAttempt } from './paymentAttempt'
import type { RecoveryAnalysis } from './recovery'

/** View model combinando Payment + Customer + Subscription/Plan + tentativas + análise de recovery. */
export interface PaymentDetail {
  payment: Payment
  customerName: string
  planName: string
  attempts: PaymentAttempt[]
  /** null quando o pagamento nunca foi recusado — não existe análise para ele. */
  recoveryAnalysis: RecoveryAnalysis | null
}
