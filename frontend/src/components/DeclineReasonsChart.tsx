import { Card } from '@/components/ui/Card'
import { EmptyState } from '@/components/ui/EmptyState'
import { ErrorState } from '@/components/ui/ErrorState'
import { LoadingState } from '@/components/ui/LoadingState'
import type { AsyncState } from '@/hooks/useAsyncData'
import { declineReasonLabel } from '@/lib/labels'
import type { DeclineReasonCount } from '@/types/dashboard'
import { Bar, BarChart, CartesianGrid, ResponsiveContainer, Tooltip, XAxis, YAxis } from 'recharts'

interface DeclineReasonsChartProps {
  state: AsyncState<DeclineReasonCount[]>
}

/** Cor de recusa (rose-600) — mesma paleta usada em StatusBadge/PaymentsBreakdownChart. */
const BAR_COLOR = '#e11d48'

export function DeclineReasonsChart({ state }: DeclineReasonsChartProps) {
  return (
    <Card>
      <h2 className="mb-4 text-sm font-semibold text-slate-900">Motivos de recusa</h2>

      {state.status === 'loading' && <LoadingState label="Carregando…" />}
      {state.status === 'error' && <ErrorState message={state.error} onRetry={state.refetch} />}

      {state.status === 'success' &&
        (state.data.length === 0 ? (
          <EmptyState
            title="Nenhuma recusa registrada"
            description="Ainda não há pagamentos recusados para exibir esta distribuição."
          />
        ) : (
          <div className="h-64">
            <ResponsiveContainer>
              <BarChart
                data={[...state.data]
                  .sort((a, b) => b.count - a.count)
                  .map((entry) => ({ label: declineReasonLabel(entry.declineReason), count: entry.count }))}
                layout="vertical"
                margin={{ left: 16 }}
              >
                <CartesianGrid strokeDasharray="3 3" horizontal={false} />
                <XAxis type="number" allowDecimals={false} />
                <YAxis type="category" dataKey="label" width={140} tick={{ fontSize: 12 }} />
                <Tooltip />
                <Bar dataKey="count" name="Pagamentos" fill={BAR_COLOR} radius={[0, 4, 4, 0]} />
              </BarChart>
            </ResponsiveContainer>
          </div>
        ))}
    </Card>
  )
}
