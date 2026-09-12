import { Badge } from '@/components/ui/Badge'
import { RECOVERY_ACTION_CONFIG } from '@/lib/labels'
import type { RecoveryAction } from '@/types/recovery'

export function RecoveryActionBadge({ action }: { action: RecoveryAction }) {
  const config = RECOVERY_ACTION_CONFIG[action]
  return <Badge tone={config.tone}>{config.label}</Badge>
}
