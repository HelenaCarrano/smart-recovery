import { Badge, type BadgeTone } from '@/components/ui/Badge'
import type { SubscriptionStatus } from '@/types/subscription'

const STATUS_CONFIG: Record<SubscriptionStatus, { label: string; tone: BadgeTone }> = {
  Active: { label: 'Ativa', tone: 'approved' },
  Paused: { label: 'Pausada', tone: 'pending' },
  Cancelled: { label: 'Cancelada', tone: 'neutral' },
}

export function SubscriptionStatusBadge({ status }: { status: SubscriptionStatus }) {
  const config = STATUS_CONFIG[status]
  return <Badge tone={config.tone}>{config.label}</Badge>
}
