import type { BadgeTone } from '@/components/ui/Badge'
import { Card } from '@/components/ui/Card'
import { clsx } from 'clsx'
import type { ComponentType, SVGProps } from 'react'

interface KpiCardProps {
  label: string
  value: string
  caption?: string
  tone?: BadgeTone
  icon: ComponentType<SVGProps<SVGSVGElement>>
}

const ICON_CLASSES: Record<BadgeTone, string> = {
  approved: 'bg-status-approved-bg text-status-approved',
  declined: 'bg-status-declined-bg text-status-declined',
  pending: 'bg-status-pending-bg text-status-pending',
  attention: 'bg-status-attention-bg text-status-attention',
  neutral: 'bg-slate-100 text-slate-500',
}

export function KpiCard({ label, value, caption, tone = 'neutral', icon: Icon }: KpiCardProps) {
  return (
    <Card className="flex items-start gap-4">
      <span className={clsx('flex h-10 w-10 shrink-0 items-center justify-center rounded-lg', ICON_CLASSES[tone])}>
        <Icon className="h-5 w-5" />
      </span>
      <div className="min-w-0">
        <p className="text-sm text-slate-500">{label}</p>
        <p className="mt-1 text-2xl font-semibold text-slate-900">{value}</p>
        {caption && <p className="mt-1 text-xs text-slate-400">{caption}</p>}
      </div>
    </Card>
  )
}
