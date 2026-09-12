import type { BadgeTone } from '@/components/ui/Badge'
import type { DeclineReason } from '@/types/payment'
import type { RecoveryAction } from '@/types/recovery'

const DECLINE_REASON_LABELS: Record<DeclineReason, string> = {
  InsufficientFunds: 'Saldo insuficiente',
  ExpiredCard: 'Cartão expirado',
  InvalidCard: 'Cartão inválido',
  TemporaryError: 'Erro temporário',
  BlockedCard: 'Cartão bloqueado',
  Unknown: 'Motivo desconhecido',
}

export function declineReasonLabel(reason: DeclineReason | null): string {
  if (!reason) return '—'
  return DECLINE_REASON_LABELS[reason] ?? reason
}

export const RECOVERY_ACTION_CONFIG: Record<RecoveryAction, { label: string; tone: BadgeTone }> = {
  RetryIn2Hours: { label: 'Retentar em 2h', tone: 'pending' },
  RetryIn24Hours: { label: 'Retentar em 24h', tone: 'pending' },
  RetryIn72Hours: { label: 'Retentar em 72h', tone: 'pending' },
  RequestPaymentMethodUpdate: { label: 'Atualizar forma de pagamento', tone: 'attention' },
  CancelSubscription: { label: 'Cancelar assinatura', tone: 'declined' },
  ManualReview: { label: 'Revisão manual', tone: 'attention' },
}

export function recoveryActionLabel(action: RecoveryAction): string {
  return RECOVERY_ACTION_CONFIG[action].label
}
