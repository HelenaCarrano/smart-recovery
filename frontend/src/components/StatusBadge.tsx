import { Badge, type BadgeTone } from '@/components/ui/Badge'
import type { PaymentStatus } from '@/types/payment'

const STATUS_CONFIG: Record<PaymentStatus, { label: string; tone: BadgeTone }> = {
  Pending: { label: 'Pendente', tone: 'pending' },
  Approved: { label: 'Aprovado', tone: 'approved' },
  Declined: { label: 'Recusado', tone: 'declined' },
  Cancelled: { label: 'Cancelado', tone: 'neutral' },
  Refunded: { label: 'Estornado', tone: 'attention' },
}

export function StatusBadge({ status, attemptCount }: { status: PaymentStatus; attemptCount?: number }) {
  if (status === 'Approved' && attemptCount !== undefined) {
    return attemptCount > 1 ? (
      <Badge tone="approved">Recuperado ✓</Badge>
    ) : (
      <Badge tone="approved">Aprovado na 1ª tentativa</Badge>
    )
  }

  const config = STATUS_CONFIG[status]
  return <Badge tone={config.tone}>{config.label}</Badge>
}
