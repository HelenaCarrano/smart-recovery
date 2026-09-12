import { declineReasonLabel } from './labels'
import type { DeclineReason } from '@/types/payment'
import type { RecoveryAction } from '@/types/recovery'

/** Texto em linguagem simples explicando por que o RecoveryDecisionEngine (backend) escolheu essa ação. */
export function explainRecommendedAction(action: RecoveryAction, declineReason: DeclineReason | null): string {
  switch (action) {
    case 'RetryIn2Hours':
      return 'O motivo da recusa costuma se resolver sozinho rapidamente e o histórico do cliente é bom, então uma nova tentativa foi agendada para daqui a 2 horas.'
    case 'RetryIn24Hours':
      return 'O histórico do cliente indica boa chance de recuperação. Uma nova tentativa foi agendada para daqui a 24 horas.'
    case 'RetryIn72Hours':
      return 'O cliente tem um histórico moderado. Uma nova tentativa foi agendada para daqui a 72 horas, dando tempo para o problema se resolver (ex: saldo).'
    case 'RequestPaymentMethodUpdate':
      return `Este pagamento apresenta baixa probabilidade de recuperação automática${declineReason ? ` (${declineReasonLabel(declineReason).toLowerCase()})` : ''} e requer que o cliente atualize a forma de pagamento.`
    case 'CancelSubscription':
      return 'O score de recuperação está muito baixo e há sinais consistentes de que novas tentativas não seriam bem-sucedidas, então a assinatura foi cancelada automaticamente.'
    case 'ManualReview':
      return 'A situação é ambígua demais para decidir automaticamente e requer revisão de um analista antes de qualquer ação.'
  }
}
