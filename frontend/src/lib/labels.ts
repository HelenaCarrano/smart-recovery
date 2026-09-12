import type { BadgeTone } from '@/components/ui/Badge'
import type { DeclineReason } from '@/types/payment'
import type { PlanPeriodicity } from '@/types/plan'
import type { RecoveryAction } from '@/types/recovery'

const DECLINE_REASON_LABELS: Record<DeclineReason, string> = {
  InsufficientFunds: 'Saldo insuficiente',
  ExpiredCard: 'Cartão expirado',
  InvalidCard: 'Cartão inválido',
  TemporaryError: 'Erro temporário',
  BlockedCard: 'Cartão bloqueado',
  SuspectedFraud: 'Suspeita de fraude',
  CardLimitExceeded: 'Limite do cartão excedido',
  SecurityCodeInvalid: 'Código de segurança inválido',
  IssuerUnavailable: 'Banco emissor indisponível',
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
  SendPaymentReminderEmail: { label: 'Enviar e-mail de regularização', tone: 'attention' },
  CancelSubscription: { label: 'Cancelar assinatura', tone: 'declined' },
  ManualReview: { label: 'Revisão manual', tone: 'attention' },
}

export function recoveryActionLabel(action: RecoveryAction): string {
  return RECOVERY_ACTION_CONFIG[action].label
}

const PERIODICITY_LABELS: Record<PlanPeriodicity, string> = {
  Monthly: 'Mensal',
  Quarterly: 'Trimestral',
  Annual: 'Anual',
}

export function periodicityLabel(periodicity: PlanPeriodicity): string {
  return PERIODICITY_LABELS[periodicity] ?? periodicity
}
