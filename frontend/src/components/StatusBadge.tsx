import { Badge, type BadgeTone } from '@/components/ui/Badge'
import type { PaymentStatus } from '@/types/payment'

const STATUS_CONFIG: Record<PaymentStatus, { label: string; tone: BadgeTone }> = {
  Pending: { label: 'Pendente', tone: 'pending' },
  Approved: { label: 'Aprovado', tone: 'approved' },
  Declined: { label: 'Recusado', tone: 'declined' },
  Cancelled: { label: 'Cancelado', tone: 'neutral' },
  Refunded: { label: 'Estornado', tone: 'attention' },
}

export function StatusBadge({ status }: { status: PaymentStatus }) {
  const config = STATUS_CONFIG[status]
  return <Badge tone={config.tone}>{config.label}</Badge>
}
