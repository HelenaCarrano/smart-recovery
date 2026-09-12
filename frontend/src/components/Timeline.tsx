import type { BadgeTone } from '@/components/ui/Badge'
import { formatDateTime } from '@/lib/format'
import type { TimelineStep } from '@/lib/paymentTimeline'
import { clsx } from 'clsx'

const DOT_COLOR: Record<BadgeTone, string> = {
  approved: 'bg-status-approved',
  declined: 'bg-status-declined',
  pending: 'bg-status-pending',
  attention: 'bg-status-attention',
  neutral: 'bg-slate-300',
}

export function Timeline({ steps }: { steps: TimelineStep[] }) {
  return (
    <ol>
      {steps.map((step, index) => (
        <li key={step.id} className="relative flex gap-3 pb-6 last:pb-0">
          {index < steps.length - 1 && (
            <span className="absolute top-3 left-[4.5px] h-full w-px bg-slate-200" aria-hidden="true" />
          )}
          <span className={clsx('relative z-10 mt-1.5 h-2.5 w-2.5 shrink-0 rounded-full', DOT_COLOR[step.tone])} />
          <div>
            <p className="text-sm font-medium text-slate-800">{step.label}</p>
            {step.timestamp && <p className="text-xs text-slate-400">{formatDateTime(step.timestamp)}</p>}
          </div>
        </li>
      ))}
    </ol>
  )
}
