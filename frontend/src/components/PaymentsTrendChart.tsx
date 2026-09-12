import { Card } from '@/components/ui/Card'
import { EmptyState } from '@/components/ui/EmptyState'
import { ErrorState } from '@/components/ui/ErrorState'
import { LoadingState } from '@/components/ui/LoadingState'
import type { AsyncState } from '@/hooks/useAsyncData'
import { formatDate } from '@/lib/format'
import type { PaymentTrendPoint } from '@/types/dashboard'
import { Area, AreaChart, CartesianGrid, Legend, ResponsiveContainer, Tooltip, XAxis, YAxis } from 'recharts'

interface PaymentsTrendChartProps {
  state: AsyncState<PaymentTrendPoint[]>
}

/** Mesmas cores semânticas de aprovado/recusado usadas no resto do app. */
const COLORS = { approved: '#059669', declined: '#e11d48' } as const

export function PaymentsTrendChart({ state }: PaymentsTrendChartProps) {
  return (
    <Card>
      <h2 className="mb-4 text-sm font-semibold text-slate-900">Pagamentos ao longo do tempo</h2>

      {state.status === 'loading' && <LoadingState label="Carregando…" />}
      {state.status === 'error' && <ErrorState message={state.error} onRetry={state.refetch} />}

      {state.status === 'success' &&
        (state.data.length === 0 ? (
          <EmptyState
            title="Sem histórico suficiente"
            description="Ainda não há pagamentos no período selecionado para exibir a tendência."
          />
        ) : (
          <div className="h-64">
            <ResponsiveContainer>
              <AreaChart data={state.data.map((point) => ({ ...point, label: formatDate(point.date) }))}>
                <CartesianGrid strokeDasharray="3 3" vertical={false} />
                <XAxis dataKey="label" tick={{ fontSize: 12 }} minTickGap={24} />
                <YAxis allowDecimals={false} tick={{ fontSize: 12 }} />
                <Tooltip />
                <Legend />
                <Area
                  type="monotone"
                  dataKey="approved"
                  name="Aprovados"
                  stroke={COLORS.approved}
                  fill={COLORS.approved}
                  fillOpacity={0.15}
                />
                <Area
                  type="monotone"
                  dataKey="declined"
                  name="Recusados"
                  stroke={COLORS.declined}
                  fill={COLORS.declined}
                  fillOpacity={0.15}
                />
              </AreaChart>
            </ResponsiveContainer>
          </div>
        ))}
    </Card>
  )
}
