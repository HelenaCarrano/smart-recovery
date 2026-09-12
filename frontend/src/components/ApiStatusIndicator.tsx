import { Badge, type BadgeTone } from '@/components/ui/Badge'
import { useApiStatus } from '@/hooks/useApiStatus'

const STATUS_CONFIG: Record<ReturnType<typeof useApiStatus>, { label: string; tone: BadgeTone }> = {
  checking: { label: 'Verificando API…', tone: 'neutral' },
  online: { label: 'API conectada', tone: 'approved' },
  offline: { label: 'API indisponível', tone: 'declined' },
}

export function ApiStatusIndicator() {
  const status = useApiStatus()
  const { label, tone } = STATUS_CONFIG[status]

  return (
    <Badge tone={tone} dot>
      {label}
    </Badge>
  )
}
